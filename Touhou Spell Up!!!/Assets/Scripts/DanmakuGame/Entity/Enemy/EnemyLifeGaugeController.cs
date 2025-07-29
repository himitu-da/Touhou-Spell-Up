using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeGaugeController : MonoBehaviour
{
    [SerializeField] private Image _lifeGaugeImage; // 体力ゲージのUIイメージ
    private EnemyController _enemyController; // 敵のコントローラーコンポーネント

    
    public void Initialize(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.OnHealthChanged += UpdateLifeGauge; // 体力が変化したときにゲージを更新
        UpdateLifeGauge(_enemyController.CurrentHealth, _enemyController.MaxHealth); // 初期状態のゲージを更新
    }

    private void OnDestroy()
    {
        if (_enemyController != null)
        {
            _enemyController.OnHealthChanged -= UpdateLifeGauge; // イベントの登録を解除
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
