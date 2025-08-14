using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FAF_", menuName = "Danmaku/Pattern/FireAndForget")]
public class FireAndForgetPattern : PatternBase
{
    [SerializeField] private List<PatternBaseReference> _patterns = new List<PatternBaseReference>();

    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return UniTask.CompletedTask;

        foreach (var patternRef in _patterns)
        {
            if (patternRef != null && patternRef.Value != null)
            {
                // awaitを使用しない
                patternRef.Value.Execute(controller, token).Forget();
            }
        }

        return UniTask.CompletedTask;
    }
}
