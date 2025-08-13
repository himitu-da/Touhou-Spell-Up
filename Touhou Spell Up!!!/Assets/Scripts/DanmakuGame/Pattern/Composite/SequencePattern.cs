using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System; // TimeSpanのために追加

[CreateAssetMenu(fileName = "SEQ_", menuName = "Danmaku/Pattern/Composite/Sequence")]
public class SequencePattern : PatternBase
{
    [System.Serializable]
    public class PatternStep
    {
        public PatternBaseReference pattern;
        [Tooltip("このパターンの実行前に待機する時間（秒）")]
        public FloatReference delay = new FloatReference { useConstant = true, constantValue = 0f };

        // [Tooltip("このパターンでのみ使用する弾を上書きする")]
        // public Bullet overrideBullet;
    }

    [SerializeField] private List<PatternStep> sequence;

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        foreach (var step in sequence)
        {
            // キャンセルチェック
            if (token.IsCancellationRequested) return;

            if (step.delay.Value > 0)
            {
                // UniTask.Delayで待機
                await UniTask.Delay(TimeSpan.FromSeconds(step.delay.Value), cancellationToken: token);
            }
            
            // キャンセルチェック
            if (token.IsCancellationRequested) return;

            if (step.pattern != null && step.pattern.Value != null)
            {
                // 子パターンのUniTaskを実行し、完了を待つ
                await step.pattern.Value.Execute(controller, token);
            }
        }
    }

    public override async UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        foreach (var step in sequence)
        {
            if (token.IsCancellationRequested) return;

            if (step.delay.Value > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(step.delay.Value), cancellationToken: token);
            }

            if (token.IsCancellationRequested) return;

            if (step.pattern != null && step.pattern.Value != null)
            {
                await step.pattern.Value.Execute(state, token);
            }
        }
    }
}
