using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "LoopPattern", menuName = "Touhou Spell Up/Bullet Pattern/Loop")]
public class LoopPattern : BulletPatternBase
{
    [SerializeField]
    private BulletPatternBase pattern;

    [SerializeField, Min(0.01f)]
    private float interval = 1.0f;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        if (pattern == null)
        {
            Debug.LogError("Patternが設定されていません。", this);
            return;
        }

        while (!token.IsCancellationRequested)
        {
            // 子パターンの実行
            await pattern.Execute(spawnPoint, token);

            // 指定された間隔だけ待機
            await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
        }
    }
}
