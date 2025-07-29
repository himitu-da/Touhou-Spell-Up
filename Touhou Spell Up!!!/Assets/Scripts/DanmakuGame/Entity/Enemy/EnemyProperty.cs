using UnityEngine;

[CreateAssetMenu(fileName = "ENMP_", menuName = "Danmaku/Entity/Enemy/EnemyProperty", order = 2)]
public class EnemyProperty : GameEntityProperty
{
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;
}
