using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "LOOP_", menuName = "Danmaku/Pattern/Composite/Loop")]
public class LoopPattern : PatternBase
{
    [SerializeField]
    private PatternBase pattern;

    [SerializeField]
    private FloatReference interval = new FloatReference { useConstant = true, constantValue = 1.0f };
    [SerializeField]
    private IntReference loopCount = new IntReference { useConstant = true, constantValue = 0 };

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (pattern == null)
        {
            Debug.LogError("Patternが設定されていません。", this);
            return;
        }

        for (int i = 0; !token.IsCancellationRequested; ) {
            // 子パターンの実行
            await pattern.Execute(controller, token);

            if (interval.Value > 0)
            {
                // 指定された間隔だけ待機
                await UniTask.Delay((int)(interval.Value * 1000), cancellationToken: token);
            }

            if (loopCount.Value > 0)
            {
                i++;
                if (i >= loopCount.Value)
                {
                    break;
                }
            }
        }
    }

    public override async UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        if (pattern == null)
        {
            Debug.LogError("Patternが設定されていません。", this);
            return;
        }

        for (int i = 0; !token.IsCancellationRequested; )
        {
            await pattern.Execute(state, token);

            if (interval.Value > 0)
            {
                await UniTask.Delay((int)(interval.Value * 1000), cancellationToken: token);
            }

            if (loopCount.Value > 0)
            {
                i++;
                if (i >= loopCount.Value)
                {
                    break;
                }
            }
        }
    }
}
