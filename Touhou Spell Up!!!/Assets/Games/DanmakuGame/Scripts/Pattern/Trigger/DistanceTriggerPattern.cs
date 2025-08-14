using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 距離ベースでトリガーを発火するパターン
/// </summary>
[CreateAssetMenu(fileName = "DISTANCE_", menuName = "Danmaku/Pattern/Trigger/Distance Trigger")]
public class DistanceTriggerPattern : TriggerPatternBase
{
    [Header("距離トリガー設定")]
    [Tooltip("基準点の種類")]
    [SerializeField] private DistanceReferenceType _referenceType = DistanceReferenceType.Player;
    
    [Tooltip("基準点がCustomの場合の固定座標")]
    [SerializeField] private Vector3Reference _customPosition = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };
    
    [Tooltip("基準点がTargetObjectの場合の対象オブジェクト")]
    [SerializeField] private GameObjectReference _targetObject;
    
    [Tooltip("トリガーを発火する距離")]
    [SerializeField] private FloatReference _triggerDistance = new FloatReference { useConstant = true, constantValue = 5.0f };
    
    [Tooltip("比較演算子の種類")]
    [SerializeField] private ComparisonOperatorReference _comparisonOperator = new ComparisonOperatorReference { useConstant = true, constantValue = ComparisonOperator.LessThanOrEqual };
    
    [Tooltip("2D距離計算を使用するか（falseの場合は3D距離）")]
    [SerializeField] private BoolReference _use2DDistance = new BoolReference { useConstant = true, constantValue = true };

    /// <summary>
    /// 基準点との距離をチェック
    /// </summary>
    protected override bool CheckTriggerCondition(GameEntityController controller)
    {
        Vector3 referencePosition = GetReferencePosition();
        if (referencePosition == Vector3.zero && _referenceType != DistanceReferenceType.Custom)
        {
            // 基準点が取得できない場合はfalse
            return false;
        }

        Vector3 currentPosition = controller.transform.position;
        float distance = CalculateDistance(currentPosition, referencePosition);

        return CheckDistanceCondition(distance);
    }

    /// <summary>
    /// 基準点の座標を取得
    /// </summary>
    private Vector3 GetReferencePosition()
    {
        switch (_referenceType)
        {
            case DistanceReferenceType.Player:
                var playerTransform = PlayerUtility.GetPlayerTransform();
                return playerTransform != null ? playerTransform.position : Vector3.zero;
                
            case DistanceReferenceType.Custom:
                return _customPosition.Value;
                
            case DistanceReferenceType.TargetObject:
                return _targetObject != null && _targetObject.Value != null ? 
                       _targetObject.Value.transform.position : Vector3.zero;
                
            case DistanceReferenceType.WorldOrigin:
                return Vector3.zero;
                
            default:
                return Vector3.zero;
        }
    }

    /// <summary>
    /// 距離を計算
    /// </summary>
    private float CalculateDistance(Vector3 pos1, Vector3 pos2)
    {
        if (_use2DDistance.Value)
        {
            Vector2 pos1_2D = new Vector2(pos1.x, pos1.y);
            Vector2 pos2_2D = new Vector2(pos2.x, pos2.y);
            return Vector2.Distance(pos1_2D, pos2_2D);
        }
        else
        {
            return Vector3.Distance(pos1, pos2);
        }
    }

    /// <summary>
    /// 距離の条件チェック
    /// </summary>
    private bool CheckDistanceCondition(float currentDistance)
    {
        switch (_comparisonOperator.Value)
        {
            case ComparisonOperator.Equal:
                return Mathf.Approximately(currentDistance, _triggerDistance.Value);
            case ComparisonOperator.NotEqual:
                return !Mathf.Approximately(currentDistance, _triggerDistance.Value);
            case ComparisonOperator.GreaterThan:
                return currentDistance > _triggerDistance.Value;
            case ComparisonOperator.GreaterThanOrEqual:
                return currentDistance >= _triggerDistance.Value;
            case ComparisonOperator.LessThan:
                return currentDistance < _triggerDistance.Value;
            case ComparisonOperator.LessThanOrEqual:
                return currentDistance <= _triggerDistance.Value;
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
            Vector3 referencePosition = GetReferencePosition();
            float currentDistance = CalculateDistance(controller.transform.position, referencePosition);
            Debug.Log($"[DistanceTriggerPattern] Distance trigger fired! Current distance: {currentDistance:F2}, Reference: {_referenceType}, Condition: {_comparisonOperator.Value} {_triggerDistance.Value:F2}", this);
        }
        
        return UniTask.CompletedTask;
    }
}

/// <summary>
/// 距離の基準点の種類
/// </summary>
public enum DistanceReferenceType
{
    Player,         // プレイヤーとの距離
    Custom,         // カスタム座標との距離
    TargetObject,   // 指定されたオブジェクトとの距離
    WorldOrigin     // ワールド原点との距離
}
