using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

[CreateAssetMenu(fileName = "PARA_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Parallel")]
public class ParallelPattern : ShootPatternBase
{
    [System.Serializable]
    public class ParallelStep
    {
        public ShootPatternBase pattern;
        [Tooltip("このパターンでのみ使用する弾を上書きする")]
        public Bullet overrideBullet;
    }

    [SerializeField] private List<ParallelStep> patterns;

    public override async UniTask ExecuteImpl(Transform spawnPoint, Bullet bulletToUse, CancellationToken token)
    {
        if (patterns == null || patterns.Count == 0)
        {
            return;
        }

        var tasks = new List<UniTask>();
        foreach (var step in patterns)
        {
            if (token.IsCancellationRequested) return;
            if (step.pattern != null)
            {
                // このステップで最終的に使う弾を決定
                // ステップ固有の上書きがあれば最優先、なければパターン全体で使う弾を引き継ぐ
                Bullet finalBulletForStep = step.overrideBullet != null ? step.overrideBullet : bulletToUse;

                // awaitせず、タスクだけをリストに追加していく
                tasks.Add(step.pattern.Execute(spawnPoint, finalBulletForStep, token));
            }
        }
        // UniTask.WhenAllで、リスト内の全てのタスクが完了するのを待つ
        await UniTask.WhenAll(tasks);
    }
}
