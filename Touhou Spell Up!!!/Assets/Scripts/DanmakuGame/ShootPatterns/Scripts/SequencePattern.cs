using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System; // TimeSpanのために追加

[CreateAssetMenu(fileName = "SEQ_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Sequence")]
public class SequencePattern : ShootPatternBase
{
    [System.Serializable]
    public class PatternStep
    {
        public ShootPatternBase pattern;
        [Tooltip("このパターンの実行前に待機する時間（秒）")]
        public float delay;

        [Tooltip("このパターンでのみ使用する弾を上書きする")]
        public Bullet overrideBullet;
    }

    [SerializeField] private List<PatternStep> sequence;

    public override async UniTask ExecuteImpl(Shooter shooter, Bullet bulletToUse, CancellationToken token)
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
                // このステップで最終的に使う弾を決定
                // ステップ固有の上書きがあれば最優先、なければパターン全体で使う弾を引き継ぐ
                Bullet finalBulletForStep = step.overrideBullet != null ? step.overrideBullet : bulletToUse;

                // 子パターンのUniTaskを実行し、完了を待つ
                await step.pattern.Execute(shooter, finalBulletForStep, token);
            }
        }
    }
}
