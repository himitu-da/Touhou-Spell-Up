using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EnemyController : GameEntityController
{
    [SerializeField] private PrefabReference lifeGaugePrefab;
    private GameObject _lifeGaugeInstance;

    // Health Properties
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public UnityAction<float, float> OnHealthChanged;
    public float HealthPercentage => CurrentHealth / MaxHealth;

    void Start()
    {
        Initialize(_entity.Value);

        if (Property is EnemyProperty enemyProperty)
        {
            MaxHealth = enemyProperty.MaxHealth;
        }
        CurrentHealth = MaxHealth;

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
