using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 6f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
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
