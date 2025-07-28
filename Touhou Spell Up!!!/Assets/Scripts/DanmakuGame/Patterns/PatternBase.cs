using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

// 全てのパターンの基底となる抽象クラス
public abstract class PatternBase : ScriptableObject
{
    [Tooltip("ExecuteImplの実行前に待機する時間（秒）")]
    [SerializeField] private float _beforeAwaitSeconds = 0f;

    [Tooltip("ExecuteImplの実行後に待機する時間（秒）")]
    [SerializeField] private float _afterAwaitSeconds = 0f;

    // EntityControllerを受け取るようにシグネチャを変更
    public virtual async UniTask Execute(EntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        if (_beforeAwaitSeconds > 0)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_beforeAwaitSeconds), cancellationToken: token);
        }

        if (token.IsCancellationRequested) return;

        // 実際の処理はExecuteImplに委譲する
        await ExecuteImpl(controller, token);

        if (token.IsCancellationRequested) return;

        if (_afterAwaitSeconds > 0)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_afterAwaitSeconds), cancellationToken: token);
        }
    }

    // サブクラスで具体的な処理を実装するための抽象メソッド
    public abstract UniTask ExecuteImpl(EntityController controller, CancellationToken token);
}
