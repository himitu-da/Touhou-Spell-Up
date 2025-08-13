using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class GameEntityController : MonoBehaviour, IMovable, IShootable
{
    [SerializeField] protected GameEntityReference _entity;
    protected MovementState _movementState;
    public MovementState MovementState => _movementState; // SatelliteMovePatternから参照するためにpublicにする
    public GameEntityProperty Property { get; protected set; }
    protected CancellationTokenSource _cancellationTokenSource;

    // 描画補間用
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    [SerializeField] private FloatReference _interpolationSpeed = new FloatReference { useConstant = true, constantValue = 15f };

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

        // GameEntityReferenceに定数として設定する
        this._entity = new GameEntityReference { useConstant = true, constantValue = entity };
        this.Property = entity.Property;
        
        // MovementStateをnewで生成する
        _movementState = new MovementState
        {
            // 初期位置と向きを設定
            Position = transform.position,
            Rotation = transform.rotation
        };

        // 補間用の目標値も初期化
        _targetPosition = _movementState.Position;
        _targetRotation = _movementState.Rotation;

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
                ParentActor.InstantiateProperty(ParentActor._entity.Value, position, rotation);
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

    protected virtual void FixedUpdate()
    {
        if (_movementState == null) return;

        // 速度に基づいて論理位置を更新
        _movementState.Position += _movementState.Velocity * Time.fixedDeltaTime;

        // 補間目標値を更新
        _targetPosition = _movementState.Position;
        _targetRotation = _movementState.Rotation;
    }

    protected virtual void Update()
    {
        if (_movementState == null) return;

        // 描画位置を目標位置に滑らかに補間
        transform.position = Vector3.Lerp(transform.position, _targetPosition, _interpolationSpeed.Value * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _interpolationSpeed.Value * Time.deltaTime);
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
