using UnityEngine;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using System;

/// <summary>
/// mXparserの式を評価して得られたインデックスに基づき、リストから値を選択する汎用的な計算パターンのための抽象基底クラス。
/// </summary>
/// <typeparam name="T">選択する値の型</typeparam>
public abstract class ExpressionCalculatePatternBase<T> : CalculatePatternBase<T>
{
    [Header("選択肢リスト")]
    [Tooltip("計算結果のインデックスで選択される値のリスト")]
    [SerializeField] protected List<T> _options = new List<T>();

    [Header("計算式")]
    [TextArea(3, 6)]
    [Tooltip("評価する計算式。結果はリストのインデックスとして解釈されます。\n" +
             "・例: 'if(p0 > 10, 1, 0)' (p0が10より大きい場合インデックス1、そうでなければ0)")]
    [SerializeField] private string _expressionString;

    protected override T CalculateValue()
    {
        if (_options.Count == 0)
        {
            Debug.LogWarning($"Options list is empty in {this.name}", this);
            return default;
        }

        // 式を評価してインデックスを計算
        int index = (int)EvaluateExpression();

        // インデックスの範囲チェック
        if (index >= 0 && index < _options.Count)
        {
            return _options[index];
        }

        Debug.LogWarning($"Calculated index {index} is out of range for options in {this.name}", this);
        return default;
    }

    /// <summary>
    /// mXparserを用いて式を評価する
    /// </summary>
    /// <returns>計算結果のdouble値</returns>
    private double EvaluateExpression()
    {
        if (string.IsNullOrEmpty(_expressionString))
        {
            Debug.LogWarning($"Expression string is not set in {this.name}", this);
            return 0;
        }

        try
        {
            Expression expression = new Expression(_expressionString);

            // 参照パラメータを引数として設定
            for (int i = 0; i < _referencedParameters.Count; i++)
            {
                var param = _referencedParameters[i];
                if (param == null) continue;

                double value = 0;
                if (param is GameParameter<float> floatParam) 
                    value = floatParam.Value;
                else if (param is GameParameter<int> intParam) 
                    value = intParam.Value;
                else if (param is GameParameter<bool> boolParam) 
                    value = boolParam.Value ? 1 : 0;
                else 
                    continue;

                expression.addArguments(new Argument(param.name, value));
                expression.addArguments(new Argument($"p{i}", value));
            }

            // グローバルなゲーム状態も利用可能にする
            expression.addArguments(new Argument("time", Time.time));
            expression.addArguments(new Argument("deltaTime", Time.deltaTime));

            return expression.calculate();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to evaluate expression '{_expressionString}' in {this.name}: {e.Message}", this);
            return 0; // エラー時は0を返す
        }
    }
}
