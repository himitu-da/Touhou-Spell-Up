using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System; // TimeSpanのために追加

[CreateAssetMenu(fileName = "SEQ_", menuName = "Touhou Spell Up/Bullet Pattern/Sequence")]
public class SequencePattern : BulletPatternBase
{
    [System.Serializable]
    public class PatternStep
    {
        public BulletPatternBase pattern;
        [Tooltip("このパターンの実行前に待機する時間（秒）")]
        public float delay;

        [Tooltip("このパターンでのみ使用する弾を上書きする")]
        public GameObject overrideBulletPrefab;
    }

    [SerializeField] private List<PatternStep> sequence;

    public override async UniTask Execute(Transform spawnPoint, GameObject inheritedBulletPrefab, CancellationToken token)
    {
        // パターン全体で使う弾を決定（自身の上書きがあればそれを使い、なければ親から継承）
        GameObject patternScopeBullet = this.overrideBulletPrefab != null ? this.overrideBulletPrefab : inheritedBulletPrefab;

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
                // このステップで最終的に使う弾を決定
                // ステップ固有の上書きがあれば最優先、なければパターン全体で使う弾を引き継ぐ
                GameObject finalBulletForStep = step.overrideBulletPrefab != null ? step.overrideBulletPrefab : patternScopeBullet;

                // 子パターンのUniTaskを実行し、完了を待つ
                await step.pattern.Execute(spawnPoint, finalBulletForStep, token);
            }
        }
    }
}
