using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BASIC_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Basic Shot")]
public class BasicShotPattern : ShootPatternBase
{
    public override async UniTask Execute(Transform spawnPoint, GameObject inheritedBulletPrefab, CancellationToken token)
    {
        // 自身の上書き設定があればそれを優先し、なければ親からの継承をそのまま使う
        GameObject bulletToUse = this.overrideBulletPrefab != null ? this.overrideBulletPrefab : inheritedBulletPrefab;

        if (bulletToUse == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        if (token.IsCancellationRequested) return;

        // spawnPointの位置と角度で、指定された弾を1つ生成する
        Instantiate(bulletToUse, spawnPoint.position, spawnPoint.rotation);

        await UniTask.CompletedTask;
    }
}
