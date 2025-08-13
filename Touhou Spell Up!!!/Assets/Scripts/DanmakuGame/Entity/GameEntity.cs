using UnityEngine;

public class GameEntity : ScriptableObject
{
    [SerializeField] private PrefabReference prefab;
    [SerializeField] protected GameEntityPropertyReference property;

    public GameObject Prefab => prefab.Value;
    public virtual GameEntityProperty Property => property.Value;
}
