using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] float lifeTime = 4f;

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
        if (col.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);   // 敵を倒す
            Destroy(gameObject);       // 弾も消える
        }
    }
}
