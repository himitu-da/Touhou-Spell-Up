using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class GameEntityController : MonoBehaviour, IMovable, IShootable
{
    [SerializeField] protected GameEntity _entity;
    protected MovementState _movementState;
    public MovementState MovementState => _movementState; // SatelliteMovePatternから参照するためにpublicにする
    public GameEntityProperty Property { get; protected set; }
    protected CancellationTokenSource _cancellationTokenSource;

    private GameEntityController _parentActor;
    public GameEntityController ParentActor
    {
        get => _parentActor;
        set => _parentActor = value;
    }

    // GameEntity を受け取るように変更
    public virtual void Initialize(GameEntity entity)
    {
        if (entity == null) return;

        this._entity = entity;
        this.Property = entity.Property;
        
        // MovementStateをnewで生成する
        _movementState = new MovementState
        {
            // 初期位置と向きを設定
            Position = transform.position,
            Rotation = transform.rotation
        };

        if (this.Property == null) return;

        _cancellationTokenSource = new CancellationTokenSource();

        // パターンを実行
        if (Property.MovePattern != null)
        {
            // SatelliteMovePatternは親Actorの位置を必要とする特殊なケースなため、
            // GameEntityControllerを渡して実行する
            if (Property.MovePattern is SatelliteMovePattern)
            {
                Property.MovePattern.Execute(this, _cancellationTokenSource.Token).Forget();
            }
            else
            {
                Property.MovePattern.Execute(_movementState, _cancellationTokenSource.Token).Forget();
            }
        }
        if (Property.ShootPattern != null)
        {
            Property.ShootPattern.Execute(this, _cancellationTokenSource.Token).Forget();
        }
    }

    // IShootableインターフェースの実装
    public void InstantiateProperty(GameEntity entity, Vector3 position, Quaternion rotation)
    {
        // 渡されたEntityアセットや、その中のPrefabがnullならエラーを防ぐ
        if (entity == null || entity.Prefab == null)
        {
            // 親から借りてくる
            if (ParentActor != null)
            {
                ParentActor.InstantiateProperty(ParentActor._entity, position, rotation);
                return;
            }
            Debug.LogError("InstantiateProperty was called with a null entity or prefab, and no parent actor to borrow from.", this);
            return;
        }

        // entity.Prefab (GameObject) をInstantiateする
        var instance = Instantiate(entity.Prefab, position, rotation);

        // 生成したインスタンスからBulletControllerコンポーネントを取得して初期化
        var bulletController = instance.GetComponent<BulletController>();
        if (bulletController != null)
        {
            // entity.Property (GameEntityProperty) と親Actor（自身）を渡して初期化
            bulletController.Initialize(entity, this);
        }
    }

    protected virtual void Update()
    {
        if (_movementState == null) return;

        // 速度に基づいて位置を更新
        _movementState.Position += _movementState.Velocity * Time.deltaTime;

        // MovementStateをtransformに反映
        transform.position = _movementState.Position;
        transform.rotation = _movementState.Rotation;
    }

    protected virtual void OnDestroy()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
