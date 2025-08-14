using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class MovePatternBase : PatternBase
{
    // GameEntityController版はvirtualな空実装に戻す（SatelliteMovePatternのため）
    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        // 基本的には呼ばれないが、SatelliteMovePatternのような特殊なケースのために残す
        return UniTask.CompletedTask;
    }

    // MovementState版のExecuteImplをoverride
    public override UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        return ExecuteMove(state, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteMove(MovementState state, CancellationToken token);
}
