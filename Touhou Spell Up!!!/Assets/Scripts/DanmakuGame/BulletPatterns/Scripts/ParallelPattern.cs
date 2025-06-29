using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

[CreateAssetMenu(fileName = "PARA_", menuName = "Touhou Spell Up/Bullet Pattern/Parallel")]
public class ParallelPattern : BulletPatternBase
{
    [System.Serializable]
    public class ParallelStep
    {
        public BulletPatternBase pattern;
        [Tooltip("このパターンでのみ使用する弾を上書きする")]
        public GameObject overrideBulletPrefab;
    }

    [SerializeField] private List<ParallelStep> patterns;

    public override async UniTask Execute(Transform spawnPoint, GameObject inheritedBulletPrefab, CancellationToken token)
    {
        if (patterns == null || patterns.Count == 0)
        {
            return;
        }

        // パターン全体で使う弾を決定（自身の上書きがあればそれを使い、なければ親から継承）
        GameObject patternScopeBullet = this.overrideBulletPrefab != null ? this.overrideBulletPrefab : inheritedBulletPrefab;

        var tasks = new List<UniTask>();
        foreach (var step in patterns)
        {
            if (token.IsCancellationRequested) return;
            if (step.pattern != null)
            {
                // このステップで最終的に使う弾を決定
                // ステップ固有の上書きがあれば最優先、なければパターン全体で使う弾を引き継ぐ
                GameObject finalBulletForStep = step.overrideBulletPrefab != null ? step.overrideBulletPrefab : patternScopeBullet;

                // awaitせず、タスクだけをリストに追加していく
                tasks.Add(step.pattern.Execute(spawnPoint, finalBulletForStep, token));
            }
        }
        // UniTask.WhenAllで、リスト内の全てのタスクが完了するのを待つ
        await UniTask.WhenAll(tasks);
    }
}
