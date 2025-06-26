using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System; // TimeSpanのために追加

[CreateAssetMenu(fileName = "SequencePattern", menuName = "Touhou Spell Up/Bullet Pattern/Sequence")]
public class SequencePattern : BulletPatternBase
{
    [System.Serializable]
    public class PatternStep
    {
        public BulletPatternBase pattern;
        [Tooltip("このパターンの実行前に待機する時間（秒）")]
        public float delay;
    }

    [SerializeField] private List<PatternStep> sequence;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        foreach (var step in sequence)
        {
            // キャンセルチェック
            if (token.IsCancellationRequested) return;

            if (step.delay > 0)
            {
                // UniTask.Delayで待機
                await UniTask.Delay(TimeSpan.FromSeconds(step.delay), cancellationToken: token);
            }
            
            // キャンセルチェック
            if (token.IsCancellationRequested) return;

            if (step.pattern != null)
            {
                // 子パターンのUniTaskを実行し、完了を待つ
                await step.pattern.Execute(spawnPoint, token);
            }
        }
    }
}
