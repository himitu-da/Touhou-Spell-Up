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

    // Health Properties
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public UnityAction<float, float> OnHealthChanged;
    public float HealthPercentage => CurrentHealth / MaxHealth;

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
        SetupHealthSystem();
        SetupLifeGauge();
    }

    /// <summary>
    /// ヘルスシステムの初期化
    /// </summary>
    private void SetupHealthSystem()
    {
        if (Property is EnemyProperty enemyProperty)
        {
            MaxHealth = enemyProperty.MaxHealth;
        }
        CurrentHealth = MaxHealth;

        // GameParameterが設定されていれば初期値を同期
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

    public void TakeDamage(float damage)
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

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

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
