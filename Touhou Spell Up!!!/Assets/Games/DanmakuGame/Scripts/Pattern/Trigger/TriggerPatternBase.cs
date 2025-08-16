using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;

/// <summary>
/// イベント駆動のトリガーパターンの基底クラス
/// 各FixedUpdateで条件をチェックし、条件が満たされたときに実行するパターンを定義する
/// </summary>
[System.Obsolete("TriggerPatternBase is deprecated. Use PatternBase with start/end condition expressions instead.", false)]
public abstract class TriggerPatternBase : PatternBase
{
    [Header("トリガー設定")]
    [Tooltip("トリガーが発火したときに実行するパターン")]
    [SerializeField] protected PatternBaseReference _triggerPattern;
    
    [Tooltip("トリガーを一度だけ発火させるか（falseの場合は条件を満たすたびに発火）")]
    [SerializeField] protected BoolReference _fireOnce = new BoolReference { useConstant = true, constantValue = true };
    
    [Tooltip("トリガーの有効/無効")]
    [SerializeField] protected BoolReference _enabled = new BoolReference { useConstant = true, constantValue = true };

    [Header("デバッグ")]
    [Tooltip("トリガー発火時にログを出力するか")]
    [SerializeField] protected BoolReference _debugLog = new BoolReference { useConstant = true, constantValue = false };

    // 内部状態
    protected bool _hasTriggered = false;
    protected GameEntityController _targetController;
    protected CancellationToken _cancellationToken;

    /// <summary>
    /// PatternBaseのExecuteImplを実装
    /// GameEntityControllerに対してトリガー監視を開始する
    /// </summary>
    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (!_enabled.Value)
        {
            if (_debugLog.Value)
                Debug.Log($"[{GetType().Name}] Trigger is disabled", this);
            return;
        }

        _targetController = controller;
        _cancellationToken = token;
        _hasTriggered = false;

        // イベントリスナーを登録
        RegisterEventListeners(controller);

        try
        {
            // FixedUpdateループでの監視を開始
            await MonitorTriggerCondition(controller, token);
        }
        finally
        {
            // イベントリスナーを解除
            UnregisterEventListeners(controller);
        }
    }

    /// <summary>
    /// MovementStateを受け取るExecuteImplのオーバーロード
    /// TriggerPatternはGameEntityControllerが必要なため、警告を出して何もしない
    /// </summary>
    public override UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        Debug.LogWarning($"{GetType().Name} requires GameEntityController to function properly. Use Execute(GameEntityController, CancellationToken) instead.", this);
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// FixedUpdateループでトリガー条件を監視する
    /// </summary>
    protected virtual async UniTask MonitorTriggerCondition(GameEntityController controller, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 一度発火済みで、一度だけ発火の設定なら監視を終了
            if (_hasTriggered && _fireOnce.Value)
            {
                break;
            }

            // トリガー条件をチェック
            if (CheckTriggerCondition(controller))
            {
                await FireTrigger(controller, token);
                
                if (_fireOnce.Value)
                {
                    break;
                }
            }

            // FixedUpdateタイミングで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }

    /// <summary>
    /// トリガーを発火する
    /// </summary>
    protected virtual async UniTask FireTrigger(GameEntityController controller, CancellationToken token)
    {
        if (_hasTriggered && _fireOnce.Value)
            return;

        _hasTriggered = true;

        if (_debugLog.Value)
            Debug.Log($"[{GetType().Name}] Trigger fired for {controller.name}", this);

        // 設定されたパターンを実行
        if (_triggerPattern != null && _triggerPattern.Value != null)
        {
            await _triggerPattern.Value.Execute(controller, token);
        }

        // サブクラス固有の発火処理
        await OnTriggerFired(controller, token);
    }

    /// <summary>
    /// イベントリスナーを登録する（サブクラスでオーバーライド）
    /// </summary>
    protected virtual void RegisterEventListeners(GameEntityController controller)
    {
        // サブクラスで実装
    }

    /// <summary>
    /// イベントリスナーを解除する（サブクラスでオーバーライド）
    /// </summary>
    protected virtual void UnregisterEventListeners(GameEntityController controller)
    {
        // サブクラスで実装
    }

    /// <summary>
    /// トリガー条件をチェックする（サブクラスで実装）
    /// </summary>
    /// <returns>トリガー条件が満たされた場合true</returns>
    protected abstract bool CheckTriggerCondition(GameEntityController controller);

    /// <summary>
    /// トリガー発火時のサブクラス固有処理（オプション）
    /// </summary>
    protected virtual UniTask OnTriggerFired(GameEntityController controller, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// トリガーをリセットする（外部から呼び出し可能）
    /// </summary>
    public virtual void ResetTrigger()
    {
        _hasTriggered = false;
        
        if (_debugLog.Value)
            Debug.Log($"[{GetType().Name}] Trigger reset", this);
    }

    /// <summary>
    /// トリガーの有効/無効を切り替える（外部から呼び出し可能）
    /// </summary>
    public virtual void SetEnabled(bool enabled)
    {
        if (_enabled != null)
        {
            // GameParameterReferenceが定数値を使用している場合のみ設定可能
            if (_enabled.useConstant)
            {
                _enabled.constantValue = enabled;
            }
            else
            {
                Debug.LogWarning($"[{GetType().Name}] Cannot set enabled state - _enabled is using a parameter reference", this);
            }
        }
        
        if (_debugLog.Value)
            Debug.Log($"[{GetType().Name}] Trigger {(enabled ? "enabled" : "disabled")}", this);
    }

    /// <summary>
    /// トリガーが発火済みかどうかを取得
    /// </summary>
    public bool HasTriggered => _hasTriggered;

    /// <summary>
    /// トリガーが有効かどうかを取得
    /// </summary>
    public bool IsEnabled => _enabled?.Value ?? true;
}
