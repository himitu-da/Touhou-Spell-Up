using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// GameParameterの値変化ベースでトリガーを発火するパターン
/// </summary>
[CreateAssetMenu(fileName = "PARAMETER_", menuName = "Danmaku/Pattern/Trigger/Parameter Trigger")]
public class ParameterTriggerPattern : TriggerPatternBase
{
    [Header("パラメータトリガー設定")]
    [Tooltip("監視対象のGameParameter")]
    [SerializeField] private GameParameterReference _targetParameter;
    
    [Tooltip("比較する値の種類")]
    [SerializeField] private ParameterValueType _valueType = ParameterValueType.ConstantFloat;
    
    [Header("比較値設定（valueTypeに応じて使用）")]
    [Tooltip("Float値での比較（ConstantFloatの場合）")]
    [SerializeField] private FloatReference _floatValue = new FloatReference { useConstant = true, constantValue = 0f };
    
    [Tooltip("Int値での比較（ConstantIntの場合）")]
    [SerializeField] private IntReference _intValue = new IntReference { useConstant = true, constantValue = 0 };
    
    [Tooltip("Bool値での比較（ConstantBoolの場合）")]
    [SerializeField] private BoolReference _boolValue = new BoolReference { useConstant = true, constantValue = true };
    
    [Tooltip("別のGameParameterとの比較（ParameterReferenceの場合）")]
    [SerializeField] private GameParameterReference _compareParameter;
    
    [Tooltip("比較演算子の種類")]
    [SerializeField] private ComparisonOperatorReference _comparisonOperator = new ComparisonOperatorReference { useConstant = true, constantValue = ComparisonOperator.Equal };
    
    [Tooltip("パラメータ値の変化を監視するか（falseの場合は現在値のみチェック）")]
    [SerializeField] private BoolReference _monitorChanges = new BoolReference { useConstant = true, constantValue = true };

    private object _lastValue;
    private bool _isFirstCheck = true;

    /// <summary>
    /// 監視開始時に初期値を記録
    /// </summary>
    protected override async UniTask MonitorTriggerCondition(GameEntityController controller, CancellationToken token)
    {
        if (_targetParameter?.Value != null)
        {
            _lastValue = GetParameterValue(_targetParameter.Value);
            _isFirstCheck = true;
            
            if (_debugLog.Value)
                Debug.Log($"[ParameterTriggerPattern] Started monitoring parameter: {_targetParameter.Value.name}, initial value: {_lastValue}", this);
        }

        await base.MonitorTriggerCondition(controller, token);
    }

    /// <summary>
    /// パラメータの条件をチェック
    /// </summary>
    protected override bool CheckTriggerCondition(GameEntityController controller)
    {
        if (_targetParameter?.Value == null)
        {
            if (_debugLog.Value)
                Debug.LogWarning($"[ParameterTriggerPattern] Target parameter is null", this);
            return false;
        }

        object currentValue = GetParameterValue(_targetParameter.Value);
        
        // 変化監視が有効で、初回チェックでない場合は値の変化もチェック
        if (_monitorChanges.Value && !_isFirstCheck)
        {
            bool hasChanged = !Equals(currentValue, _lastValue);
            if (!hasChanged)
            {
                return false; // 値が変化していない場合はfalse
            }
        }
        
        _isFirstCheck = false;
        _lastValue = currentValue;

        // 条件チェック
        object compareValue = GetCompareValue();
        bool conditionMet = CheckValueCondition(currentValue, compareValue);
        
        if (_debugLog.Value && conditionMet)
            Debug.Log($"[ParameterTriggerPattern] Parameter condition met! Current: {currentValue}, Compare: {compareValue}, Operator: {_comparisonOperator.Value}", this);
            
        return conditionMet;
    }

    /// <summary>
    /// GameParameterから値を取得
    /// </summary>
    private object GetParameterValue(GameParameter parameter)
    {
        if (parameter is FloatParameter floatParam)
            return floatParam.Value;
        else if (parameter is IntParameter intParam)
            return intParam.Value;
        else if (parameter is BoolParameter boolParam)
            return boolParam.Value;
        else if (parameter is Vector3Parameter vec3Param)
            return vec3Param.Value;
        else
            return null;
    }

    /// <summary>
    /// 比較値を取得
    /// </summary>
    private object GetCompareValue()
    {
        switch (_valueType)
        {
            case ParameterValueType.ConstantFloat:
                return _floatValue.Value;
            case ParameterValueType.ConstantInt:
                return _intValue.Value;
            case ParameterValueType.ConstantBool:
                return _boolValue.Value;
            case ParameterValueType.ParameterReference:
                return _compareParameter?.Value != null ? GetParameterValue(_compareParameter.Value) : null;
            default:
                return null;
        }
    }

    /// <summary>
    /// 値の条件チェック
    /// </summary>
    private bool CheckValueCondition(object currentValue, object compareValue)
    {
        if (currentValue == null || compareValue == null)
            return false;

        // 型が異なる場合は変換を試行
        if (currentValue.GetType() != compareValue.GetType())
        {
            compareValue = ConvertValue(compareValue, currentValue.GetType());
            if (compareValue == null) return false;
        }

        // Bool型の場合は等価比較のみ
        if (currentValue is bool boolCurrent && compareValue is bool boolCompare)
        {
            switch (_comparisonOperator.Value)
            {
                case ComparisonOperator.Equal:
                    return boolCurrent == boolCompare;
                case ComparisonOperator.NotEqual:
                    return boolCurrent != boolCompare;
                default:
                    return false;
            }
        }

        // 数値型の場合は各種比較をサポート
        if (currentValue is float floatCurrent && compareValue is float floatCompare)
        {
            return CompareNumbers(floatCurrent, floatCompare);
        }
        else if (currentValue is int intCurrent && compareValue is int intCompare)
        {
            return CompareNumbers(intCurrent, intCompare);
        }

        // その他の型は等価比較のみ
        switch (_comparisonOperator.Value)
        {
            case ComparisonOperator.Equal:
                return currentValue.Equals(compareValue);
            case ComparisonOperator.NotEqual:
                return !currentValue.Equals(compareValue);
            default:
                return false;
        }
    }

    /// <summary>
    /// 数値の比較
    /// </summary>
    private bool CompareNumbers(float current, float compare)
    {
        switch (_comparisonOperator.Value)
        {
            case ComparisonOperator.Equal:
                return Mathf.Approximately(current, compare);
            case ComparisonOperator.NotEqual:
                return !Mathf.Approximately(current, compare);
            case ComparisonOperator.GreaterThan:
                return current > compare;
            case ComparisonOperator.GreaterThanOrEqual:
                return current >= compare;
            case ComparisonOperator.LessThan:
                return current < compare;
            case ComparisonOperator.LessThanOrEqual:
                return current <= compare;
            default:
                return false;
        }
    }

    /// <summary>
    /// 値の型変換
    /// </summary>
    private object ConvertValue(object value, System.Type targetType)
    {
        try
        {
            if (targetType == typeof(float) && value is int intValue)
                return (float)intValue;
            else if (targetType == typeof(int) && value is float floatValue)
                return Mathf.RoundToInt(floatValue);
            else
                return System.Convert.ChangeType(value, targetType);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// トリガー発火時の追加処理
    /// </summary>
    protected override UniTask OnTriggerFired(GameEntityController controller, CancellationToken token)
    {
        if (_debugLog.Value)
        {
            object currentValue = GetParameterValue(_targetParameter.Value);
            object compareValue = GetCompareValue();
            Debug.Log($"[ParameterTriggerPattern] Parameter trigger fired! Parameter: {_targetParameter.Value.name}, Current: {currentValue}, Compare: {compareValue}", this);
        }
        
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// トリガーリセット時に監視状態もリセット
    /// </summary>
    public override void ResetTrigger()
    {
        base.ResetTrigger();
        _isFirstCheck = true;
        _lastValue = null;
    }
}

/// <summary>
/// パラメータ値の種類
/// </summary>
public enum ParameterValueType
{
    ConstantFloat,      // 固定Float値
    ConstantInt,        // 固定Int値
    ConstantBool,       // 固定Bool値
    ParameterReference  // 別のGameParameterとの比較
}
