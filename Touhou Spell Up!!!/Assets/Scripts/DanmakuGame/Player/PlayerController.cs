using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] GameObject shotPrefab;
    [SerializeField] GameObject hitboxMarker;

    public static PlayerController Instance { get; private set; }
    float _shotTimer;
    Vector2 _moveInput;
    bool _isShotPressed, _isSlowMovePressed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnShot(InputValue value)
    {
        _isShotPressed = value.isPressed;
    }

    public void OnSlowMove(InputValue value)
    {
        _isSlowMovePressed = value.isPressed;
    }

    void Update()
    {
        // -------- 移動 --------
        float currentSpeed = _isSlowMovePressed ? player.PlayerProperty.MoveSpeedSlow : player.PlayerProperty.MoveSpeed;
        Vector2 delta = _moveInput * currentSpeed * Time.deltaTime;

        // 範囲外に出る場合はクランプする
        Vector2 nextPosition = (Vector2)transform.position + delta;
        var movableArea = player.PlayerProperty.MovableArea;
        nextPosition.x = Mathf.Clamp(nextPosition.x, movableArea.xMin, movableArea.xMax);
        nextPosition.y = Mathf.Clamp(nextPosition.y, movableArea.yMin, movableArea.yMax);

        transform.position = nextPosition;

        // -------- ショット --------
        _shotTimer += Time.deltaTime;
        if (_isShotPressed && _shotTimer >= player.PlayerProperty.ShotInterval)
        {
            _shotTimer = 0f;
            Instantiate(shotPrefab, transform.position, Quaternion.identity);
        }

        hitboxMarker.SetActive(_isSlowMovePressed);
    }

    // Hitboxから呼ばれる
    public void OnHit()
    {
        DanmakuGameManager.Instance.GameOver();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 敵弾に当たったらゲームオーバー
        if (col.CompareTag("EnemyBullet"))
        {
            OnHit();
        }
    }
}
