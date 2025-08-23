using UnityEngine;

public class BulletController : GameEntityController
{
    void Start()
    {
        // Debug.Log($"BulletController.Start: {gameObject.name} created");
    }

    public void Initialize(GameEntity entity, GameEntityController parentController)
    {
        // Debug.Log($"BulletController.Initialize called: entity={entity}, parentController={parentController}");
        
        this.ParentActor = parentController;
        base.Initialize(entity);

        // Debug.Log($"BulletController.Initialize: _state type = {_state?.GetType().Name}, Property type = {Property?.GetType().Name}");

        // BulletState の初期化
        if (_state is BulletState bulletState && Property is BulletProperty bulletProperty)
        {
            bulletState.RemainingLifeTime = bulletProperty.InitialLifeTime;
            bulletState.AttackPower = bulletProperty.AttackPower;
            bulletState.IsPlayerShot = (parentController is PlayerController);
            
            // Debug.Log($"BulletController initialized successfully: LifeTime={bulletState.RemainingLifeTime}, AttackPower={bulletState.AttackPower}, IsPlayerShot={bulletState.IsPlayerShot}");
        }
        else
        {
            Debug.LogError($"BulletController initialization failed: _state is BulletState={_state is BulletState}, Property is BulletProperty={Property is BulletProperty}");
        }
    }

    /// <summary>
    /// BulletState を作成
    /// </summary>
    protected override GameEntityState CreateState()
    {
        return new BulletState();
    }

    protected override void Update()
    {
        base.Update(); // 基底クラスのUpdateを呼び出して移動処理を行う

        if (_state is BulletState bulletState)
        {
            bulletState.RemainingLifeTime -= Time.deltaTime;
            if (bulletState.RemainingLifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    new void OnTriggerEnter2D(Collider2D col)
    {
        // Debug.Log($"BulletController.OnTriggerEnter2D: Hit {col.name} with tag {col.tag}");
        
        if (_state is BulletState bulletState)
        {
            // Debug.Log($"BulletState found: IsPlayerShot={bulletState.IsPlayerShot}, AttackPower={bulletState.AttackPower}");
            
            if (bulletState.IsPlayerShot)
            {
                if (col.CompareTag("Enemy"))
                {
                    // Debug.Log("Player bullet hit enemy");
                    var enemy = col.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        // Debug.Log($"Dealing {bulletState.AttackPower} damage to enemy");
                        enemy.TakeDamage(bulletState.AttackPower);
                    }
                    else
                    {
                        Debug.LogError("EnemyController not found on enemy object");
                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Enemy bullet hit player");
                    Destroy(gameObject); // 当たったら弾だけ消す（プレイヤー側で死亡判定）
                }
            }
        }
        else
        {
            Debug.LogError($"BulletState not found: _state type = {_state?.GetType().Name}");
        }
    }
}
