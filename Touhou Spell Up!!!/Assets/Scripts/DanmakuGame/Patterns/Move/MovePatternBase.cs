using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class MovePatternBase : PatternBase
{
    public override UniTask ExecuteImpl(EntityController controller, CancellationToken token)
    {
        // 実際の処理はさらにサブクラスのExecuteMoveに委譲する
        return ExecuteMove(controller, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteMove(EntityController controller, CancellationToken token);
}
