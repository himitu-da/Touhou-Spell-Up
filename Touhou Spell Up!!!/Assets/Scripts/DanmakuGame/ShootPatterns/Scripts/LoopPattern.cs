using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "LOOP_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/Loop")]
public class LoopPattern : ShootPatternBase
{
    [SerializeField]
    private ShootPatternBase pattern;

    [SerializeField, Min(0.0f)]
    private float interval = 1.0f;

    public override async UniTask ExecuteImpl(Transform spawnPoint, Bullet bulletToUse, CancellationToken token)
    {
        if (pattern == null)
        {
            Debug.LogError("Patternが設定されていません。", this);
            return;
        }

        while (!token.IsCancellationRequested)
        {
            // 子パターンの実行
            await pattern.Execute(spawnPoint, bulletToUse, token);

            if (interval > 0)
            {
            // 指定された間隔だけ待機
            await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }
        }
    }
}
