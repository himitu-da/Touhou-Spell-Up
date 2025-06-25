using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireInterval = 1.0f;

    float _timer;
    float _delta = 0.0f;

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

    // 自機弾に当たったら PlayerShot.cs 側で Destroy される
}
