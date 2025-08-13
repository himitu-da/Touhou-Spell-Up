using UnityEngine;

[CreateAssetMenu(fileName = "PLY_", menuName = "Danmaku/Entity/Player/Player", order = 1)]
public class Player : GameEntity
{
    public PlayerProperty PlayerProperty => property.Value as PlayerProperty;
    public override GameEntityProperty Property => PlayerProperty;
}
