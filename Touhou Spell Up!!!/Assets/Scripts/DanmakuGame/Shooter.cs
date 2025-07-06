using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    private ShootPatternBase _shootPattern;

    [SerializeField]
    private Bullet _bullet; // このShooterが使用する基本の弾

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        // _shootPatternがアタッチされていなければ何もしない
        if (_shootPattern == null)
        {
            // Debug.LogWarning("ShootPattern is not assigned.", this);
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        // 自身の情報を渡してShootPatternを実行
        _shootPattern.Execute(this, _bullet, _cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    // 弾を生成するメソッド
    public void InstantiateBullet(Bullet bullet, Vector3 position, Quaternion rotation)
    {
        // 渡されたBulletアセットや、その中のPrefabがnullならエラーを防ぐ
        if (bullet == null || bullet.Prefab == null)
        {
            Debug.LogError("InstantiateBullet was called with a null bullet or prefab.", this);
            return;
        }

        // bullet.Prefab (GameObject) をInstantiateする
        var bulletInstance = Instantiate(bullet.Prefab, position, rotation);

        // 生成したインスタンスからEnemyBulletコンポーネントを取得して初期化
        var enemyBullet = bulletInstance.GetComponent<EnemyBullet>();
        if (enemyBullet != null)
        {
            // bullet.Property (BulletProperty) を渡して初期化
            enemyBullet.Initialize(bullet.Property);
        }
    }
}
