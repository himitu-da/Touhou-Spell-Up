using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class MovePatternBase : PatternBase
{
    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        // 実際の処理はさらにサブクラスのExecuteMoveに委譲する
        return ExecuteMove(controller, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteMove(GameEntityController controller, CancellationToken token);
}
