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

    public override async UniTask Execute(Transform spawnPoint, GameObject inheritedBulletPrefab, CancellationToken token)
    {
        // このパターンが使う弾を決定
        GameObject bulletToUse = this.overrideBulletPrefab != null ? this.overrideBulletPrefab : inheritedBulletPrefab;

        if (bulletToUse == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        if (token.IsCancellationRequested) return;

        // aimAtPlayerがfalseのときは無効化
        alwaysAimToPlayer = aimAtPlayer && alwaysAimToPlayer;

        float centerAngle = spawnPoint.eulerAngles.z;
        if (aimAtPlayer)
        {
            centerAngle = AngleUtility.GetAngleToPlayer(spawnPoint.position);
        }

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2;
        float endAngle = finalAngle / 2;

        Quaternion originalRotation = spawnPoint.rotation;

        for (int i = 0; i < scatterCount; i++)
        {
            if (token.IsCancellationRequested) break;

            float scatterAngle = centerAngle + Random.Range(startAngle, endAngle);
            spawnPoint.rotation = Quaternion.Euler(0, 0, scatterAngle);

            Instantiate(bulletToUse, spawnPoint.position, spawnPoint.rotation);

            // 回転を元に戻す
            spawnPoint.rotation = originalRotation;

            // intervalだけ待つ
            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }

            if (alwaysAimToPlayer)
            {
               centerAngle = AngleUtility.GetAngleToPlayer(spawnPoint.position);
            }
        }

        await UniTask.CompletedTask;
    }
}
