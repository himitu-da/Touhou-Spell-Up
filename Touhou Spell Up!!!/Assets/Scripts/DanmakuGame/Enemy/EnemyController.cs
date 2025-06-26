using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    //[SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireInterval = 1.0f;
    [SerializeField] GameObject lifeGaugePrefab;

    [Header("攻撃パターン")]
    [SerializeField] BulletPatternBase attackPattern;

    private Health _health;
    private GameObject _lifeGaugeInstance;

    float _timer;

    void Start()
    {
        _health = GetComponent<Health>();
        if (lifeGaugePrefab != null)
        {
            _lifeGaugeInstance = Instantiate(lifeGaugePrefab, transform.position, Quaternion.identity, transform);
            
            // 生成したインスタンスからCanvasコンポーネントを取得
            var canvas = _lifeGaugeInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Render ModeをWorld Spaceに設定
                canvas.renderMode = RenderMode.WorldSpace;
                // メインカメラをEvent Cameraに設定
                canvas.worldCamera = Camera.main;
            }

            var lifeGaugeController = _lifeGaugeInstance.GetComponent<EnemyLifeGaugeController>();
            if (lifeGaugeController != null)
            {
                lifeGaugeController.Initialize(_health);
            }
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= fireInterval)
        {
            _timer = 0f;
            if (attackPattern != null)
            {
                attackPattern.Execute(transform, this.GetCancellationTokenOnDestroy()).Forget();
            }
        }
    }
}
