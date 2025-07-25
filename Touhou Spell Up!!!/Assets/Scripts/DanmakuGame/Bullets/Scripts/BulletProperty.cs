using UnityEngine;

[CreateAssetMenu(fileName = "BLTP_", menuName = "Touhou Spell Up/Danmaku/BulletProperty", order = 0)]
public class BulletProperty : ScriptableObject
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 6f;
    [SerializeField] private PatternBase shootPattern;
    [SerializeField] private PatternBase movePattern;

    public float Speed => speed;
    public float LifeTime => lifeTime;
    public PatternBase ShootPattern => shootPattern;
    public PatternBase MovePattern => movePattern;
}
