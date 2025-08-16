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

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Value == null || _entity.Value.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
        bool useAvoidAtPlayer = allRound.Value && avoidAtPlayer.Value;
        float directionMultiplier = (rotationDirection.Value == RotationDirection.CounterClockwise) ? 1f : -1f;

        // --- 実行開始時の中央角度を決定 ---
        float centerAngle = InitializeBaseAngle(controller, emissionData, baseSpawnPosition);

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
                // 毎回自機狙い角度を再計算
                loopCenterAngle = CalculateShootAngle(controller, emissionData, currentSpawnPosition);
            }

            float currentAngle = loopCenterAngle + (allRound.Value ? 0 : startAngleOffset) + (useAvoidAtPlayer ? angleStep / 2 : 0) + (angleStep * i * directionMultiplier);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, currentAngle);
            controller.InstantiateProperty(_entity.Value, finalSpawnPosition, rotation);
        }

        // 内部角度状態を更新（次回実行のため） - 中央角度から基準を引いた差分を保存
        Vector3 initialSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
        float baseCenterAngle = aimAtPlayer.Value ? AngleUtility.GetAngleToPlayer(initialSpawnPosition) + 180f : controller.transform.eulerAngles.z + emissionData.localAngle;
        float finalInternalAngle = centerAngle - baseCenterAngle;
        SetInternalAngle(finalInternalAngle);
        
        // --- パターン終了時のオフセット処理 ---
        ApplyPostExecutionAngleOffset();

        await UniTask.CompletedTask;
    }
}
