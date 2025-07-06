using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "LOOP_", menuName = "Touhou Spell Up/Danmaku/Composite/Loop")]
public class LoopPattern : ShootablePattern
{
    [SerializeField]
    private PatternBase pattern;

    [SerializeField, Min(0.0f)]
    private float interval = 1.0f;

    public override async UniTask ExecuteImpl(Mover mover, Shooter shooter, Bullet bulletToUse, CancellationToken token)
    {
        if (pattern == null)
        {
            Debug.LogError("Patternが設定されていません。", this);
            return;
        }

        while (!token.IsCancellationRequested)
        {
            // 子パターンの実行
            await pattern.Execute(mover, shooter, bulletToUse, token);

            if (interval > 0)
            {
            // 指定された間隔だけ待機
            await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }
        }
    }
}
