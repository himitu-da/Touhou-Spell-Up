using UnityEngine;

public class GameEntityProperty : ScriptableObject
{
    [SerializeField] private PatternBaseReference shootPattern;
    [SerializeField] private PatternBaseReference movePattern;

    public PatternBase ShootPattern => shootPattern.Value;
    public PatternBase MovePattern => movePattern.Value;
}
