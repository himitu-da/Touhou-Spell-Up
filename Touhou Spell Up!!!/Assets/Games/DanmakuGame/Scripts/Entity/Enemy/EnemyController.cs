using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EnemyController : GameEntityController
{
    [SerializeField] private PrefabReference lifeGaugePrefab;
    private GameObject _lifeGaugeInstance;

    // 現在HPを表すGameParameterの参照（オプション）
    [Header("GameParameter同期（オプション）")]
    [SerializeField] private IntGameParameter currentHealthParameter;

    public UnityAction<float, float> OnHealthChanged;
    
    // EnemyState にキャストして CurrentHealth にアクセス
    public float CurrentHealth 
    { 
        get => (_state as EnemyState)?.CurrentHealth ?? 0f;
        private set 
        {
            if (_state is EnemyState enemyState)
            {
                enemyState.CurrentHealth = value;
            }
        }
    }
    
    public float HealthPercentage => CurrentHealth / (Property as EnemyProperty)?.MaxHealth ?? 1f;

    void Start()
    {
        // base.Start()を呼び出して制御された初期化を実行
        InitializeOnStart();
    }

    public override void Initialize(GameEntity entity)
    {
        // 依存システムが準備完了していることを確認
        if (!GameParameterManager.IsInitialized)
        {
            Debug.LogError("EnemyController initialized before GameParameterManager!", this);
        }

        base.Initialize(entity);
        
        // EnemyController固有の初期化
        // CurrentHealth を EnemyProperty.MaxHealth で初期化
        if (Property is EnemyProperty enemyProperty)
        {
            CurrentHealth = enemyProperty.MaxHealth;
        }
        
        SetupHealthSystem();
        SetupLifeGauge();
    }

    /// <summary>
    /// EnemyState を作成
    /// </summary>
    protected override GameEntityState CreateState()
    {
        return new EnemyState();
    }

    /// <summary>
    /// ヘルスシステムの初期化
    /// </summary>
    private void SetupHealthSystem()
    {
        // 親クラスのCurrentHealthはInitializeでMaxHealthに設定されるため、
        // ここではGameParameterの同期のみ行う
        if (currentHealthParameter != null)
        {
            currentHealthParameter.Value = (int)CurrentHealth;
        }
    }

    /// <summary>
    /// ライフゲージの初期化
    /// </summary>
    private void SetupLifeGauge()
    {
        if (lifeGaugePrefab != null && lifeGaugePrefab.Value != null)
        {
            _lifeGaugeInstance = Instantiate(lifeGaugePrefab.Value, transform.position, Quaternion.identity, transform);
            
            var canvas = _lifeGaugeInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
            }
            
            var lifeGaugeController = _lifeGaugeInstance.GetComponent<EnemyLifeGaugeController>();
            if (lifeGaugeController != null)
            {
                lifeGaugeController.Initialize(this);
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // GameParameterが設定されていれば同期
        if (currentHealthParameter != null)
        {
            currentHealthParameter.Value = (int)CurrentHealth;
        }

        float maxHealth = (Property as EnemyProperty)?.MaxHealth ?? 1f;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
