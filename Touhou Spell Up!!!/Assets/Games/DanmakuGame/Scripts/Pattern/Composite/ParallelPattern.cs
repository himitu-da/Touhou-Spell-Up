using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

[CreateAssetMenu(fileName = "PARA_", menuName = "Danmaku/Pattern/Composite/Parallel")]
public class ParallelPattern : PatternBase
{
    [System.Serializable]
    public class ParallelStep
    {
        public PatternBaseReference pattern;
        // [Tooltip("このパターンでのみ使用する弾を上書きする")]
        // public Bullet overrideBullet;
    }

    [SerializeField] private List<ParallelStep> patterns;

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (patterns == null || patterns.Count == 0)
        {
            return;
        }

        var tasks = new List<UniTask>();
        foreach (var step in patterns)
        {
            if (token.IsCancellationRequested) return;
            if (step.pattern != null && step.pattern.Value != null)
            {
                // awaitせず、タスクだけをリストに追加していく
                tasks.Add(step.pattern.Value.Execute(controller, token));
            }
        }
        // UniTask.WhenAllで、リスト内の全てのタスクが完了するのを待つ
        await UniTask.WhenAll(tasks);
    }

    public override async UniTask ExecuteImpl(GameEntityState state, CancellationToken token)
    {
        if (patterns == null || patterns.Count == 0)
        {
            return;
        }

        var tasks = new List<UniTask>();
        foreach (var step in patterns)
        {
            if (token.IsCancellationRequested) return;
            if (step.pattern != null && step.pattern.Value != null)
            {
                tasks.Add(step.pattern.Value.Execute(state, token));
            }
        }
        await UniTask.WhenAll(tasks);
    }
}
