using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperty", menuName = "Touhou-Spell-Up/PlayerProperty", order = 0)]
public class PlayerProperty : ScriptableObject
{
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float moveSpeedSlow = 6f;
    [SerializeField] float shotInterval = 0.15f;
    [SerializeField] Rect movableArea;

    public float MoveSpeed => moveSpeed;
    public float MoveSpeedSlow => moveSpeedSlow;
    public float ShotInterval => shotInterval;
    public Rect MovableArea => movableArea;
}
