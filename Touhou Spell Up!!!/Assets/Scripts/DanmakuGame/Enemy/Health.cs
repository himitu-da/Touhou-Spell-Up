using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    // maxHealthを外部から読み取れるようにpublicプロパティを追加
    public float MaxHealth => maxHealth;

    // 体力が変化したときに呼び出されるイベント
    public UnityAction<float, float> OnHealthChanged;

    // 現在の体力の割合を返すプロパティ
    public float HealthPercentage
    {
        get { return currentHealth / maxHealth; }
    }

    void Awake()
    {
        currentHealth = maxHealth; // 初期化
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0; // 体力が0未満にならないようにする
        }

        // 体力が変化したときのイベントを呼び出す
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 死亡時の処理
        Destroy(gameObject); // オブジェクトを削除
    }
}
