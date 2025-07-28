using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SCTR_", menuName = "Touhou Spell Up/Danmaku/Shoot/Scattering")]
public class ScatteringPattern : ShootPatternBase
{
    [Header("ばらまき弾の設定")]
    // 分布方法＝正規分布、ランダムを将来的に追加する
    [SerializeField, Range(1, 100)] private int scatterCount = 10;
    [SerializeField, Range(0f, 10f)] private float interval = 0.5f;
    [SerializeField, Range(0f, 360f)] private float totalAngle = 60f;
    [SerializeField] private bool allRound;

    public override async UniTask ExecuteShoot(EntityController controller, CancellationToken token)
    {
        if (_entity == null || _entity.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        var emissions = new List<EmissionData>();
        if (emissionShape != null)
        {
            emissions.AddRange(emissionShape.GetEmissions(controller));
        }
        else
        {
            // EmissionShapeがない場合は、従来通り単一の発生源を追加
            emissions.Add(new EmissionData { localPosition = positionOffset, localAngle = 0 });
        }

        if (emissions.Count == 0)
        {
            Debug.LogWarning("発生源がありません。", this);
            return;
        }

        bool useAlwaysAim = this.aimAtPlayer && this.alwaysAimToPlayer;
        float finalAngle = allRound ? 360f : totalAngle;
        float startAngle = -finalAngle / 2;
        float endAngle = finalAngle / 2;

        for (int i = 0; i < scatterCount; i++)
        {
            if (token.IsCancellationRequested) break;

            // 発生源をランダムに選択
            EmissionData emissionData = emissions[Random.Range(0, emissions.Count)];

            Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
            if (followShooterPosition)
            {
                baseSpawnPosition = GetSpawnPosition(controller) + controller.transform.rotation * emissionData.localPosition;
            }

            float centerAngle;
            if (aimAtPlayer)
            {
                centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
            }
            else
            {
                centerAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
            }
            centerAngle += directionOffset;

            if (useAlwaysAim)
            {
                centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f + directionOffset;
            }

            float scatterAngle = centerAngle + Random.Range(startAngle, endAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, scatterAngle);

            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, scatterAngle);
            controller.InstantiateProperty(_entity, finalSpawnPosition, rotation);

            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }
        }
    }

    // このメソッドはExecuteShootでロジックを実装したため、空にするか例外をスローする
    public override UniTask ExecuteShootFromPoint(EntityController controller, EmissionData emissionData, CancellationToken token)
    {
        // このパターンでは、ExecuteShootで全て処理するため、ここは使用しない
        return UniTask.CompletedTask;
    }
}
