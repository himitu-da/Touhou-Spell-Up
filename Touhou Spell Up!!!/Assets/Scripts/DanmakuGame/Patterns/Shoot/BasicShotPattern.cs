using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BASIC_", menuName = "Touhou Spell Up/Danmaku/Shoot/Basic Shot")]
public class BasicShotPattern : ShootPatternBase
{
    public override async UniTask ExecuteShoot(Shooter shooter, CancellationToken token)
    {
        if (_bullet == null || _bullet.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }
        Vector3 baseSpawnPosition = GetSpawnPosition(shooter);
        float angle = GetAimAngle(shooter, baseSpawnPosition) + directionOffset;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        // 最終的な発射位置を計算
        Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, angle);

        // spawnPointの位置と角度で、指定された弾を1つ生成する
        shooter.InstantiateBullet(_bullet, finalSpawnPosition, spawnRotation);

        await UniTask.CompletedTask;
    }
}
