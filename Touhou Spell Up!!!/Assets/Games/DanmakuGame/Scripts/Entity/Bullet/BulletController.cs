using UnityEngine;

public class BulletController : GameEntityController
{
    private float _lifeTime = 6f;
    private float _attackPower = 0f;
    private bool _isPlayerShot = false;

    public void Initialize(GameEntity entity, GameEntityController parentController)
    {
        this.ParentActor = parentController;
        base.Initialize(entity);

        if (Property is BulletProperty bulletProperty)
        {
            _lifeTime = bulletProperty.LifeTime;
            _attackPower = bulletProperty.AttackPower;
        }

        if (parentController is PlayerController)
        {
            _isPlayerShot = true;
        }
    }

    protected override void Update()
    {
        base.Update(); // 基底クラスのUpdateを呼び出して移動処理を行う

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    new void OnTriggerEnter2D(Collider2D col)
    {
        if (_isPlayerShot)
        {
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_attackPower);
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (col.CompareTag("Player"))
            {
                Destroy(gameObject); // 当たったら弾だけ消す（プレイヤー側で死亡判定）
            }
        }
    }
}
