using UnityEngine;

[CreateAssetMenu(fileName = "PLY_", menuName = "Danmaku/Entity/Player/Player", order = 1)]
public class Player : GameEntity
{
    public PlayerProperty PlayerProperty => property as PlayerProperty;
    public override GameEntityProperty Property => PlayerProperty;
}
