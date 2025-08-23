using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class GameEntityController : MonoBehaviour, IMovable, IShootable
{
    [SerializeField] protected GameEntityReference _entity;
    protected GameEntityState _state;
    public GameEntityState State => _state;
    public GameEntityProperty Property { get; protected set; }
    protected CancellationTokenSource _cancellationTokenSource;

    [SerializeField] private List<GameParameter> _gameParameters = new List<GameParameter>();
    public List<GameParameter> GameParameters => _gameParameters;

    // イベント駆動のメンバ
    private float _currentLifetime = 0f;
    public float CurrentLifeTime => _currentLifetime;
    
    public bool IsCollided { get; private set; } = false;
    public Collider2D LastCollider { get; private set; } = null;

    // 初期化管理
    protected bool _isInitialized = false;
    public bool IsInitialized => _isInitialized;
    
    // イベント定義
    [System.Serializable]
    public class LifetimeChangedEvent : UnityEvent<float> { }
    
    [System.Serializable]
    public class CollisionDetectedEvent : UnityEvent<Collider2D> { }
    
    [System.Serializable]
    public class EntityDestroyedEvent : UnityEvent<GameEntityController> { }
    
    public LifetimeChangedEvent OnLifetimeChanged = new LifetimeChangedEvent();
    public CollisionDetectedEvent OnCollisionDetected = new CollisionDetectedEvent();
    public EntityDestroyedEvent OnEntityDestroyed = new EntityDestroyedEvent();

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

    void Start()
    {
        if (CanInitialize())
        {
            PerformInitialization();
        }
        else
        {
            StartCoroutine(WaitForSystemReady());
        }
    }

    /// <summary>
    /// サブクラスから呼び出し可能なStart処理
    /// </summary>
    protected virtual void InitializeOnStart()
    {
        Start();
    }

    /// <summary>
    /// 初期化可能かどうかをチェック
    /// </summary>
    private bool CanInitialize()
    {
        return GameParameterManager.IsInitialized && 
               DanmakuGameManager.Instance != null && 
               DanmakuGameManager.Instance.IsSystemReady;
    }

    /// <summary>
    /// システム準備完了まで待機
    /// </summary>
    private IEnumerator WaitForSystemReady()
    {
        yield return new WaitUntil(CanInitialize);
        PerformInitialization();
    }

    /// <summary>
    /// 実際の初期化処理を実行
    /// </summary>
    private void PerformInitialization()
    {
        if (!_isInitialized && _entity.Value != null)
        {
            Initialize(_entity.Value);
            _isInitialized = true;
        }
    }

    /// <summary>
    /// 適切な State オブジェクトを作成する（サブクラスでオーバーライド可能）
    /// </summary>
    protected virtual GameEntityState CreateState()
    {
        return new GameEntityState();
    }

    // GameEntity を受け取るように変更
    public virtual void Initialize(GameEntity entity)
    {
        // 依存システムの初期化完了をチェック
        if (!GameParameterManager.IsInitialized)
        {
            Debug.LogError($"{GetType().Name} initialized before GameParameterManager! This may cause issues with Pattern execution.", this);
        }

        if (entity == null) return;

        // GameEntityReferenceに定数として設定する
        this._entity = new GameEntityReference { useConstant = true, constantValue = entity };
        this.Property = entity.Property;

        // GameEntityStateをnewで生成する
        _state = CreateState();
        
        // 初期位置と向きを設定
        _state.Position = transform.position;
        _state.Rotation = transform.rotation;
        _state.InitialScale = transform.localScale; // 初期スケールをstateに保存
        _state.ScaleMultiplier = Vector3.one; // stateのScaleMultiplierは乗数として扱うため1で初期化

        // 補間用の目標値も初期化
        _targetPosition = _state.Position;
        _targetRotation = _state.Rotation;

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
                Property.MovePattern.Execute(_state, _cancellationTokenSource.Token).Forget();
            }
        }
        if (Property.ShootPattern != null)
        {
            Property.ShootPattern.Execute(this, _cancellationTokenSource.Token).Forget();
        }
        if (Property.AnimatePattern != null)
        {
            Property.AnimatePattern.Execute(this, _cancellationTokenSource.Token).Forget();
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
        else
        {
            Debug.LogError($"InstantiateProperty: BulletController not found on {instance.name}");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_state == null) return;

        // 速度に基づいて論理位置を更新
        _state.Position += _state.Velocity * Time.fixedDeltaTime;

        // 補間目標値を更新
        _targetPosition = _state.Position;
        _targetRotation = _state.Rotation;
    }

    protected virtual void Update()
    {
        if (_state == null) return;

        // 生存時間を更新
        float previousLifetime = _currentLifetime;
        _currentLifetime += Time.deltaTime;
        
        // 生存時間変更イベントを発火
        if (previousLifetime != _currentLifetime)
        {
            OnLifetimeChanged?.Invoke(_currentLifetime);
        }
        
        if (_entity.Value != null && _entity.Value.Lifetime != -1 && _currentLifetime > _entity.Value.Lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 描画位置を目標位置に滑らかに補間
        transform.position = Vector3.Lerp(transform.position, _targetPosition, _interpolationSpeed.Value * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _interpolationSpeed.Value * Time.deltaTime);
        transform.localScale = Vector3.Scale(_state.InitialScale, _state.ScaleMultiplier);
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        IsCollided = true;
        LastCollider = other.collider;
        OnCollisionDetected?.Invoke(other.collider);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        IsCollided = true;
        LastCollider = other;
        OnCollisionDetected?.Invoke(other);
    }

    protected virtual void OnDestroy()
    {
        // エンティティ破棄イベントを発火
        OnEntityDestroyed?.Invoke(this);
        
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
