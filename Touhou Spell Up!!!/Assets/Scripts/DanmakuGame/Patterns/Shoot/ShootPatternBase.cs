using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum SpawnPointType
{
    RelativeToShooter, // Shooterの位置を基準にする
    Absolute,          // ワールド絶対座標
    RelativeToPlayer,  // プレイヤーを基準にする (今後の拡張用)
}

public abstract class ShootPatternBase : PatternBase
{
    [Header("弾の設定")]
    [SerializeField] protected Bullet _bullet;

    [Header("発射地点の設定")]
    [SerializeField] protected SpawnPointType spawnPointType = SpawnPointType.RelativeToShooter;
    [SerializeField] protected Vector3 positionOffset = Vector3.zero;
    [Tooltip("射撃中にシューターの位置に追従するか")]
    [SerializeField] protected bool followShooterPosition = false;

    [Header("発射角度の設定")]
    [SerializeField] protected float directionOffset = 0f;
    [Tooltip("自機を狙うか")]
    [SerializeField] protected bool aimAtPlayer = false;
    [Tooltip("常に自機を狙い続けるか")]
    [SerializeField] protected bool alwaysAimToPlayer = false;
    [Header("発射地点の極座標オフセット")]
    [Tooltip("発射地点からのオフセット距離")]
    [SerializeField] protected float spawnRadius = 0f;

    // ExecuteImplのシグネチャを変更
    public override UniTask ExecuteImpl(Mover _, Shooter shooter, CancellationToken token)
    {
        // 既存のExecuteImplのロジックをここに移動、またはサブクラスに委譲
        // このクラス自体は抽象なので、サブクラスに実装を強制する
        return ExecuteShoot(shooter, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteShoot(Shooter shooter, CancellationToken token);

    protected float GetAimAngle(Shooter shooter, Vector3 spawnPosition)
    {
        if (aimAtPlayer)
        {
            // 180度回転させて、逆向きになる問題を修正
            return AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else
        {
            return shooter.transform.eulerAngles.z;
        }
    }

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

    protected Vector3 CalculateFinalSpawnPosition(Vector3 basePosition, float angle)
    {
        if (spawnRadius <= 0)
        {
            return basePosition;
        }
        Vector3 polarOffset = Quaternion.Euler(0, 0, angle) * Vector3.up * spawnRadius;
        return basePosition + polarOffset;
    }
}
