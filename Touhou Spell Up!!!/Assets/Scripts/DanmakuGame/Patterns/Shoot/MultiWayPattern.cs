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

    [Header("N-Way弾の角度")]
    [Tooltip("全方位で自機外し")]
    [SerializeField] private bool avoidAtPlayer = false;

    [Header("角度の共有・維持設定")]
    [Tooltip("（任意）設定すると、このアセットの値を共有の角度として使用します。")]
    [SerializeField] private SharedAngle sharedAngle;
    [Tooltip("SharedAngleが未設定の場合に、このパターン内でのみ角度を引き継ぐか。")]
    [SerializeField] private bool isKeepAngle = false;
    [Tooltip("実行ごとに加算されていくオフセット角度。isKeepAngle=trueかSharedAngleが設定されている場合のみ有効。")]
    [SerializeField] private float accumulatingOffset = 0f;

    // sharedAngleを使わない場合に、isKeepAngleを実現するための内部状態
    private float _localCurrentAngle;
    private bool _isInitialized = false;

    public override async UniTask ExecuteShoot(Shooter shooter, CancellationToken token)
    {
        if (_bullet == null || _bullet.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        // 全方位でないなら自機外しはfalse
        avoidAtPlayer = allRound && avoidAtPlayer;

        // 回転方向に応じて角度の増減を決定（反時計回りなら+1、時計回りなら-1）
        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;

        Vector3 baseSpawnPosition = GetSpawnPosition(shooter);

        // --- 実行開始時の中央角度を決定 ---
        float centerAngle;
        if (sharedAngle != null)
        {
            // SharedAngleが設定されていれば、常にその値から開始
            centerAngle = sharedAngle.Value;
        }
        else
        {
            // SharedAngleがなければ、isKeepAngleのロジックを適用
            if (!isKeepAngle || !_isInitialized)
            {
                centerAngle = GetAimAngle(shooter, baseSpawnPosition) + directionOffset;
                _isInitialized = true; // 実行したので初期化済みに
            }
            else
            {
                // 引き継ぎが有効で、初期化済みなら前回の値を復元
                centerAngle = _localCurrentAngle;
            }
        }
        // --------------------------

        float finalAngle = allRound ? 360f : totalAngle;

        float startAngle = -finalAngle / 2 * directionMultiplier;
        // 全方位の場合は最後の弾が最初と重ならないようにする
        float angleStep = allRound ? finalAngle / wayCount : ((wayCount > 1) ? finalAngle / (wayCount - 1) : 0f);

        for (int i = 0; i < wayCount; i++)
        {
            if (token.IsCancellationRequested) break;

            // 射撃中にシューターに追従する場合、基準位置を更新
            if (followShooterPosition)
            {
                baseSpawnPosition = GetSpawnPosition(shooter);
            }

            // 常に自機を狙う場合、中央の角度を更新
            // ただし、角度共有・維持が有効な場合は、初回の角度決定にのみ影響し、ループ中の更新は行わない
            if (alwaysAimToPlayer && sharedAngle == null && !isKeepAngle)
            {
                centerAngle = GetAimAngle(shooter, baseSpawnPosition) + directionOffset;
            }

            // 全方位の場合、startAngleは不要（0度から開始するため）
            // 全方位の自機外しの場合、angleStepの半分だけずらす
            // directionMultiplierで回転方向を制御
            float currentAngle = centerAngle + (allRound ? 0 : startAngle) + (avoidAtPlayer ? angleStep / 2 : 0) + (angleStep * i * directionMultiplier);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            // 最終的な発射位置を計算
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, currentAngle);

            shooter.InstantiateBullet(_bullet, finalSpawnPosition, rotation);

            // intervalだけ待つ
            if (interval > 0)
            {
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
            }
        }

        // --- 次回実行のために最後の角度を保存 ---
        // isKeepAngleかsharedAngleが有効な場合、次回のためにオフセットを加算して保存する
        if (sharedAngle != null)
        {
            sharedAngle.Value = centerAngle + accumulatingOffset;
        }
        else if (isKeepAngle)
        {
            _localCurrentAngle = centerAngle + accumulatingOffset;
        }
        // ------------------------------------

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 外部から内部状態をリセットするためのメソッド
    /// </summary>
    public void ResetState()
    {
        _isInitialized = false;
        _localCurrentAngle = 0f; // 初期値は0で良いでしょう
    }
}
