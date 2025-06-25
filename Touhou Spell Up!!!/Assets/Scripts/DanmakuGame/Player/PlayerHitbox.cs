using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHitbox : MonoBehaviour
{
    PlayerController _playerController;

    void Start()
    {
        // 親オブジェクトのPlayerControllerを取得
        _playerController = GetComponentInParent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController not found on parent object.");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 敵弾に当たったらPlayerControllerに通知
        if (col.CompareTag("EnemyBullet"))
        {
            _playerController.OnHit();
        }
    }
}
