using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "ACL_", menuName = "Danmaku/Pattern/Move/Acceleration")]
public class AccelerationMovePattern : MovePatternBase
{
    [SerializeField] private FloatReference _acceleration = new FloatReference { useConstant = true, constantValue = 1f };
    [SerializeField] private FloatReference _duration = new FloatReference { useConstant = true, constantValue = 1f };

    public override async UniTask ExecuteMove(MovementState state, CancellationToken token)
    {
        // 向きの正規化されたベクトルを取得
        Vector3 direction = state.Velocity.normalized;
        if (direction == Vector3.zero)
        {
            // 速度がゼロの場合は、向きが定義できないため上方向をデフォルトとする
            direction = Vector3.up;
        }

        float elapsedTime = 0f;
        bool isInfinite = _duration.Value <= 0;
        // 1秒あたりの加速度
        float accelerationPerSecond = isInfinite ? _acceleration.Value : _acceleration.Value / _duration.Value;

        while (isInfinite || elapsedTime < _duration.Value)
        {
            // CancellationTokenをチェック
            if (token.IsCancellationRequested)
            {
                return;
            }

            // 現在の速度を取得
            float currentSpeed = state.Velocity.magnitude;
            // 新しい速度を計算
            float newSpeed = currentSpeed + accelerationPerSecond * Time.fixedDeltaTime;
            // 速度を更新
            state.Velocity = direction * newSpeed;

            if (!isInfinite)
            {
                elapsedTime += Time.fixedDeltaTime;
            }

            // 次のFixedUpdateまで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }
}