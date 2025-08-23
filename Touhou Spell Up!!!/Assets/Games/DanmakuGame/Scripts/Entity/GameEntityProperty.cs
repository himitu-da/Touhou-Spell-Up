using UnityEngine;

public class GameEntityProperty : ScriptableObject
{
    [SerializeField] private PatternBaseReference shootPattern;
    [SerializeField] private PatternBaseReference movePattern;
    [SerializeField] private PatternBaseReference animatePattern;

    public PatternBase ShootPattern => shootPattern.Value;
    public PatternBase MovePattern => movePattern.Value;
    public PatternBase AnimatePattern => animatePattern.Value;
}
