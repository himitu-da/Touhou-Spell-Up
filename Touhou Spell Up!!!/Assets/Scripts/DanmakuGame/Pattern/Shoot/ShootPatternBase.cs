using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;


public enum SpawnPointType
{
    RelativeToShooter, // Shooterの位置を基準にする
    Absolute,          // ワールド絶対座標
    RelativeToPlayer,  // プレイヤーを基準にする (今後の拡張用)
}

public abstract class ShootPatternBase : PatternBase
{
    [Header("弾の設定")]
    [SerializeField] protected GameEntityReference _entity;

    [Header("発射地点の設定")]
    [SerializeField] protected SpawnPointTypeReference spawnPointType = new SpawnPointTypeReference { useConstant = true, constantValue = SpawnPointType.RelativeToShooter };
    [SerializeField] protected Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };
    [Tooltip("射撃中にシューターの位置に追従するか")]
    [SerializeField] protected BoolReference followShooterPosition = new BoolReference { useConstant = true, constantValue = false };

    [Header("発生源シェイプ（オプション）")]
    [SerializeField] protected EmissionShapeReference emissionShape;
    [Tooltip("順次発射か（同時ならfalse）")]
    [SerializeField] protected BoolReference sequential = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("順次発射時の間隔（秒）")]
    [SerializeField] protected FloatReference emissionInterval = new FloatReference { useConstant = true, constantValue = 0.1f };

    [Header("発射角度の設定")]
    [SerializeField] protected FloatReference directionOffset = new FloatReference { useConstant = true, constantValue = 0f };
    [Tooltip("自機を狙うか")]
    [SerializeField] protected BoolReference aimAtPlayer = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("常に自機を狙い続けるか")]
    [SerializeField] protected BoolReference alwaysAimToPlayer = new BoolReference { useConstant = true, constantValue = false };
    [Header("発射地点の極座標オフセット")]
    [Tooltip("発射地点からのオフセット距離")]
    [SerializeField] protected FloatReference spawnRadius = new FloatReference { useConstant = true, constantValue = 0f };

    // ExecuteImplのシグネチャを変更
    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        // 既存のExecuteImplのロジックをここに移動、またはサブクラスに委譲
        // このクラス自体は抽象なので、サブクラスに実装を強制する
        return ExecuteShoot(controller, token);
    }

    // ShootPatternの本体。EmissionShapeの有無で処理を分岐し、最終的にExecuteShootFromPointを呼び出す
    public virtual async UniTask ExecuteShoot(GameEntityController controller, CancellationToken token)
    {
        var emissions = new List<EmissionData>();

        if (emissionShape == null || emissionShape.Value == null)
        {
            // EmissionShapeがない場合、単一の発生源として動作
            emissions.Add(new EmissionData
            {
                localPosition = positionOffset.Value, // 従来の位置オフセット
                localAngle = 0 // 角度はサブクラスで計算
            });
        }
        else
        {
            // EmissionShapeがある場合、そこから発生源リストを取得
            emissions.AddRange(emissionShape.Value.GetEmissions(controller));
        }

        // 各発生源から弾を発射
        foreach (var emission in emissions)
        {
            if (token.IsCancellationRequested) break;

            await ExecuteShootFromPoint(controller, emission, token);

            if (sequential.Value && emissionInterval.Value > 0)
            {
                await UniTask.Delay((int)(emissionInterval.Value * 1000), cancellationToken: token);
            }
        }
    }

    // サブクラスは、このメソッドを実装して「１つの発生源からどのように弾を撃つか」を定義する
    public abstract UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token);

    protected float GetAimAngle(GameEntityController controller, Vector3 spawnPosition)
    {
        if (aimAtPlayer.Value)
        {
            // 180度回転させて、逆向きになる問題を修正
            return AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else
        {
            return controller.transform.eulerAngles.z;
        }
    }

    // 発射地点を計算するヘルパーメソッド
    protected Vector3 GetSpawnPosition(GameEntityController controller)
    {
        switch (spawnPointType.Value)
        {
            case SpawnPointType.Absolute:
                return positionOffset.Value;

            case SpawnPointType.RelativeToShooter:
                return controller.transform.position + positionOffset.Value;

            case SpawnPointType.RelativeToPlayer:
                var playerTransform = PlayerUtility.GetPlayerTransform();
                if (playerTransform != null)
                {
                    return playerTransform.position + positionOffset.Value;
                }
                else
                {
                    // プレイヤーが見つからない場合はシューターを基準にする
                    Debug.LogWarning("Player not found for RelativeToPlayer spawn type. Falling back to RelativeToShooter.");
                    return controller.transform.position + positionOffset.Value;
                }

            default:
                return controller.transform.position;
        }
    }

    protected Vector3 CalculateFinalSpawnPosition(Vector3 basePosition, float angle)
    {
        if (spawnRadius.Value <= 0)
        {
            return basePosition;
        }
        Vector3 polarOffset = Quaternion.Euler(0, 0, angle) * Vector3.up * spawnRadius.Value;
        return basePosition + polarOffset;
    }
}
