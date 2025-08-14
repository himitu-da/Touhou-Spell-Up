using UnityEngine;

public class GameEntity : ScriptableObject
{
    [SerializeField] private PrefabReference prefab;
    [SerializeField] protected GameEntityPropertyReference property;
    [SerializeField] private FloatReference _lifetime = new FloatReference { useConstant = true, constantValue = 10f };

    public GameObject Prefab => prefab.Value;
    public virtual GameEntityProperty Property => property.Value;
    public float Lifetime => _lifetime.Value;
}
