using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "STR_", menuName = "Danmaku/Pattern/Move/Straight")]
public class StraightMovePattern : MovePatternBase
{
    [SerializeField] private FloatReference _speed = new FloatReference { useConstant = true, constantValue = 5f };
    // durationは不要になるため削除

    public override UniTask ExecuteMove(MovementState state, CancellationToken token)
    {
        // 向き（Rotation）に基づいて速度を設定する
        // 現在はスプライトが上向き前提なので、upベクトルを回転させて進行方向を決定
        state.Velocity = state.Rotation * Vector3.up * _speed.Value;
        
        // このパターンは状態を設定するだけなので、即座に完了する
        return UniTask.CompletedTask;
    }
}
