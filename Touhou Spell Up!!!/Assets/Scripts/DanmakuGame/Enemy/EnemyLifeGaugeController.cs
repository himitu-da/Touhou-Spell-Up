using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeGaugeController : MonoBehaviour
{
    [SerializeField] private Image _lifeGaugeImage; // 体力ゲージのUIイメージ
    private Health _enemyHealth; // 敵の体力コンポーネント

    
    public void Initialize(Health enemyHealth)
    {
        _enemyHealth = enemyHealth;
        _enemyHealth.OnHealthChanged += UpdateLifeGauge; // 体力が変化したときにゲージを更新
        UpdateLifeGauge(_enemyHealth.HealthPercentage * _enemyHealth.MaxHealth, _enemyHealth.MaxHealth); // 初期状態のゲージを更新
    }

    private void OnDestroy()
    {
        if (_enemyHealth != null)
        {
            _enemyHealth.OnHealthChanged -= UpdateLifeGauge; // イベントの登録を解除
        }
    }

    private void UpdateLifeGauge(float currentHealth, float maxHealth)
    {
        if (_lifeGaugeImage != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            _lifeGaugeImage.fillAmount = healthPercentage; // ゲージの表示を更新
        }
    }
}
