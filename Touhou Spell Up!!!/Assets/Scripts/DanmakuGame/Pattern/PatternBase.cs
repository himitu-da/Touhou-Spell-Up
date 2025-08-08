using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

// 全てのパターンの基底となる抽象クラス
public abstract class PatternBase : ScriptableObject
{
    [Tooltip("ExecuteImplの実行前に待機する時間（秒）")]
    [SerializeField] private FloatReference _beforeAwaitSeconds = new FloatReference { useConstant = true, constantValue = 0f };

    [Tooltip("ExecuteImplの実行後に待機する時間（秒）")]
    [SerializeField] private FloatReference _afterAwaitSeconds = new FloatReference { useConstant = true, constantValue = 0f };

    // GameEntityControllerを受け取るExecuteメソッド
    public virtual async UniTask Execute(GameEntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        if (_beforeAwaitSeconds.Value > 0) await UniTask.Delay(System.TimeSpan.FromSeconds(_beforeAwaitSeconds.Value), cancellationToken: token);
        if (token.IsCancellationRequested) return;

        await ExecuteImpl(controller, token);

        if (token.IsCancellationRequested) return;
        if (_afterAwaitSeconds.Value > 0) await UniTask.Delay(System.TimeSpan.FromSeconds(_afterAwaitSeconds.Value), cancellationToken: token);
    }

    // MovementStateを受け取るExecuteメソッドのオーバーロード
    public virtual async UniTask Execute(MovementState state, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        if (_beforeAwaitSeconds.Value > 0) await UniTask.Delay(System.TimeSpan.FromSeconds(_beforeAwaitSeconds.Value), cancellationToken: token);
        if (token.IsCancellationRequested) return;

        await ExecuteImpl(state, token);

        if (token.IsCancellationRequested) return;
        if (_afterAwaitSeconds.Value > 0) await UniTask.Delay(System.TimeSpan.FromSeconds(_afterAwaitSeconds.Value), cancellationToken: token);
    }

    // サブクラスで具体的な処理を実装するための抽象メソッド
    public abstract UniTask ExecuteImpl(GameEntityController controller, CancellationToken token);

    // MovementStateを受け取るExecuteImplのオーバーロード（デフォルト実装は例外をスロー）
    public virtual UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        // このメソッドはMovePattern系のクラスでoverrideされることを想定
        throw new System.NotImplementedException($"{this.GetType().Name} does not implement ExecuteImpl for MovementState.");
    }
}
