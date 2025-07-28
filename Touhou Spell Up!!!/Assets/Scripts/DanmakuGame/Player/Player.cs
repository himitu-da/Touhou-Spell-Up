using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Touhou-Spell-Up/Player", order = 1)]
public class Player : ScriptableObject
{
    [SerializeField] private PlayerProperty playerProperty;
    public PlayerProperty PlayerProperty => playerProperty;
}
