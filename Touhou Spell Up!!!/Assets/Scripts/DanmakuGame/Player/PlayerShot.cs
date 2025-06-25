using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] float lifeTime = 4f;
    [SerializeField] float shotDamage = 1f;

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.World);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Health health = col.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(shotDamage); // 敵のHPを減らす
            Destroy(gameObject);             // 弾は消える
            return;
        }
    }
}
