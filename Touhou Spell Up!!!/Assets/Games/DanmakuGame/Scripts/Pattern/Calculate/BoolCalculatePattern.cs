using UnityEngine;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// 数式評価によりbool値を計算し、GameParameterに代入するパターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Bool_", menuName = "Danmaku/Pattern/Calculate/Bool")]
public class BoolCalculatePattern : CalculatePatternBase<bool>
{
    [TextArea(3, 10)]
    [Tooltip("評価する数式。\n" +
             "・結果が0以外ならtrue、0ならfalseと解釈されます。\n" +
             "・if(cond, true_val, false_val) 形式が利用できます。\n" +
             "・例: 'if(p0 > 100, 1, 0)'")]
    [SerializeField] private string _expressionString;

    protected override bool CalculateValue()
    {
        if (string.IsNullOrEmpty(_expressionString))
        {
            Debug.LogWarning($"ExpressionString is not set in {this.name}", this);
            return _targetParameter.Value;
        }

        Expression expression = new Expression(_expressionString);

        for (int i = 0; i < _referencedParameters.Count; i++)
        {
            var param = _referencedParameters[i];
            if (param == null) continue;

            double value = 0;
            if (param is GameParameter<float> floatParam) value = floatParam.Value;
            else if (param is GameParameter<int> intParam) value = intParam.Value;
            else if (param is GameParameter<bool> boolParam) value = boolParam.Value ? 1 : 0;
            else continue;
            
            expression.addArguments(new Argument(param.name, value));
            expression.addArguments(new Argument($"p{i}", value));
        }

        expression.addArguments(new Argument("time", Time.time));
        expression.addArguments(new Argument("deltaTime", Time.deltaTime));

        // 計算結果が0でなければtrue、0ならfalseとする
        return expression.calculate() != 0;
    }
}
