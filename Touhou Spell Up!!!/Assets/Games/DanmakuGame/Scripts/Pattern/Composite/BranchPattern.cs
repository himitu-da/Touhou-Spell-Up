using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using System;

/// <summary>
/// 条件分岐を表現する構造体。
/// 条件式とそれに対応するパターンをペアにして保持する。
/// </summary>
[System.Serializable]
public struct ConditionBranch
{
    [Header("条件")]
    [TextArea(2, 5)]
    [Tooltip("評価する条件式。\n" +
             "・結果が0以外ならtrue、0ならfalseと解釈されます。\n" +
             "・例: 'p0 > 100' (p0が100より大きい場合)\n" +
             "・例: 'time > 10 && p1 < 50' (時間が10秒を超え、かつp1が50未満)")]
    public string conditionExpression;

    [Header("実行パターン")]
    [Tooltip("条件がtrueの場合に実行するパターン")]
    public PatternBaseReference pattern;

    [Header("説明")]
    [Tooltip("この分岐の説明（任意、エディタでの識別用）")]
    public string description;
}

/// <summary>
/// 複数の条件式を順番に評価して処理を分岐させるコンポジットパターン。
/// 上から順番に条件を評価し、最初にtrueになった条件のパターンを実行する。
/// すべての条件がfalseの場合はデフォルトパターンを実行する。
/// </summary>
[CreateAssetMenu(fileName = "BRANCH_", menuName = "Danmaku/Pattern/Composite/Branch")]
public class BranchPattern : PatternBase
{
    [Header("条件分岐リスト")]
    [Tooltip("上から順番に評価される条件分岐のリスト。最初にtrueになった条件のパターンが実行されます。")]
    [SerializeField] private List<ConditionBranch> _conditionBranches = new List<ConditionBranch>();

    [Header("参照パラメータ")]
    [Tooltip("条件式内で使用するGameParameter")]
    [SerializeField] private List<GameParameter> _referencedParameters = new List<GameParameter>();

    [Header("デフォルト処理")]
    [Tooltip("すべての条件がfalseの場合に実行するパターン（任意）")]
    [SerializeField] private PatternBaseReference _defaultPattern;

    [Header("デバッグ")]
    [Tooltip("条件評価結果をログに出力するかどうか")]
    [SerializeField] private bool _logEvaluationResult = false;

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        PatternBaseReference selectedPattern = FindMatchingPattern();

        if (selectedPattern != null && selectedPattern.Value != null)
        {
            await selectedPattern.Value.Execute(controller, token);
        }
        else if (_logEvaluationResult)
        {
            Debug.Log($"[BranchPattern] {this.name}: No pattern to execute (all conditions false and no default)", this);
        }
    }

    public override async UniTask ExecuteImpl(GameEntityState state, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        PatternBaseReference selectedPattern = FindMatchingPattern();

        if (selectedPattern != null && selectedPattern.Value != null)
        {
            await selectedPattern.Value.Execute(state, token);
        }
        else if (_logEvaluationResult)
        {
            Debug.Log($"[BranchPattern] {this.name}: No pattern to execute (all conditions false and no default)", this);
        }
    }

    /// <summary>
    /// 条件を順番に評価して、最初にtrueになった条件のパターンを返す
    /// </summary>
    /// <returns>実行すべきパターン、またはnull</returns>
    private PatternBaseReference FindMatchingPattern()
    {
        for (int i = 0; i < _conditionBranches.Count; i++)
        {
            var branch = _conditionBranches[i];
            bool conditionResult = EvaluateCondition(branch.conditionExpression, i);

            if (_logEvaluationResult)
            {
                string desc = string.IsNullOrEmpty(branch.description) ? $"Branch {i}" : branch.description;
                Debug.Log($"[BranchPattern] {this.name}: {desc} - Condition '{branch.conditionExpression}' evaluated to {conditionResult}", this);
            }

            if (conditionResult)
            {
                return branch.pattern;
            }
        }

        // すべての条件がfalseの場合はデフォルトパターンを返す
        if (_logEvaluationResult)
        {
            Debug.Log($"[BranchPattern] {this.name}: All conditions false, using default pattern", this);
        }
        return _defaultPattern;
    }

    /// <summary>
    /// 条件式を評価してbool値を返す
    /// </summary>
    /// <param name="conditionExpression">評価する条件式</param>
    /// <param name="branchIndex">分岐のインデックス（ログ用）</param>
    /// <returns>条件式の評価結果</returns>
    private bool EvaluateCondition(string conditionExpression, int branchIndex)
    {
        if (string.IsNullOrEmpty(conditionExpression))
        {
            Debug.LogWarning($"Condition expression is not set in branch {branchIndex} of {this.name}", this);
            return false; // 条件式が空の場合はfalse
        }

        try
        {
            Expression expression = new Expression(conditionExpression);

            // 参照パラメータを設定
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

                // パラメータ名とインデックスベースの両方で参照可能にする
                expression.addArguments(new Argument(param.name, value));
                expression.addArguments(new Argument($"p{i}", value));
            }

            // グローバルなゲーム状態も利用可能にする
            expression.addArguments(new Argument("time", Time.time));
            expression.addArguments(new Argument("deltaTime", Time.deltaTime));

            // 計算結果が0でなければtrue、0ならfalseとする
            double result = expression.calculate();
            return result != 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to evaluate condition expression '{conditionExpression}' in branch {branchIndex} of {this.name}: {e.Message}", this);
            return false; // エラー時はfalse
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// エディタでの検証用メソッド
    /// </summary>
    [ContextMenu("Test Condition Evaluation")]
    private void TestConditionEvaluation()
    {
        if (Application.isPlaying)
        {
            Debug.Log($"[Test] Testing all conditions in {this.name}:", this);
            for (int i = 0; i < _conditionBranches.Count; i++)
            {
                var branch = _conditionBranches[i];
                bool result = EvaluateCondition(branch.conditionExpression, i);
                string desc = string.IsNullOrEmpty(branch.description) ? $"Branch {i}" : branch.description;
                Debug.Log($"[Test] {desc}: Condition '{branch.conditionExpression}' evaluated to: {result}", this);
            }

            var selectedPattern = FindMatchingPattern();
            if (selectedPattern != null && selectedPattern.Value != null)
            {
                Debug.Log($"[Test] Selected pattern: {selectedPattern.Value.name}", this);
            }
            else
            {
                Debug.Log($"[Test] No pattern selected (all conditions false and no default)", this);
            }
        }
        else
        {
            Debug.LogWarning("Condition evaluation test can only be performed during play mode.", this);
        }
    }
#endif
}
