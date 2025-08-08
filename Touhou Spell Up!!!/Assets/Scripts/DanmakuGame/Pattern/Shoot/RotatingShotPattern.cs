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
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.CounterClockwise;

    [Header("角度の共有と更新")]
    [Tooltip("角度の入出力に使用するパラメータ。未設定の場合は内部で角度を管理します。")]
    [SerializeField] private AngleParameter angleParameter;
    [Tooltip("実行開始時に、StartAngleの値でAngle Parameterを初期化するかどうか。")]
    [SerializeField] private BoolReference initializeParameterOnStart = new BoolReference { useConstant = true, constantValue = true };
    [Tooltip("実行後にAngle Parameterに加算するオフセット値。")]
    [SerializeField] private FloatReference accumulatingOffset = new FloatReference { useConstant = true, constantValue = 0f };

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;
        
        // --- 実行開始時の角度を決定 ---
        float currentAngle;
        Vector3 initialSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;

        if (angleParameter != null)
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
                angleParameter.Value = initialAngle + startAngle.Value;
            }
            currentAngle = angleParameter.Value;
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
        }
        // --------------------------

        for (int i = 0; ; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
            if (followShooterPosition.Value && emissionShape == null)
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            float loopStartAngle = currentAngle;
            if (alwaysAimToPlayer.Value)
            {
                loopStartAngle = AngleUtility.GetAngleToPlayer(currentSpawnPosition) + 180f + startAngle.Value;
            }

            float shotAngle = loopStartAngle + (directionMultiplier * intervalAngle.Value * i);
            Quaternion rotation = Quaternion.Euler(0, 0, shotAngle);
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, shotAngle);

            controller.InstantiateProperty(_entity, finalSpawnPosition, rotation);

            if (intervalTime.Value > 0)
            {
                await UniTask.Delay((int)(intervalTime.Value * 1000), cancellationToken: token);
            }
            if (shotCount.Value > 0 && i >= shotCount.Value - 1)
            {
                break;
            }
        }

        // --- 次回実行のために最後の角度を保存 ---
        if (angleParameter != null)
        {
            float lastAngle = currentAngle + (directionMultiplier * intervalAngle.Value * shotCount.Value);
            angleParameter.Value = lastAngle + accumulatingOffset.Value;
        }
        // ------------------------------------

        await UniTask.CompletedTask;
    }
}
