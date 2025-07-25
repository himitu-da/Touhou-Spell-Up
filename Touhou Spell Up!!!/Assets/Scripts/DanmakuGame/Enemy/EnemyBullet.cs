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
        if (property != null)
        {
            this._lifeTime = property.LifeTime;
            _cancellationTokenSource = new CancellationTokenSource();

            Shooter shooter = null; // shooter変数を宣言
            if (property.ShootPattern != null)
            {
                shooter = gameObject.AddComponent<Shooter>(); // Shooterを動的に追加
                shooter.ParentShooter = parentShooter; // 親Shooterを設定
                // ShootPatternを実行。Bulletは不要なのでnullを渡す
                property.ShootPattern.Execute(null, shooter, _cancellationTokenSource.Token).Forget();
            }
            if (property.MovePattern != null)
            {
                var mover = gameObject.AddComponent<Mover>(); // Moverを動的に追加
                
                // shooterがnullの場合（ShootPatternがない場合）にShooterコンポーネントを追加
                if (shooter == null)
                {
                    shooter = gameObject.AddComponent<Shooter>();
                }
                
                shooter.ParentShooter = parentShooter; // 親Shooterを設定

                // MovePatternを実行。今度はshooterを渡す
                property.MovePattern.Execute(mover, shooter, _cancellationTokenSource.Token).Forget();
            }
            else
            {
                // MovePatternが指定されていない場合は、従来の直線移動をフォールバックとして実装
                gameObject.AddComponent<StraightMoveForBullet>().Initialize(property.Speed);
            }
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
