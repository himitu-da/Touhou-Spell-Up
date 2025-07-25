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

    public override async UniTask ExecuteShoot(IMovable movable, IShootable shootable, CancellationToken token)
    {
        if (_bullet == null || _bullet.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        // aimAtPlayerがfalseのときは無効化
        this.alwaysAimToPlayer = this.aimAtPlayer && this.alwaysAimToPlayer;

        Vector3 baseSpawnPosition = GetSpawnPosition(movable);

        float centerAngle = GetAimAngle(movable, baseSpawnPosition) + directionOffset;

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2;
        float endAngle = finalAngle / 2;

        for (int i = 0; i < scatterCount; i++)
        {
            if (token.IsCancellationRequested) break;

            if (followShooterPosition)
            {
                baseSpawnPosition = GetSpawnPosition(movable);
            }

            if (alwaysAimToPlayer)
            {
                centerAngle = GetAimAngle(movable, baseSpawnPosition) + directionOffset;
            }

            float scatterAngle = centerAngle + Random.Range(startAngle, endAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, scatterAngle);

            // 最終的な発射位置を計算
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, scatterAngle);

            shootable.InstantiateBullet(_bullet, finalSpawnPosition, rotation);

            // intervalだけ待つ
            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }

        }

        await UniTask.CompletedTask;
    }
}
