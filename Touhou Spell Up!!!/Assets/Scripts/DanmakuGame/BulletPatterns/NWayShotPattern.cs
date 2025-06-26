using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "NWayShotPattern", menuName = "Touhou Spell Up/Bullet Pattern/N-Way Shot")]
public class NWayShotPattern : BulletPatternBase
{
    [Header("弾の設定")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("N-Way弾の設定")]
    [SerializeField, Range(1, 30)] private int wayCount = 5;
    [SerializeField, Range(0f, 360f)] private float angle = 90f;

    public override async UniTask Execute(Transform spawnPoint, CancellationToken token)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefabが設定されていません。", this);
            return;
        }

        if (token.IsCancellationRequested) return;

        float startAngle = -angle / 2;
        // wayCountが1の場合はステップを0にする（ゼロ除算を避ける）
        float angleStep = (wayCount > 1) ? angle / (wayCount - 1) : 0f;

        for (int i = 0; i < wayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, spawnPoint.eulerAngles.z + currentAngle);
            Instantiate(bulletPrefab, spawnPoint.position, rotation);
        }

        // この処理は同期的だが、シグネチャを合わせるために UniTask.CompletedTask を返す
        await UniTask.CompletedTask;
    }
}
