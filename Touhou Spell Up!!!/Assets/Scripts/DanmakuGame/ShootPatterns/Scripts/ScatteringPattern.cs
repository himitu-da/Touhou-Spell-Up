using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SCTR_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Scattering")]
public class ScatteringPattern : ShootPatternBase
{
    [Header("ばらまき弾の設定")]
    // 分布方法＝正規分布、ランダムを将来的に追加する
    [SerializeField, Range(1, 100)] private int scatterCount = 10;
    [SerializeField, Range(0f, 10f)] private float interval = 0.5f;
    [SerializeField, Range(0f, 360f)] private float totalAngle = 60f;
    [SerializeField] private bool allRound;

    [Header("自機狙い")]
    [SerializeField] private bool aimAtPlayer = false;
    [SerializeField] private bool alwaysAimToPlayer = false;

    public override async UniTask ExecuteImpl(Shooter shooter, Bullet bulletToUse, CancellationToken token)
    {
        if (bulletToUse == null || bulletToUse.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        // aimAtPlayerがfalseのときは無効化
        alwaysAimToPlayer = aimAtPlayer && alwaysAimToPlayer;

        Vector3 spawnPosition = GetSpawnPosition(shooter);

        float centerAngle = shooter.transform.eulerAngles.z;
        if (aimAtPlayer)
        {
            centerAngle = AngleUtility.GetAngleToPlayer(spawnPosition);
        }

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2;
        float endAngle = finalAngle / 2;

        for (int i = 0; i < scatterCount; i++)
        {
            if (token.IsCancellationRequested) break;

            float scatterAngle = centerAngle + Random.Range(startAngle, endAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, scatterAngle);

            var bulletInstance = Instantiate(bulletToUse.Prefab, spawnPosition, rotation);
            var enemyBullet = bulletInstance.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.Initialize(bulletToUse.Property);
            }

            // intervalだけ待つ
            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }

            if (alwaysAimToPlayer)
            {
               centerAngle = AngleUtility.GetAngleToPlayer(spawnPosition);
            }
        }

        await UniTask.CompletedTask;
    }
}
