using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "RTS_", menuName = "Touhou Spell Up/Danmaku/Shoot/RotatingShot")]
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

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;
        Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;

        // --- 実行開始時の角度を決定 ---
        float currentAngle;
        if (sharedAngle != null)
        {
            currentAngle = sharedAngle.Value;
        }
        else
        {
            if (!isKeepAngle || !_isInitialized)
            {
                if (aimAtPlayer)
                {
                    currentAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
                }
                else
                {
                    currentAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
                }
                currentAngle += startAngle;
                _isInitialized = true;
            }
            else
            {
                currentAngle = _localCurrentAngle;
            }
        }
        // --------------------------

        for (int i = 0; ; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = baseSpawnPosition;
            if (followShooterPosition && emissionShape == null)
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            float loopStartAngle = currentAngle;
            if (alwaysAimToPlayer)
            {
                loopStartAngle = AngleUtility.GetAngleToPlayer(currentSpawnPosition) + 180f + startAngle;
            }

            float shotAngle = loopStartAngle + (directionMultiplier * intervalAngle * i);
            Quaternion rotation = Quaternion.Euler(0, 0, shotAngle);
            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, shotAngle);

            controller.InstantiateProperty(_entity, finalSpawnPosition, rotation);

            if (intervalTime > 0)
            {
                await UniTask.Delay((int)(intervalTime * 1000), cancellationToken: token);
            }
            if (shotCount > 0 && i >= shotCount - 1) // shotCountが1ならi=0で終了
            {
                break;
            }
        }

        // --- 次回実行のために最後の角度を保存 ---
        float lastAngle = currentAngle + (directionMultiplier * intervalAngle * shotCount);
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
