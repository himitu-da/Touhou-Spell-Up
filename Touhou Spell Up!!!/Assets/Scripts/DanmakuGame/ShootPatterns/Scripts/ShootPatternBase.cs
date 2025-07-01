using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class ShootPatternBase : ScriptableObject
{
    [Tooltip("パターン以下で使う弾を上書きします")]
    [SerializeField] protected GameObject overrideBulletPrefab = null;
    public abstract UniTask Execute(Transform spawnPoint, GameObject inheritedBulletPrefab, CancellationToken token);
}
