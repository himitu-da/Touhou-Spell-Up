using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

[CreateAssetMenu(fileName = "ParallelPattern", menuName = "Touhou Spell Up/Bullet Pattern/Parallel")]
public class ParallelPattern : BulletPatternBase
{
    [SerializeField] private List<BulletPatternBase> patterns;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        if (patterns == null || patterns.Count == 0)
        {
            return;
        }

        var tasks = new List<UniTask>();
        foreach (var pattern in patterns)
        {
            if (token.IsCancellationRequested) return;
            if (pattern != null)
            {
                // awaitせず、タスクだけをリストに追加していく
                tasks.Add(pattern.Execute(spawnPoint, token));
            }
        }
        // UniTask.WhenAllで、リスト内の全てのタスクが完了するのを待つ
        await UniTask.WhenAll(tasks);
    }
}
