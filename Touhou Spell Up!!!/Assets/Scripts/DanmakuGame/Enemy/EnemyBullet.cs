using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EnemyBullet : MonoBehaviour
{
    public BulletProperty Property { get; private set; }
    private float _lifeTime = 6f;
    private CancellationTokenSource _cancellationTokenSource;

    public void Initialize(BulletProperty property, Shooter parentShooter)
    {
        this.Property = property;
        if (property == null) return;

        this._lifeTime = property.LifeTime;
        _cancellationTokenSource = new CancellationTokenSource();

        Shooter shooter = null;
        Mover mover = null;

        // 先にMoverを準備
        if (property.MovePattern != null)
        {
            mover = gameObject.AddComponent<Mover>();
        }

        // ShootPatternがある場合
        if (property.ShootPattern != null)
        {
            shooter = gameObject.AddComponent<Shooter>();
            shooter.ParentShooter = parentShooter;

            // Moverが存在すればそれを、なければ弾自身をIMovableとして渡す
            // ただし、弾自身はIMovableを実装していないため、Moverを渡すのが正しい
            IMovable bulletMover = mover ?? gameObject.AddComponent<Mover>();
            property.ShootPattern.Execute(bulletMover, shooter, _cancellationTokenSource.Token).Forget();
        }

        // MovePatternがある場合
        if (property.MovePattern != null)
        {
            // shooterがまだなければ追加
            if (shooter == null)
            {
                shooter = gameObject.AddComponent<Shooter>();
                shooter.ParentShooter = parentShooter;
            }
            // 既に準備済みのmoverとshooterを渡す
            property.MovePattern.Execute(mover, shooter, _cancellationTokenSource.Token).Forget();
        }
        else
        {
            // MovePatternがない場合のフォールバック
            gameObject.AddComponent<StraightMoveForBullet>().Initialize(property.Speed);
        }
    }

    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            Destroy(gameObject);   // 当たったら弾だけ消す（プレイヤー側で死亡判定）
    }

    void OnDestroy()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}

// MovePatternがない場合のフォールバック用コンポーネント
public class StraightMoveForBullet : MonoBehaviour
{
    private float _speed;
    public void Initialize(float speed) => _speed = speed;
    void Update() => transform.Translate(Vector2.up * _speed * Time.deltaTime);
}
