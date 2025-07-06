using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum SpawnPointType
{
    RelativeToShooter, // Shooterの位置を基準にする
    Absolute,          // ワールド絶対座標
    RelativeToPlayer,  // プレイヤーを基準にする (今後の拡張用)
}

public abstract class ShootPatternBase : ShootablePattern
{
    [Header("発射地点の設定")]
    [SerializeField] protected SpawnPointType spawnPointType = SpawnPointType.RelativeToShooter;
    [SerializeField] protected Vector3 positionOffset = Vector3.zero;
    [Header("発射角度の設定")]
    [SerializeField] protected float directionOffset = 0f;

    // ExecuteImplのシグネチャを変更
    public override UniTask ExecuteImpl(Mover _, Shooter shooter, Bullet bulletToUse, CancellationToken token)
    {
        // 既存のExecuteImplのロジックをここに移動、またはサブクラスに委譲
        // このクラス自体は抽象なので、サブクラスに実装を強制する
        return ExecuteShoot(shooter, bulletToUse, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteShoot(Shooter shooter, Bullet bulletToUse, CancellationToken token);

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
