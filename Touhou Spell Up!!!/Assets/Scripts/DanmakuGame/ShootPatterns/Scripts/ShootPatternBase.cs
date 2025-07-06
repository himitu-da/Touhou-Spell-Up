using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum SpawnPointType
{
    RelativeToShooter, // Shooterの位置を基準にする
    Absolute,          // ワールド絶対座標
    RelativeToPlayer,  // プレイヤーを基準にする (今後の拡張用)
}

public abstract class ShootPatternBase : ScriptableObject
{
    [Tooltip("パターン以下で使う弾を上書きします")]
    [SerializeField] protected Bullet overrideBullet = null;
    [Header("発射地点の設定")]
    [SerializeField] protected SpawnPointType spawnPointType = SpawnPointType.RelativeToShooter;
    [SerializeField] protected Vector3 positionOffset = Vector3.zero;
    
    public virtual async UniTask Execute(Shooter shooter, Bullet inheritedBullet, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        // 自身の上書き設定があればそれを優先し、なければ親からの継承をそのまま使う
        Bullet bulletToUse = this.overrideBullet != null ? this.overrideBullet : inheritedBullet;

        await ExecuteImpl(shooter, bulletToUse, token);
    }

    public abstract UniTask ExecuteImpl(Shooter shooter, Bullet bulletToUse, CancellationToken token);

        // 発射地点を計算するヘルパーメソッド
    protected Vector3 GetSpawnPosition(Shooter shooter)
    {

        switch (spawnPointType)
        {
            case SpawnPointType.Absolute:
                return positionOffset;

            case SpawnPointType.RelativeToShooter:
                return shooter.transform.position + positionOffset;

            case SpawnPointType.RelativeToPlayer:
                // TODO: Playerの位置を取得する処理を実装
                // 例: return PlayerFinder.GetPosition() + positionOffset;
                Debug.LogWarning("RelativeToPlayer is not implemented yet.");
                return shooter.transform.position + positionOffset; // Fallback

            default:
                return shooter.transform.position;
        }
    }
}
