using UnityEngine;

[CreateAssetMenu(fileName = "BLTP_", menuName = "Touhou Spell Up/Danmaku/BulletProperty", order = 0)]
public class BulletProperty : GameEntityProperty
{
    [SerializeField] private float lifeTime = 6f;
    public float LifeTime => lifeTime;
}
