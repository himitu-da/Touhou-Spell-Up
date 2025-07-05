using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float _speed = 5f;
    private float _lifeTime = 6f;

    public void Initialize(BulletProperty property)
    {
        if (property != null)
        {
            this._speed = property.Speed;
            this._lifeTime = property.LifeTime;
        }
    }

    void Update()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            Destroy(gameObject);   // 当たったら弾だけ消す（プレイヤー側で死亡判定）
    }
}
