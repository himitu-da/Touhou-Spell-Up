using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SCTR_", menuName = "Danmaku/Pattern/Shoot/Scattering")]
public class ScatteringPattern : ShootPatternBase
{
    [Header("ばらまき弾の設定")]
    // 分布方法＝正規分布、ランダムを将来的に追加する
    [SerializeField] private IntReference scatterCount = new IntReference { useConstant = true, constantValue = 10 };
    [SerializeField] private FloatReference interval = new FloatReference { useConstant = true, constantValue = 0.5f };
    [SerializeField] private FloatReference totalAngle = new FloatReference { useConstant = true, constantValue = 60f };
    [SerializeField] private BoolReference allRound = new BoolReference { useConstant = true, constantValue = false };

    public override async UniTask ExecuteShoot(GameEntityController controller, CancellationToken token)
    {
        if (_entity == null || _entity.Value == null || _entity.Value.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        var emissions = new List<EmissionData>();
        if (emissionShape != null && emissionShape.Value != null)
        {
            emissions.AddRange(emissionShape.Value.GetEmissions(controller));
        }
        else
        {
            // Emissionがない場合は、従来通り単一の発生源を追加
            emissions.Add(new EmissionData { localPosition = positionOffset.Value, localAngle = 0 });
        }

        if (emissions.Count == 0)
        {
            Debug.LogWarning("発生源がありません。", this);
            return;
        }

        bool useAlwaysAim = this.aimAtPlayer.Value && this.alwaysAimToPlayer.Value;
        float finalAngle = allRound.Value ? 360f : totalAngle.Value;
        float startAngle = -finalAngle / 2;
        float endAngle = finalAngle / 2;

        for (int i = 0; i < scatterCount.Value; i++)
        {
            if (token.IsCancellationRequested) break;

            // 発生源をランダムに選択
            EmissionData emissionData = emissions[Random.Range(0, emissions.Count)];

            Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
            if (followShooterPosition.Value)
            {
                baseSpawnPosition = GetSpawnPosition(controller) + controller.transform.rotation * emissionData.localPosition;
            }

            float centerAngle;
            if (aimAtPlayer.Value)
            {
                centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
            }
            else
            {
                centerAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
            }
            centerAngle += directionOffset.Value;

            if (useAlwaysAim)
            {
                centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f + directionOffset.Value;
            }

            float scatterAngle = centerAngle + Random.Range(startAngle, endAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, scatterAngle);

            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, scatterAngle);
            controller.InstantiateProperty(_entity.Value, finalSpawnPosition, rotation);

            if (interval.Value > 0)
            {
                float waitTime = interval.Value;
                while (waitTime > 0 && !token.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                    waitTime -= Time.fixedDeltaTime;
                }
            }
        }
    }

    // このメソッドはExecuteShootでロジックを実装したため、空にするか例外をスローする
    public override UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        // このパターンでは、ExecuteShootで全て処理するため、ここは使用しない
        return UniTask.CompletedTask;
    }
}
