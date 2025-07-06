using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class MovePatternBase : ScriptableObject
{
    public virtual async UniTask Execute(Mover mover, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        await ExecuteImpl(mover, token);
    }

    public abstract UniTask ExecuteImpl(Mover mover, CancellationToken token);
}
