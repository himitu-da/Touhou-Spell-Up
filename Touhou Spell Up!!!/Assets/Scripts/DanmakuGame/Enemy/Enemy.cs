using UnityEngine;

[CreateAssetMenu(fileName = "ENM_", menuName = "Touhou Spell Up/Danmaku/Enemy", order = 3)]
public class Enemy : GameEntity
{
    // GameEntityの'property'フィールドをEnemyPropertyとしてキャストして公開する
    public EnemyProperty PropertyTyped => property as EnemyProperty;
}
