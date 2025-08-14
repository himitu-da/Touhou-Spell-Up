using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "RTS_", menuName = "Danmaku/Pattern/Shoot/RotatingShot")]
public class RotatingShotPattern : ShootPatternBase
{
    [Header("基本設定")]
    [SerializeField] private FloatReference startAngle = new FloatReference { useConstant = true, constantValue = 0f };
    [SerializeField] private FloatReference intervalAngle = new FloatReference { useConstant = true, constantValue = 10f };
    [SerializeField] private FloatReference intervalTime = new FloatReference { useConstant = true, constantValue = 0.5f };
    [SerializeField] private IntReference shotCount = new IntReference { useConstant = true, constantValue = 1 };
    [SerializeField] private RotationDirectionReference rotationDirection = new RotationDirectionReference { useConstant = true, constantValue = RotationDirection.CounterClockwise };

    [Header("角度の共有と更新")]
    [Tooltip("角度の入出力に使用するパラメータ。未設定の場合は内部で角度を管理します。")]
    [SerializeField] private AngleParameterReference angleParameter;
    [Tooltip("実行開始時に、StartAngleの値でAngle Parameterを初期化するかどうか。")]
    [SerializeField] private BoolReference initializeParameterOnStart = new BoolReference { useConstant = true, constantValue = true };
    [Tooltip("実行後にAngle Parameterに加算するオフセット値。")]
    [SerializeField] private FloatReference accumulatingOffset = new FloatReference { useConstant = true, constantValue = 0f };

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Value == null || _entity.Value.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        float directionMultiplier = (rotationDirection.Value == RotationDirection.CounterClockwise) ? 1f : -1f;
        
        // --- 実行開始時の角度を決定 ---
        float currentAngle;
        Vector3 initialSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;

        if (angleParameter != null && angleParameter.Value != null)
        {
            if (initializeParameterOnStart.Value)
            {
                float initialAngle;
                if (aimAtPlayer.Value)
                {
                    initialAngle = AngleUtility.GetAngleToPlayer(initialSpawnPosition) + 180f;
                }
                else
                {
                    initialAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
                }
                angleParameter.Value.Value = initialAngle + startAngle.Value;
            }
            currentAngle = angleParameter.Value.Value;
            currentAngle += directionOffset.Value;
        }
        else
        {
            // パラメータが未設定の場合、従来通りに初期角度を計算
            if (aimAtPlayer.Value)
            {
                currentAngle = AngleUtility.GetAngleToPlayer(initialSpawnPosition) + 180f;
            }
            else
            {
                currentAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
            }
            currentAngle += startAngle.Value;
            currentAngle += directionOffset.Value;
        }
        // --------------------------

        for (int i = 0; ; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
            if (followShooterPosition.Value && (emissionShape == null || emissionShape.Value == null))
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            // alwaysAimToPlayerが有効な場合、毎回プレイヤーの方向を向く
            if (alwaysAimToPlayer.Value)
            {
                float aimAngle = AngleUtility.GetAngleToPlayer(currentSpawnPosition) + 180f;
                // angleParameterが指定されていればそれを更新、なければ内部変数を使う
                if (angleParameter != null && angleParameter.Value != null)
                {
                    angleParameter.Value.Value = aimAngle + startAngle.Value;
                }
                else
                {
                    currentAngle = aimAngle + startAngle.Value;
                }
            }

            float shotAngle = (angleParameter != null && angleParameter.Value != null) ? angleParameter.Value.Value : currentAngle;
            Quaternion rotation = Quaternion.Euler(0, 0, shotAngle);
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, shotAngle);

            controller.InstantiateProperty(_entity.Value, finalSpawnPosition, rotation);

            // 次の弾のために角度を更新
            float angleIncrement = directionMultiplier * intervalAngle.Value;
            if (angleParameter != null && angleParameter.Value != null)
            {
                angleParameter.Value.Add(angleIncrement);
            }
            else
            {
                currentAngle += angleIncrement;
            }

            if (intervalTime.Value > 0)
            {
                float waitTime = intervalTime.Value;
                while (waitTime > 0 && !token.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                    waitTime -= Time.fixedDeltaTime;
                }
            }
            if (shotCount.Value > 0 && i >= shotCount.Value - 1)
            {
                break;
            }
        }

        // --- 次回実行のためにオフセットを加算 ---
        if (angleParameter != null && angleParameter.Value != null && accumulatingOffset.Value != 0)
        {
            angleParameter.Value.Add(accumulatingOffset.Value);
        }
        // ------------------------------------

        await UniTask.CompletedTask;
    }
}
