using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Touhou-Spell-Up/Player", order = 1)]
public class Player : GameEntity
{
    public PlayerProperty PlayerProperty => property as PlayerProperty;
    public override GameEntityProperty Property => PlayerProperty;
}
