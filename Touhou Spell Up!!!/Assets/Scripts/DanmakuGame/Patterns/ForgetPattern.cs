using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "FGT_", menuName = "Touhou Spell Up/Danmaku/Forget")]
public class ForgetPattern : PatternBase
{
    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        // 中身は空。待機処理はPatternBase側で行う
        await UniTask.CompletedTask;
    }
}
