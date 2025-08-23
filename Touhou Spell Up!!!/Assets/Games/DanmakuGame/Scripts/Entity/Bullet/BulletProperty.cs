using UnityEngine;

[CreateAssetMenu(fileName = "BLTP_", menuName = "Danmaku/Entity/Bullet/BulletProperty", order = 0)]
public class BulletProperty : GameEntityProperty
{
    [SerializeField] private FloatReference initialLifeTime = new FloatReference { useConstant = true, constantValue = 6f };
    [SerializeField] private FloatReference attackPower = new FloatReference { useConstant = true, constantValue = 10f };

    public float InitialLifeTime => initialLifeTime.Value;
    public float AttackPower => attackPower.Value;
}
