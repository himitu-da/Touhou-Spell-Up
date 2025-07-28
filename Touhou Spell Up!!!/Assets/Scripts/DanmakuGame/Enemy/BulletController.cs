using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BulletController : EntityController
{
    private float _lifeTime = 6f;

    public void Initialize(GameEntity entity, EntityController parentController)
    {
        this.ParentActor = parentController;
        base.Initialize(entity);
        if (Property is BulletProperty bulletProperty)
        {
            _lifeTime = bulletProperty.LifeTime;
        }
    }

    void Update()
    {
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
