using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BASIC_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Basic Shot")]
public class BasicShotPattern : ShootPatternBase
{
    public override async UniTask ExecuteImpl(Transform spawnPoint, Bullet bulletToUse, CancellationToken token)
    {
        if (bulletToUse == null || bulletToUse.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        // spawnPointの位置と角度で、指定された弾を1つ生成する
        var bulletInstance = Instantiate(bulletToUse.Prefab, spawnPoint.position, spawnPoint.rotation);
        var enemyBullet = bulletInstance.GetComponent<EnemyBullet>();
        if (enemyBullet != null)
        {
            enemyBullet.Initialize(bulletToUse.Property);
        }

        await UniTask.CompletedTask;
    }
}
