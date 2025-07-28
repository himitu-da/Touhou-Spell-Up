using UnityEngine;

public class GameEntityProperty : ScriptableObject
{
    [SerializeField] private PatternBase shootPattern;
    [SerializeField] private PatternBase movePattern;

    public PatternBase ShootPattern => shootPattern;
    public PatternBase MovePattern => movePattern;
}
