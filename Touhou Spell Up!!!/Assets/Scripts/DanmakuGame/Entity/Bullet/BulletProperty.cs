using UnityEngine;

[CreateAssetMenu(fileName = "BLTP_", menuName = "Danmaku/Entity/Bullet/BulletProperty", order = 0)]
public class BulletProperty : GameEntityProperty
{
    [SerializeField] private FloatReference lifeTime = new FloatReference { useConstant = true, constantValue = 6f };
    [SerializeField] private FloatReference attackPower = new FloatReference { useConstant = true, constantValue = 10f };

    public float LifeTime => lifeTime.Value;
    public float AttackPower => attackPower.Value;
}
