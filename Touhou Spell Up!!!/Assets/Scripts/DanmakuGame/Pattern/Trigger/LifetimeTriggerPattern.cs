using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// エンティティの生存時間ベースでトリガーを発火するパターン
/// </summary>
[CreateAssetMenu(fileName = "LIFETIME_", menuName = "Danmaku/Pattern/Trigger/Lifetime Trigger")]
public class LifetimeTriggerPattern : TriggerPatternBase
{
    [Header("生存時間トリガー設定")]
    [Tooltip("トリガーを発火する生存時間（秒）")]
    [SerializeField] private FloatReference _triggerLifetime = new FloatReference { useConstant = true, constantValue = 2.0f };
    
    [Tooltip("比較演算子の種類")]
    [SerializeField] private ComparisonOperatorReference _comparisonOperator = new ComparisonOperatorReference { useConstant = true, constantValue = ComparisonOperator.GreaterThanOrEqual };

    /// <summary>
    /// イベントベースの監視も併用するためリスナーを登録
    /// </summary>
    protected override void RegisterEventListeners(GameEntityController controller)
    {
        base.RegisterEventListeners(controller);
        controller.OnLifetimeChanged.AddListener(OnLifetimeChanged);
    }

    /// <summary>
    /// イベントリスナーを解除
    /// </summary>
    protected override void UnregisterEventListeners(GameEntityController controller)
    {
        base.UnregisterEventListeners(controller);
        if (controller != null)
        {
            controller.OnLifetimeChanged.RemoveListener(OnLifetimeChanged);
        }
    }

    /// <summary>
    /// 生存時間の変更イベントハンドラ
    /// </summary>
    private void OnLifetimeChanged(float newLifetime)
    {
        if (_targetController != null && CheckLifetimeCondition(newLifetime))
        {
            // イベントベースで即座にトリガー発火
            _ = FireTrigger(_targetController, _cancellationToken);
        }
    }

    /// <summary>
    /// 生存時間の条件をチェック
    /// </summary>
    protected override bool CheckTriggerCondition(GameEntityController controller)
    {
        return CheckLifetimeCondition(controller.CurrentLifeTime);
    }

    /// <summary>
    /// 生存時間の条件チェックのヘルパーメソッド
    /// </summary>
    private bool CheckLifetimeCondition(float currentLifetime)
    {
        switch (_comparisonOperator.Value)
        {
            case ComparisonOperator.Equal:
                return Mathf.Approximately(currentLifetime, _triggerLifetime.Value);
            case ComparisonOperator.NotEqual:
                return !Mathf.Approximately(currentLifetime, _triggerLifetime.Value);
            case ComparisonOperator.GreaterThan:
                return currentLifetime > _triggerLifetime.Value;
            case ComparisonOperator.GreaterThanOrEqual:
                return currentLifetime >= _triggerLifetime.Value;
            case ComparisonOperator.LessThan:
                return currentLifetime < _triggerLifetime.Value;
            case ComparisonOperator.LessThanOrEqual:
                return currentLifetime <= _triggerLifetime.Value;
            default:
                return false;
        }
    }

    /// <summary>
    /// トリガー発火時の追加処理
    /// </summary>
    protected override UniTask OnTriggerFired(GameEntityController controller, CancellationToken token)
    {
        if (_debugLog.Value)
        {
            Debug.Log($"[LifetimeTriggerPattern] Lifetime trigger fired! Current lifetime: {controller.CurrentLifeTime:F2}s, Trigger condition: {_comparisonOperator.Value} {_triggerLifetime.Value:F2}s", this);
        }
        
        return UniTask.CompletedTask;
    }
}

/// <summary>
/// 比較演算子の種類
/// </summary>
public enum ComparisonOperator
{
    Equal,              // ==
    NotEqual,           // !=
    GreaterThan,        // >
    GreaterThanOrEqual, // >=
    LessThan,           // <
    LessThanOrEqual     // <=
}
