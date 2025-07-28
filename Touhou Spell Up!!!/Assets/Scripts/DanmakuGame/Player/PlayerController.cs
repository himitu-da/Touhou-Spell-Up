using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : GameEntityController
{
    [SerializeField] GameObject hitboxMarker;

    private PlayerProperty _playerProperty;
    private float _shotTimer;
    private Vector2 _moveInput;
    private bool _isShotPressed, _isSlowMovePressed;

    void Awake()
    {
        Initialize(_entity);
    }

    public override void Initialize(GameEntity entity)
    {
        base.Initialize(entity);
        _playerProperty = entity.Property as PlayerProperty;
        if (_playerProperty == null)
        {
            Debug.LogError("PlayerProperty is not set.", this);
            enabled = false;
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
        float currentSpeed = _isSlowMovePressed ? _playerProperty.MoveSpeedSlow : _playerProperty.MoveSpeed;
        Vector2 delta = _moveInput * currentSpeed * Time.deltaTime;

        // 範囲外に出る場合はクランプする
        Vector2 nextPosition = (Vector2)transform.position + delta;
        var movableArea = _playerProperty.MovableArea;
        nextPosition.x = Mathf.Clamp(nextPosition.x, movableArea.xMin, movableArea.xMax);
        nextPosition.y = Mathf.Clamp(nextPosition.y, movableArea.yMin, movableArea.yMax);

        transform.position = nextPosition;

        // -------- ショット --------
        _shotTimer += Time.deltaTime;
        if (_isShotPressed && _shotTimer >= _playerProperty.ShotInterval)
        {
            _shotTimer = 0f;
            var shotPattern = _isSlowMovePressed ? _playerProperty.ShotPatternSlow : _playerProperty.ShotPatternNormal;
            if (shotPattern != null)
            {
                shotPattern.Execute(this, _cancellationTokenSource.Token).Forget();
            }
        }

        hitboxMarker.SetActive(_isSlowMovePressed);
    }

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
