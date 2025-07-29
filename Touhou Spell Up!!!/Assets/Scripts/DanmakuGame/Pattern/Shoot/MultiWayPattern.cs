using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using TouhouSpellUp.Danmaku;

[CreateAssetMenu(fileName = "NWAY_", menuName = "Danmaku/Pattern/Shoot/Multi-Way")]
public class MultiWayPattern : ShootPatternBase
{
    [Header("N-Way弾の設定")]
    [SerializeField, Range(1, 100)] private int wayCount = 5;
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

    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        // --- 基準位置と基準角度をEmissionDataから決定 ---
        // IMovableの回転を考慮したローカル位置をワールド位置に変換
        Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;
        float baseAngle = controller.transform.eulerAngles.z;

        // 全方位でないなら自機外しはfalse
        avoidAtPlayer = allRound && avoidAtPlayer;

        // 回転方向に応じて角度の増減を決定（反時計回りなら+1、時計回りなら-1）
        float directionMultiplier = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;

        // --- 実行開始時の中央角度を決定 ---
        float centerAngle;
        if (sharedAngle != null)
        {
            centerAngle = sharedAngle.Value;
        }
        else
        {
            if (!isKeepAngle || !_isInitialized)
            {
                // aimAtPlayerが有効なら、各射出点から自機を狙う
                if (aimAtPlayer)
                {
                    centerAngle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
                }
                else
                {
                    // そうでなければ、EmissionDataの角度を基準にする
                    centerAngle = baseAngle + emissionData.localAngle;
                }
                centerAngle += directionOffset; // 共通のオフセットを加算
                _isInitialized = true;
            }
            else
            {
                centerAngle = _localCurrentAngle;
            }
        }
        // --------------------------

        float finalAngle = allRound ? 360f : totalAngle;
        float startAngle = -finalAngle / 2 * directionMultiplier;
        float angleStep = allRound ? finalAngle / wayCount : ((wayCount > 1) ? finalAngle / (wayCount - 1) : 0f);

        for (int i = 0; i < wayCount; i++)
        {
            if (token.IsCancellationRequested) break;

            Vector3 currentSpawnPosition = baseSpawnPosition;
            // 射撃中にシューターに追従する場合、基準位置を更新 (EmissionShape使用時は各点が基準なので追従は複雑)
            if (followShooterPosition && emissionShape == null)
            {
                currentSpawnPosition = GetSpawnPosition(controller);
            }

            float loopCenterAngle = centerAngle;
            // 常に自機を狙う場合、中央の角度を更新
            if (alwaysAimToPlayer && sharedAngle == null && !isKeepAngle)
            {
                loopCenterAngle = AngleUtility.GetAngleToPlayer(currentSpawnPosition) + 180f + directionOffset;
            }

            float currentAngle = loopCenterAngle + (allRound ? 0 : startAngle) + (avoidAtPlayer ? angleStep / 2 : 0) + (angleStep * i * directionMultiplier);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(currentSpawnPosition, currentAngle);
            controller.InstantiateProperty(_entity, finalSpawnPosition, rotation);

            // N-Way弾を順次発射する機能はShootPatternBaseのsequentialとは意味が違うため、ここでは一旦無効化
            // もしN-Way弾自体の順次発射が必要なら、別途interval設定が必要
        }

        // --- 次回実行のために最後の角度を保存 ---
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
