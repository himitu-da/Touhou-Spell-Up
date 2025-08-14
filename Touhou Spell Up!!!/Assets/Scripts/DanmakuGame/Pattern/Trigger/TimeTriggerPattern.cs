using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 指定された時間経過後にトリガーを発火する時間ベースのトリガーパターン
/// </summary>
[CreateAssetMenu(fileName = "TIME_", menuName = "Danmaku/Pattern/Trigger/Time Trigger")]
public class TimeTriggerPattern : TriggerPatternBase
{
    [Header("時間トリガー設定")]
    [Tooltip("トリガーを発火する時間（秒）")]
    [SerializeField] private FloatReference _triggerTime = new FloatReference { useConstant = true, constantValue = 3.0f };
    
    [Tooltip("開始時刻からの経過時間で判定するか（falseの場合はゲーム時間）")]
    [SerializeField] private BoolReference _useRealTime = new BoolReference { useConstant = true, constantValue = false };

    private float _startTime;

    /// <summary>
    /// 監視開始時に開始時刻を記録
    /// </summary>
    protected override async UniTask MonitorTriggerCondition(GameEntityController controller, CancellationToken token)
    {
        // 開始時刻を記録
        _startTime = _useRealTime.Value ? Time.realtimeSinceStartup : Time.time;
        
        if (_debugLog.Value)
            Debug.Log($"[TimeTriggerPattern] Started monitoring at time: {_startTime}, trigger at: {_triggerTime.Value}", this);

        // 基底クラスの監視ループを開始
        await base.MonitorTriggerCondition(controller, token);
    }

    /// <summary>
    /// 指定時間が経過したかをチェック
    /// </summary>
    protected override bool CheckTriggerCondition(GameEntityController controller)
    {
        float currentTime = _useRealTime.Value ? Time.realtimeSinceStartup : Time.time;
        float elapsedTime = currentTime - _startTime;
        
        return elapsedTime >= _triggerTime.Value;
    }

    /// <summary>
    /// トリガー発火時の追加処理
    /// </summary>
    protected override UniTask OnTriggerFired(GameEntityController controller, CancellationToken token)
    {
        if (_debugLog.Value)
        {
            float currentTime = _useRealTime.Value ? Time.realtimeSinceStartup : Time.time;
            float elapsedTime = currentTime - _startTime;
            Debug.Log($"[TimeTriggerPattern] Time trigger fired! Elapsed time: {elapsedTime:F2}s", this);
        }
        
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// トリガーリセット時に開始時刻もリセット
    /// </summary>
    public override void ResetTrigger()
    {
        base.ResetTrigger();
        _startTime = _useRealTime.Value ? Time.realtimeSinceStartup : Time.time;
    }
}
