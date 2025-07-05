using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "RTS_", menuName = "Touhou Spell Up/Danmaku/Bullet Pattern/RotatingShot")]
public class RotatingShotPattern : ShootPatternBase
{
    [Header("基本設定")]
    [SerializeField, Range(0f, 360f)] private float startAngle = 0f;
    [SerializeField, Range(0f, 360f)] private float intervalAngle = 10f;
    [SerializeField] private float intervalTime = 0.5f;
    [SerializeField] private int shotCount = 1;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.CounterClockwise;

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


    public override async UniTask ExecuteImpl(Transform spawnPoint, Bullet bulletToUse, CancellationToken token)
    {
        if (bulletToUse == null || bulletToUse.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;
        Quaternion originalRotation = spawnPoint.rotation;

        // --- 実行開始時の角度を決定 ---
        float currentAngle;
        if (sharedAngle != null)
        {
            // SharedAngleが設定されていれば、常にその値から開始
            currentAngle = sharedAngle.Value;
        }
        else
        {
            // SharedAngleがなければ、isKeepAngleのロジックを適用
            if (!isKeepAngle || !_isInitialized)
            {
                currentAngle = startAngle;
                _isInitialized = true; // 実行したので初期化済みに
            }
            else
            {
                // 引き継ぎが有効で、初期化済みなら前回の値を復元
                currentAngle = _localCurrentAngle;
            }
        }
        // --------------------------

        for (int i = 0; i < shotCount; i++)
        {
            if (token.IsCancellationRequested) break;

            // 弾を発射する角度を決定（ループの初回は発射してから角度を足す）
            float shotAngle = currentAngle + (directionMultiplier * intervalAngle * i);

            spawnPoint.rotation = Quaternion.Euler(0, 0, shotAngle);

            var bulletInstance = Instantiate(bulletToUse.Prefab, spawnPoint.position, spawnPoint.rotation);
            var enemyBullet = bulletInstance.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.Initialize(bulletToUse.Property);
            }

            spawnPoint.rotation = originalRotation;

            if (intervalTime > 0)
            {
                await UniTask.Delay((int)(intervalTime * 1000), cancellationToken: token);
            }
        }

        // --- 次回実行のために最後の角度を保存 ---
        // この実行で回転した最終的な角度を計算
        float lastAngle = currentAngle + (directionMultiplier * intervalAngle * shotCount);

        // isKeepAngleかsharedAngleが有効な場合、次回のためにオフセットを加算して保存する
        if (sharedAngle != null)
        {
            sharedAngle.Value = lastAngle + accumulatingOffset;
        }
        else if (isKeepAngle)
        {
            _localCurrentAngle = lastAngle + accumulatingOffset;
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
        _localCurrentAngle = startAngle;
    }
}
