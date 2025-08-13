using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "NWAY_", menuName = "Danmaku/Pattern/Shoot/Multi-Way")]
public class MultiWayPattern : ShootPatternBase
{
    [Header("N-Way弾の設定")]
    [SerializeField] private IntReference wayCount = new IntReference { useConstant = true, constantValue = 5 };
    [SerializeField] private FloatReference totalAngle = new FloatReference { useConstant = true, constantValue = 90f };
    [SerializeField] private BoolReference allRound = new BoolReference { useConstant = true, constantValue = false };
    [SerializeField] private RotationDirectionReference rotationDirection = new RotationDirectionReference { useConstant = true, constantValue = RotationDirection.CounterClockwise };

    [Header("N-Way弾の角度")]
    [Tooltip("全方位で自機外し")]
    [SerializeField] private BoolReference avoidAtPlayer = new BoolReference { useConstant = true, constantValue = false };

    [Header("角度の共有と更新")]
    [Tooltip("角度の入出力に使用するパラメータ。未設定の場合は内部で角度を管理します。")]
    [SerializeField] private AngleParameterReference angleParameter;
    [Tooltip("実行開始時に、Angle Parameterを初期化するかどうか。")]
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

        Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
        float baseAngle = controller.transform.eulerAngles.z;
        bool useAvoidAtPlayer = allRound.Value && avoidAtPlayer.Value;
        float directionMultiplier = (rotationDirection.Value == RotationDirection.CounterClockwise) ? 1f : -1f;

        // --- 実行開始時の中央角度を決定 ---
        float centerAngle;
        if (angleParameter != null && angleParameter.Value != null)
        {
            if (initializeParameterOnStart.Value)
            {
                float initialAngle;
                if (aimAtPlayer.Value)
                {
                    initialAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
                }
                else
                {
                    initialAngle = baseAngle + emissionData.localAngle;
                }
                angleParameter.Value.Value = initialAngle + directionOffset.Value;
            }
            centerAngle = angleParameter.Value.Value;
        }
        else
        {
            if (aimAtPlayer.Value)
            {
                centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
            }
            else
            {
                centerAngle = baseAngle + emissionData.localAngle;
            }
            centerAngle += directionOffset.Value;
        }
        // --------------------------

        float finalAngle = allRound.Value ? 360f : totalAngle.Value;
        float startAngleOffset = -finalAngle / 2 * directionMultiplier;
        float angleStep = allRound.Value ? finalAngle / wayCount.Value : ((wayCount.Value > 1) ? finalAngle / (wayCount.Value - 1) : 0f);

        for (int i = 0; i < wayCount.Value; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = baseSpawnPosition;
            if (followShooterPosition.Value && (emissionShape == null || emissionShape.Value == null))
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            float loopCenterAngle = centerAngle;
            if (alwaysAimToPlayer.Value)
            {
                loopCenterAngle = AngleUtility.GetAngleToPlayer(currentSpawnPosition) + 180f + directionOffset.Value;
            }

            float currentAngle = loopCenterAngle + (allRound.Value ? 0 : startAngleOffset) + (useAvoidAtPlayer ? angleStep / 2 : 0) + (angleStep * i * directionMultiplier);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, currentAngle);
            controller.InstantiateProperty(_entity.Value, finalSpawnPosition, rotation);
        }

        // --- 次回実行のために最後の角度を保存 ---
        if (angleParameter != null && angleParameter.Value != null)
        {
            angleParameter.Value.Value = centerAngle + accumulatingOffset.Value;
        }
        // ------------------------------------

        await UniTask.CompletedTask;
    }
}
