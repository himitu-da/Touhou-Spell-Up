using UnityEngine;

[CreateAssetMenu(fileName = "BLT_", menuName = "Touhou Spell Up/Danmaku/Bullet", order = 1)]
public class Bullet : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private BulletProperty property;

    public GameObject Prefab => prefab;
    public BulletProperty Property => property;
}
