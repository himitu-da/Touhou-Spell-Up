using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "SAT_", menuName = "Danmaku/Pattern/Move/Satellite", order = 2)]
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

    // MovementStateだけでは親を取得できないため、GameEntityController版のExecuteImplをoverrideする
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
            Debug.LogWarning("SatelliteMovePattern requires a parent Actor, but it is not set. Using self as center.", controller);
            centerController = controller;
        }

        // publicプロパティ経由でMovementStateを取得
        MovementState state = controller.MovementState;
        if (state == null)
        {
            Debug.LogError("MovementState is null in GameEntityController.", this);
            return UniTask.CompletedTask;
        }

        return ExecuteSatelliteMove(state, centerController, token);
    }

    private async UniTask ExecuteSatelliteMove(MovementState state, GameEntityController centerController, CancellationToken token)
    {
        Vector3 centerPosition = GetSpawnPosition(centerController);

        float radius = initialRadius;
        float currentAngle = state.Rotation.eulerAngles.z;

        // 弾の初期位置を計算して設定
        Vector3 initialDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
        state.Position = centerPosition + initialDirection * radius;

        while (!token.IsCancellationRequested)
        {
            if (followShooter && centerController != null)
            {
                centerPosition = GetSpawnPosition(centerController);
            }

            radius += radialSpeed * Time.deltaTime;

            float angularSpeedDelta = angularSpeed * Time.deltaTime;
            float tangentialSpeedDelta = 0f;
            if (radius > 0.001f)
            {
                tangentialSpeedDelta = (tangentialSpeed / radius) * Mathf.Rad2Deg * Time.deltaTime;
            }
            currentAngle += angularSpeedDelta + tangentialSpeedDelta;

            // 向きは更新せず、位置だけを更新する
            Vector3 newDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
            state.Position = centerPosition + newDirection * radius;
            // このパターンではVelocityは直接制御しない
            state.Velocity = Vector3.zero;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    // MovePatternBaseの抽象メソッドを実装
    public override UniTask ExecuteMove(MovementState state, CancellationToken token)
    {
        // このパターンは親Actorが必須なため、MovementStateだけでは実行できない。
        // ExecuteImpl(GameEntityController, ...)が代わりに呼ばれる。
        Debug.LogError("SatelliteMovePattern.ExecuteMove(MovementState, CancellationToken) should not be called directly.", this);
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
