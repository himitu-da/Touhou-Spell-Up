using UnityEngine;

[CreateAssetMenu(fileName = "ENM_", menuName = "Danmaku/Entity/Enemy/Enemy", order = 3)]
public class Enemy : GameEntity
{
    // GameEntityの'property'フィールドをEnemyPropertyとしてキャストして公開する
    public EnemyProperty PropertyTyped => property.Value as EnemyProperty;
}
