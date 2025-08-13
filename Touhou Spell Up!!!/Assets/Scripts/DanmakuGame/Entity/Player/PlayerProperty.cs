using UnityEngine;

[CreateAssetMenu(fileName = "PLYP_", menuName = "Danmaku/Entity/Player/PlayerProperty", order = 0)]
public class PlayerProperty : GameEntityProperty
{
    [Header("移動")]
    [SerializeField] private FloatReference moveSpeed = new FloatReference { useConstant = true, constantValue = 12f };
    [SerializeField] private FloatReference moveSpeedSlow = new FloatReference { useConstant = true, constantValue = 6f };
    [SerializeField] private RectReference movableArea;

    [Header("ショット")]
    [SerializeField] private FloatReference shotInterval = new FloatReference { useConstant = true, constantValue = 0.15f };
    [SerializeField] private PatternBaseReference shotPatternNormal;
    [SerializeField] private PatternBaseReference shotPatternSlow;


    public float MoveSpeed => moveSpeed.Value;
    public float MoveSpeedSlow => moveSpeedSlow.Value;
    public Rect MovableArea => movableArea.Value;
    public float ShotInterval => shotInterval.Value;
    public PatternBase ShotPatternNormal => shotPatternNormal.Value;
    public PatternBase ShotPatternSlow => shotPatternSlow.Value;
}
