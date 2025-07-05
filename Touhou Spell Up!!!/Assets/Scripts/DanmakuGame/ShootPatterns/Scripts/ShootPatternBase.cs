using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class ShootPatternBase : ScriptableObject
{
    [Tooltip("パターン以下で使う弾を上書きします")]
    [SerializeField] protected Bullet overrideBullet = null;
    public virtual async UniTask Execute(Transform spawnPoint, Bullet inheritedBullet, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        // 自身の上書き設定があればそれを優先し、なければ親からの継承をそのまま使う
        Bullet bulletToUse = this.overrideBullet != null ? this.overrideBullet : inheritedBullet;

        await ExecuteImpl(spawnPoint, bulletToUse, token);
    }

    public abstract UniTask ExecuteImpl(Transform spawnPoint, Bullet bulletToUse, CancellationToken token);
}
