using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FAF_", menuName = "Danmaku/Pattern/FireAndForget")]
public class FireAndForgetPattern : PatternBase
{
    [SerializeField] private List<PatternBase> _patterns = new List<PatternBase>();

    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return UniTask.CompletedTask;

        foreach (var pattern in _patterns)
        {
            // awaitを使用しない
            pattern.Execute(controller, token).Forget();
        }

        return UniTask.CompletedTask;
    }
}
