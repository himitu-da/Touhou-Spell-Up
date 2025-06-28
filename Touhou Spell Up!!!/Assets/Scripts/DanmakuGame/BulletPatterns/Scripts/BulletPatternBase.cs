using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class BulletPatternBase : ScriptableObject
{
    public abstract UniTask Execute(Transform spawnPoint, CancellationToken token);
}
