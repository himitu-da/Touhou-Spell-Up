using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "NWAY_", menuName = "Touhou Spell Up/Danmaku/Shoot/Multi-Way")]
public class MultiWayPattern : ShootPatternBase
{
    [Header("N-Way弾の設定")]
    [SerializeField, Range(1, 100)] private int wayCount = 5;
    [SerializeField, Range(0f, 20f)] private float interval = 0.5f;
    [SerializeField, Range(0f, 360f)] private float totalAngle = 90f;
    [SerializeField] private bool allRound;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.CounterClockwise;

    [Header("自機の方向を狙う")]
    [Tooltip("奇数弾なら自機狙い、偶数弾なら自機外し")]
    [SerializeField] private bool aimAtPlayer = false;
    [Header("全方位で自機外し")]
    [SerializeField] private bool avoidAtPlayer = false; 

    public override async UniTask ExecuteShoot(Shooter shooter, Bullet bulletToUse, CancellationToken token)
    {
        if (bulletToUse == null || bulletToUse.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        // 全方位でないなら自機外しはfalse
        avoidAtPlayer = allRound && avoidAtPlayer;

        // 回転方向に応じて角度の増減を決定（反時計回りなら+1、時計回りなら-1）
        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;

        Vector3 spawnPosition = GetSpawnPosition(shooter);

        float centerAngle = shooter.transform.eulerAngles.z;
        if (aimAtPlayer)
        {
            centerAngle = AngleUtility.GetAngleToPlayer(spawnPosition);
        }
        centerAngle += directionOffset;

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2 * directionMultiplier;
        // 全方位の場合は最後の弾が最初と重ならないようにする
        float angleStep = allRound ? finalAngle / wayCount : ((wayCount > 1) ? finalAngle / (wayCount - 1) : 0f);

        for (int i = 0; i < wayCount; i++)
        {
            if (token.IsCancellationRequested) break;

            // 全方位の場合、startAngleは不要（0度から開始するため）
            // 全方位の自機外しの場合、angleStepの半分だけずらす
            // directionMultiplierで回転方向を制御
            float currentAngle = centerAngle + (allRound ? 0 : startAngle) + (avoidAtPlayer ? angleStep / 2 : 0) + (angleStep * i * directionMultiplier);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            shooter.InstantiateBullet(bulletToUse, spawnPosition, rotation);

            // intervalだけ待つ
            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }
        }

        await UniTask.CompletedTask;
    }
}
