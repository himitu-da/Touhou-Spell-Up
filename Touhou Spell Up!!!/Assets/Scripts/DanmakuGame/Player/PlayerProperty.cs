using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperty", menuName = "Touhou-Spell-Up/PlayerProperty", order = 0)]
public class PlayerProperty : GameEntityProperty
{
    [Header("移動")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float moveSpeedSlow = 6f;
    [SerializeField] Rect movableArea;

    [Header("ショット")]
    [SerializeField] float shotInterval = 0.15f;
    [SerializeField] private PatternBase shotPatternNormal;
    [SerializeField] private PatternBase shotPatternSlow;


    public float MoveSpeed => moveSpeed;
    public float MoveSpeedSlow => moveSpeedSlow;
    public Rect MovableArea => movableArea;
    public float ShotInterval => shotInterval;
    public PatternBase ShotPatternNormal => shotPatternNormal;
    public PatternBase ShotPatternSlow => shotPatternSlow;
}
