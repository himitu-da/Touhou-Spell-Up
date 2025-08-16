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

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Value == null || _entity.Value.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        float directionMultiplier = (rotationDirection.Value == RotationDirection.CounterClockwise) ? 1f : -1f;
        
        // --- 実行開始時の角度を決定 ---
        Vector3 initialSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
        float baseAngle = InitializeBaseAngle(controller, emissionData, initialSpawnPosition);
        float currentAngle = baseAngle + startAngle.Value;

        for (int i = 0; ; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
            if (followShooterPosition.Value && (emissionShape == null || emissionShape.Value == null))
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            float shotAngle;
            if (alwaysAimToPlayer.Value)
            {
                // 毎回自機狙い + 内部角度（相対角度として）
                shotAngle = CalculateShootAngle(controller, emissionData, currentSpawnPosition) + startAngle.Value + (directionMultiplier * intervalAngle.Value * i);
            }
            else
            {
                // 通常の角度計算
                shotAngle = currentAngle;
            }

            Quaternion rotation = Quaternion.Euler(0, 0, shotAngle);
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, shotAngle);

            controller.InstantiateProperty(_entity.Value, finalSpawnPosition, rotation);

            // 次の弾のために角度を更新
            float angleIncrement = directionMultiplier * intervalAngle.Value;
            currentAngle += angleIncrement;

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

        // 内部角度状態を更新（次回実行のため）- 最後のcurrentAngleから基準角度を引いて差分を保存
        float finalInternalAngle = currentAngle - (aimAtPlayer.Value ? AngleUtility.GetAngleToPlayer(initialSpawnPosition) + 180f : controller.transform.eulerAngles.z + emissionData.localAngle) - startAngle.Value;
        SetInternalAngle(finalInternalAngle);
        
        // --- パターン終了時のオフセット処理 ---
        ApplyPostExecutionAngleOffset();

        await UniTask.CompletedTask;
    }
}
