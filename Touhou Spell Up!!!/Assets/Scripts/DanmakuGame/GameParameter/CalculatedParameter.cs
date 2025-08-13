using UnityEngine;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser; // mXparser用に変更
using System;

/// <summary>
/// 他のGameParameterの値やゲームの状態を元に、数式で値を動的に計算するGameParameter。
/// </summary>
[CreateAssetMenu(fileName = "CP_", menuName = "Danmaku/GameParameter/CalculatedParameter")]
public class CalculatedParameter : GameParameter<float>
{
    [Tooltip("評価する数式")]
    [SerializeField] private string _expressionString; // NCalcのExpressionと区別するためリネーム

    [Tooltip("数式内で使用する他のGameParameter")]
    [SerializeField] private List<GameParameter> _referencedParameters = new List<GameParameter>();

    // mXparser用のExpressionオブジェクトは毎回生成する
    private Expression _expression;

    public override float Value
    {
        get
        {
            if (string.IsNullOrEmpty(_expressionString))
            {
                return base.Value;
            }

            try
            {
                _expression = new Expression(_expressionString);

                // 参照パラメータを設定
                foreach (var param in _referencedParameters)
                {
                    if (param == null) continue;
                    
                    // パラメータの型に応じて値を取得し、Argumentとして設定
                    if (param is GameParameter<float> floatParam)
                    {
                        _expression.addArguments(new Argument(param.name, floatParam.Value));
                    }
                    else if (param is GameParameter<int> intParam)
                    {
                        _expression.addArguments(new Argument(param.name, intParam.Value));
                    }
                    else if (param is GameParameter<bool> boolParam)
                    {
                        _expression.addArguments(new Argument(param.name, boolParam.Value ? 1 : 0));
                    }
                    else
                    {
                        Debug.LogWarning($"Unsupported GameParameter type '{param.GetType()}' in {this.name}", this);
                    }
                }
                
                // グローバルなゲーム状態もパラメータとして設定可能（例）
                _expression.addArguments(new Argument("time", Time.time));
                _expression.addArguments(new Argument("deltaTime", Time.deltaTime));

                return (float)_expression.calculate();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to evaluate expression '{_expressionString}' in {this.name}: {e.Message}", this);
                return base.Value; // エラー時はデフォルト値を返す
            }
        }
    }

    public override void Reset()
    {
        // CalculatedParameterは通常、実行時に動的に値が決まるため、
        // Reset処理は基底クラスのものをそのまま使うか、何もしないかを選択できます。
        // ここでは一旦、基底クラスの動作（currentValueをinitialValueでリセット）に従います。
        base.Reset();
    }
}
