using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] GameObject shotPrefab;
    [SerializeField] float shotInterval = 0.15f;

    float _shotTimer;
    Vector2 _moveInput;
    bool _isShotPressed;

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnShot(InputValue value)
    {
        _isShotPressed = value.isPressed;
    }

    void Update()
    {
        // -------- 移動 --------
        Vector2 delta = _moveInput * moveSpeed * Time.deltaTime;
        transform.Translate(delta, Space.World);

        // -------- ショット --------
        _shotTimer += Time.deltaTime;
        if (_isShotPressed && _shotTimer >= shotInterval)
        {
            _shotTimer = 0f;
            Instantiate(shotPrefab, transform.position, Quaternion.identity);
        }
    }

    // Hitboxから呼ばれる
    public void OnHit()
    {
        Destroy(gameObject);
    }
}
