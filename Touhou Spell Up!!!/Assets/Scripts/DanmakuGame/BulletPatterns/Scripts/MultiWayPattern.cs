using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MultiWayPattern", menuName = "Touhou Spell Up/Bullet Pattern/Multi-Way")]
public class MultiWayPattern : BulletPatternBase
{
    [Header("基本設定")]
    [Tooltip("このパターンを複数方向に展開します")]
    [SerializeField] private BulletPatternBase patternToSpread;

    [Header("N-Way弾の設定")]
    [SerializeField, Range(1, 100)] private int wayCount = 5;
    [SerializeField, Range(0f, 360f)] private float totalAngle = 90f;
    [SerializeField] private bool allRound;

    [Header("自機狙い")]
    [SerializeField] private bool aimAtPlayer = false;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        if (patternToSpread == null)
        {
            Debug.LogError("Pattern To Spreadが設定されていません。", this);
            return;
        }
        if (token.IsCancellationRequested) return;

        float centerAngle = spawnPoint.eulerAngles.z;
        if (aimAtPlayer)
        {
            if (PlayerController.Instance != null)
            {
                Vector3 dir = PlayerController.Instance.transform.position - spawnPoint.position;
                centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
            }
        }

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2;
        // 全方位の場合は最後の弾が最初と重ならないようにする
        float angleStep = allRound ? finalAngle / wayCount : ((wayCount > 1) ? finalAngle / (wayCount - 1) : 0f);

        Quaternion originalRotation = spawnPoint.rotation;

        var tasks = new List<UniTask>();
        for (int i = 0; i < wayCount; i++)
        {
            if (token.IsCancellationRequested) break;

            // 全方位の場合、startAngleは不要（0度から開始するため）
            float currentAngle = centerAngle + (allRound ? 0 : startAngle) + angleStep * i;
            spawnPoint.rotation = Quaternion.Euler(0, 0, currentAngle);

            tasks.Add(patternToSpread.Execute(spawnPoint, token));
        }

        await UniTask.WhenAll(tasks);

        spawnPoint.rotation = originalRotation;
    }
}
