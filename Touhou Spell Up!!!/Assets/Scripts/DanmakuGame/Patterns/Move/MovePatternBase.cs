using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class MovePatternBase : PatternBase
{
    public override UniTask ExecuteImpl(Mover mover, Shooter shooter, CancellationToken token)
    {
        // 実際の処理はさらにサブクラスのExecuteMoveに委譲する
        return ExecuteMove(mover, token);
    }

    // サブクラスが実装するための新しい抽象メソッド
    public abstract UniTask ExecuteMove(Mover mover, CancellationToken token);
}
