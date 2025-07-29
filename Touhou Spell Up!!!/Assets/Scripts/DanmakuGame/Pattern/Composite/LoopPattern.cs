using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "LOOP_", menuName = "Danmaku/Pattern/Composite/Loop")]
public class LoopPattern : PatternBase
{
    [SerializeField]
    private PatternBase pattern;

    [SerializeField, Min(0.0f)]
    private float interval = 1.0f;
    [SerializeField, Min(0)]
    private int loopCount = 0;

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

            if (interval > 0)
            {
                // 指定された間隔だけ待機
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }

            if (loopCount > 0)
            {
                i++;
                if (i >= loopCount)
                {
                    break;
                }
            }
        }
    }
}
