using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "SAT_", menuName = "Danmaku/Pattern/Move/Satellite", order = 2)]
public class SatelliteMovePattern : MovePatternBase
{
    [Header("中心点の設定")]
    [SerializeField] private SpawnPointTypeReference spawnPointType = new SpawnPointTypeReference { useConstant = true, constantValue = SpawnPointType.RelativeToShooter };
    [SerializeField] private Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    [Header("移動設定")]
    [SerializeField] private FloatReference initialRadius = new FloatReference { useConstant = true, constantValue = 1f };    // 初期半径
    [SerializeField] private FloatReference radialSpeed = new FloatReference { useConstant = true, constantValue = 0f };      // 半径方向の速度 (ユニット/秒)
    [SerializeField] private FloatReference angularSpeed = new FloatReference { useConstant = true, constantValue = 90f };    // 角速度 (度/秒)
    [SerializeField] private FloatReference tangentialSpeed = new FloatReference { useConstant = true, constantValue = 0f };  // 接線速度 (ユニット/秒)

    [Header("追跡設定")]
    [SerializeField] private BoolReference followShooter = new BoolReference { useConstant = true, constantValue = true }; // 親を追跡するか

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

        float radius = initialRadius.Value;
        float currentAngle = state.Rotation.eulerAngles.z; // 累積的に更新される現在の角度
        
        // 弾の初期位置を計算して設定
        Vector3 initialDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
        state.Position = centerPosition + initialDirection * radius;

        while (!token.IsCancellationRequested)
        {
            if (followShooter.Value && centerController != null)
            {
                centerPosition = GetSpawnPosition(centerController);
            }

            radius += radialSpeed.Value * Time.fixedDeltaTime;

            // 接線速度を角速度に変換
            float tangentialAngularSpeed = 0f;
            if (radius > 0.001f) // ゼロ除算を避ける
            {
                tangentialAngularSpeed = (tangentialSpeed.Value / radius) * Mathf.Rad2Deg;
            }

            // 総角速度を計算
            float totalAngularSpeed = angularSpeed.Value + tangentialAngularSpeed;

            // 角度を累積的に更新（重要な修正点）
            currentAngle += totalAngularSpeed * Time.fixedDeltaTime;

            // 向きは更新せず、位置だけを更新する
            Vector3 newDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
            state.Position = centerPosition + newDirection * radius;
            // このパターンではVelocityは直接制御しない
            state.Velocity = Vector3.zero;

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
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
        switch (spawnPointType.Value)
        {
            case SpawnPointType.Absolute:
                return positionOffset.Value;

            case SpawnPointType.RelativeToShooter:
                if (controller != null)
                {
                    return controller.transform.position + positionOffset.Value;
                }
                else
                {
                    Debug.LogWarning("Movable is null, but spawnPointType is RelativeToShooter. Falling back to absolute position.");
                    return positionOffset.Value;
                }

            case SpawnPointType.RelativeToPlayer:
                // TODO: Playerの位置を取得する処理を実装
                Debug.LogWarning("RelativeToPlayer is not implemented yet.");
                if (controller != null)
                {
                    return controller.transform.position + positionOffset.Value; // Fallback
                }
                return positionOffset.Value; // Fallback

            default:
                if (controller != null)
                {
                    return controller.transform.position;
                }
                return Vector3.zero;
        }
    }
}
