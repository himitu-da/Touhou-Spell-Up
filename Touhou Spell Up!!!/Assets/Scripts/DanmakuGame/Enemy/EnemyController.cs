using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireInterval = 1.0f;
    [SerializeField] GameObject lifeGaugePrefab;

    private Health _health;
    private GameObject _lifeGaugeInstance;

    float _timer;
    float _delta = 0.0f;

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
            // NWayBullet(5, 90, UnityEngine.Random.Range(0.0f, 360.0f));
            NWayBullet(7, 120, _delta);
            _delta += 27.7f;
        }

        if (_delta >= 360.0f)
            _delta -= 360.0f;
    }

    void NWayBullet(int wayCount, float angle, float delta = 0.0f)
    {
        float startAngle = -angle / 2 + delta;
        float angleStep = angle / (wayCount - 1);

        for (int i = 0; i < wayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Instantiate(bulletPrefab, transform.position, rotation);
        }
    }
}
