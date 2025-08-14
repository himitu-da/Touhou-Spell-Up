using UnityEngine;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// 数式評価によりint値を計算し、GameParameterに代入するパターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Int_", menuName = "Danmaku/Pattern/Calculate/Int")]
public class IntCalculatePattern : CalculatePatternBase<int>
{
    [TextArea(3, 10)]
    [Tooltip("評価する数式。\n" +
             "・計算結果は最も近い整数に丸められます。\n" +
             "・例: '20 - p0 / 20'")]
    [SerializeField] private string _expressionString;

    protected override int CalculateValue()
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

        // 計算結果を最も近い整数に丸める
        return (int)System.Math.Round(expression.calculate());
    }
}
