using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using org.mariuszgromada.math.mxparser;
using System;

/// <summary>
/// 数式評価によりfloat値を計算し、GameParameterに代入するパターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Float_", menuName = "Danmaku/Pattern/Calculate/Float")]
public class FloatCalculatePattern : CalculatePatternBase<float>
{
    [TextArea(3, 10)]
    [Tooltip("評価する数式。\n" +
             "■ 使用可能な記法 (mXparserライブラリ):\n" +
             "・基本演算子: +, -, *, /, ^ (べき乗)\n" +
             "・関数: sin(), cos(), tan(), sqrt(), abs(), min(), max(), etc.\n" +
             "・変数: 'Referenced Parameters'で指定したパラメータ名、またはp0, p1..形式のインデックス。'time', 'deltaTime'も使用可能\n" +
             "・例: 'Angle + 90', 'p0 + 90', 'sin(time * p1) * 100'")]
    [SerializeField] private string _expressionString;

    protected override float CalculateValue()
    {
        if (string.IsNullOrEmpty(_expressionString))
        {
            Debug.LogWarning($"ExpressionString is not set in {this.name}", this);
            return _targetParameter.Value; // 式が空の場合は現在の値をそのまま返す
        }

        Expression expression = new Expression(_expressionString);

        // 参照パラメータを設定
        for (int i = 0; i < _referencedParameters.Count; i++)
        {
            var param = _referencedParameters[i];
            if (param == null) continue;

            double value = 0;
            if (param is GameParameter<float> floatParam)
            {
                value = floatParam.Value;
            }
            else if (param is GameParameter<int> intParam)
            {
                value = intParam.Value;
            }
            else if (param is GameParameter<bool> boolParam)
            {
                value = boolParam.Value ? 1 : 0;
            }
            else
            {
                Debug.LogWarning($"Unsupported GameParameter type '{param.GetType()}' in {this.name}", this);
                continue;
            }

            // 名前での参照とインデックス(p0, p1...)での参照を両方追加
            expression.addArguments(new Argument(param.name, value));
            expression.addArguments(new Argument($"p{i}", value));
        }

        // グローバルなゲーム状態もパラメータとして設定
        expression.addArguments(new Argument("time", Time.time));
        expression.addArguments(new Argument("deltaTime", Time.deltaTime));

        return (float)expression.calculate();
    }
}
