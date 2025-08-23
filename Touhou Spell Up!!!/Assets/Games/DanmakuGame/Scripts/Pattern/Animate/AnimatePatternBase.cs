using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 見た目の変化（アニメーション）を制御するすべてのパターンの基底クラス
/// </summary>
public abstract class AnimatePatternBase : PatternBase
{
    public override UniTask ExecuteImpl(GameEntityState state, CancellationToken token)
    {
        // AnimatePatternはGameEntityStateを直接操作しない
        throw new System.NotImplementedException($"{this.GetType().Name} does not support GameEntityState execution.");
    }
}
