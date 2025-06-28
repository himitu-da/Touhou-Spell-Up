using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BasicShotPattern", menuName = "Touhou Spell Up/Bullet Pattern/Basic Shot")]
public class BasicShotPattern : BulletPatternBase
{
    [SerializeField]
    private GameObject bulletPrefab;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefabが設定されていません。", this);
            return;
        }

        if (token.IsCancellationRequested) return;

        // spawnPointの位置と角度で、指定された弾を1つ生成する
        Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

        await UniTask.CompletedTask;
    }
}
