using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BASIC_", menuName = "Touhou Spell Up/Danmaku/Shoot/Basic Shot")]
public class BasicShotPattern : ShootPatternBase
{
    public override async UniTask ExecuteShoot(Shooter shooter, Bullet bulletToUse, CancellationToken token)
    {
        if (bulletToUse == null || bulletToUse.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        Vector3 spawnPosition = GetSpawnPosition(shooter);
        Quaternion spawnRotation = shooter.transform.rotation * Quaternion.Euler(0, 0, directionOffset);

        // spawnPointの位置と角度で、指定された弾を1つ生成する
        shooter.InstantiateBullet(bulletToUse, spawnPosition, spawnRotation);

        await UniTask.CompletedTask;
    }
}
