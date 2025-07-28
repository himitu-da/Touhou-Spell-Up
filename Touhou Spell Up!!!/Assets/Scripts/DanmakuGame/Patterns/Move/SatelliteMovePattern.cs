using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "SAT_", menuName = "Touhou Spell Up/Danmaku/Move Pattern/Satellite", order = 2)]
public class SatelliteMovePattern : MovePatternBase
{
    [Header("中心点の設定")]
    [SerializeField] private SpawnPointType spawnPointType = SpawnPointType.RelativeToShooter;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Header("移動設定")]
    [SerializeField] private float initialRadius = 1f;    // 初期半径
    [SerializeField] private float radialSpeed = 0f;      // 半径方向の速度 (ユニット/秒)
    [SerializeField] private float angularSpeed = 90f;    // 角速度 (度/秒)
    [SerializeField] private float tangentialSpeed = 0f;  // 接線速度 (ユニット/秒)

    [Header("追跡設定")]
    [SerializeField] private bool followShooter = true; // 親を追跡するか

    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (controller == null)
        {
            Debug.LogError("SatelliteMovePattern requires a controller.", this);
            return UniTask.CompletedTask;
        }

        GameEntityController centerController = controller.ParentActor;
        if (centerController == null)
        {
            // 親が設定されていない場合、自分自身を基準点とする（デバッグ用）
            Debug.LogWarning("SatelliteMovePattern requires a parent Actor, but it is not set. Using self as center.", controller);
            centerController = controller;
        }

        // 実際の処理はExecuteSatelliteMoveに委譲
        return ExecuteSatelliteMove(controller, centerController, token);
    }

    // このパターンはActor（中心点）が必須なため、Actorを引数に取る
    private async UniTask ExecuteSatelliteMove(GameEntityController controller, GameEntityController centerController, CancellationToken token)
    {
        if (controller == null) return;

        Vector3 centerPosition = GetSpawnPosition(centerController);

        // 初期位置と角度を設定
        float radius = initialRadius;
        // 初期角度はMoverの現在の向きから取得する
        float currentAngle = controller.transform.rotation.eulerAngles.z;

        // 弾の初期位置を計算して設定
        Vector3 initialDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
        controller.transform.position = centerPosition + initialDirection * radius;

        while (!token.IsCancellationRequested && controller != null)
        {
            if (followShooter && centerController != null)
            {
                centerPosition = GetSpawnPosition(centerController);
            }

            // 1. 半径を更新
            radius += radialSpeed * Time.deltaTime;

            // 2. 角度を更新
            float angularSpeedDelta = angularSpeed * Time.deltaTime;
            float tangentialSpeedDelta = 0f;
            if (radius > 0.001f) // 半径がほぼ0の場合は接線速度による回転は発生しない
            {
                // 接線速度を角速度（度数法）に変換
                tangentialSpeedDelta = (tangentialSpeed / radius) * Mathf.Rad2Deg * Time.deltaTime;
            }
            currentAngle += angularSpeedDelta + tangentialSpeedDelta;

            // 3. 新しい位置を計算
            Vector3 newDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
            controller.transform.position = centerPosition + newDirection * radius;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    // このMovePatternはExecuteImplで処理が完結するため、MovePatternBaseの抽象メソッドは空実装でよい
    public override UniTask ExecuteMove(GameEntityController controller, CancellationToken token)
    {
        // このメソッドは呼ばれない想定
        Debug.LogWarning("SatelliteMovePattern.ExecuteMove(EntityController, CancellationToken) was called. This should not happen.", controller);
        return UniTask.CompletedTask;
    }

    // 発射地点を計算するヘルパーメソッド（ShootPatternBaseから拝借）
    protected Vector3 GetSpawnPosition(GameEntityController controller)
    {
        switch (spawnPointType)
        {
            case SpawnPointType.Absolute:
                return positionOffset;

            case SpawnPointType.RelativeToShooter:
                if (controller != null)
                {
                    return controller.transform.position + positionOffset;
                }
                else
                {
                    Debug.LogWarning("Movable is null, but spawnPointType is RelativeToShooter. Falling back to absolute position.");
                    return positionOffset;
                }

            case SpawnPointType.RelativeToPlayer:
                // TODO: Playerの位置を取得する処理を実装
                Debug.LogWarning("RelativeToPlayer is not implemented yet.");
                if (controller != null)
                {
                    return controller.transform.position + positionOffset; // Fallback
                }
                return positionOffset; // Fallback

            default:
                if (controller != null)
                {
                    return controller.transform.position;
                }
                return Vector3.zero;
        }
    }
}
