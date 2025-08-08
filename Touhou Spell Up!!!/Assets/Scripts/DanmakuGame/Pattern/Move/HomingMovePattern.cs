using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 対象を追従する移動パターン。
/// </summary>
[CreateAssetMenu(fileName = "HOM_", menuName = "Danmaku/Pattern/Move/Homing")]
public class HomingMovePattern : MovePatternBase
{
    [SerializeField]
    [Tooltip("追従の強さ（秒速）")]
    private FloatReference _homingSpeed = new FloatReference { useConstant = true, constantValue = 2f };

    [SerializeField]
    [Tooltip("方向転換の速さ（度/秒）")]
    private FloatReference _angularSpeed = new FloatReference { useConstant = true, constantValue = 180f };

    [SerializeField]
    [Tooltip("追従を継続する時間（秒）。0以下の場合は無制限。")]
    private FloatReference _duration = new FloatReference { useConstant = true, constantValue = 5f };

    public override async UniTask ExecuteMove(MovementState state, CancellationToken token)
    {
        var playerTransform = PlayerUtility.GetPlayerTransform();
        if (playerTransform == null)
        {
            Debug.LogError("追従対象のプレイヤーが見つかりません。");
            return;
        }

        var startTime = Time.time;

        while (!token.IsCancellationRequested)
        {
            // 継続時間が設定されていれば、時間をチェック
            if (_duration.Value > 0 && Time.time - startTime > _duration.Value)
            {
                break;
            }

            // プレイヤーが破棄されていないかチェック
            if (playerTransform == null)
            {
                // プレイヤーがいない場合は追従を停止
                break;
            }

            // 対象への方向ベクトル
            Vector3 targetDirection = (playerTransform.position - state.Position).normalized;

            // 現在の速度ベクトル
            Vector3 currentVelocity = state.Velocity;

            // 新しい速度ベクトルを計算
            // 現在の速度ベクトルを、対象への方向ベクトルに、角速度制限をかけながら近づける
            Vector3 newVelocity = Vector3.RotateTowards(currentVelocity, targetDirection * currentVelocity.magnitude, _angularSpeed.Value * Mathf.Deg2Rad * Time.deltaTime, 0.0f);

            // 対象への移動速度（誘導ベクトル）を加算
            // homingSpeedは、既存の速度にどれだけ強く影響を与えるかの係数として扱う
            state.Velocity = newVelocity + (targetDirection * _homingSpeed.Value * Time.deltaTime);

            // 向きを速度ベクトルに合わせる
            if (state.Velocity.sqrMagnitude > 0.01f) // 速度がゼロに近い場合は向きを変えない
            {
                float angle = Mathf.Atan2(state.Velocity.y, state.Velocity.x) * Mathf.Rad2Deg - 90f;
                state.Rotation = Quaternion.Euler(0, 0, angle);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
