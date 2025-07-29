using UnityEngine;

public class GameEntity : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] protected GameEntityProperty property;

    public GameObject Prefab => prefab;
    public virtual GameEntityProperty Property => property;
}
