using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using System;

// 全てのパターンの基底となる抽象クラス
public abstract class PatternBase : ScriptableObject
{
    [Header("実行条件")]
    [TextArea(2, 5)]
    [Tooltip("Pattern実行開始条件。空の場合は常に実行。mXparser式を使用\n" +
             "例: 'p0 > 100' (p0が100より大きい場合)\n" +
             "例: 'time > 10 && p1 < 50' (時間が10秒を超え、かつp1が50未満)")]
    [SerializeField] private string _startConditionExpression = "";
    
    [TextArea(2, 5)]
    [Tooltip("Pattern実行中の終了条件。満たされると強制終了。空の場合は条件なし\n" +
             "例: 'p0 <= 0' (p0が0以下になったら終了)\n" +
             "例: 'time > p1' (時間がp1を超えたら終了)\n" +
             "💡Tips: 親パターンが終了すると、原則として子パターンも自動的に終了します")]
    [SerializeField] private string _endConditionExpression = "";
    
    [Header("条件用パラメータ")]
    [Tooltip("条件式で使用するGameParameter。p0, p1, p2...またはパラメータ名で参照可能")]
    [SerializeField] private List<GameParameter> _conditionParameters = new List<GameParameter>();

    [Header("待機時間")]
    [Tooltip("ExecuteImplの実行前に待機する時間（秒）")]
    [SerializeField] private FloatReference _beforeAwaitSeconds = new FloatReference { useConstant = true, constantValue = 0f };

    [Tooltip("ExecuteImplの実行後に待機する時間（秒）")]
    [SerializeField] private FloatReference _afterAwaitSeconds = new FloatReference { useConstant = true, constantValue = 0f };

    // GameEntityControllerを受け取るExecuteメソッド
    public virtual async UniTask Execute(GameEntityController controller, CancellationToken token)
    {
        // 開始条件チェック
        if (!string.IsNullOrEmpty(_startConditionExpression))
        {
            if (!EvaluateCondition(_startConditionExpression))
            {
                return; // 開始条件が満たされていない
            }
        }

        if (token.IsCancellationRequested) return;
        
        // 事前待機
        if (_beforeAwaitSeconds.Value > 0) 
            await UniTask.Delay(System.TimeSpan.FromSeconds(_beforeAwaitSeconds.Value), cancellationToken: token);
        
        if (token.IsCancellationRequested) return;

        // 終了条件がある場合は監視しながら実行
        if (!string.IsNullOrEmpty(_endConditionExpression))
        {
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                var executionTask = ExecuteImpl(controller, linkedCts.Token);
                var monitorTask = MonitorEndCondition(linkedCts);
                
                await UniTask.WhenAny(executionTask, monitorTask);
                linkedCts.Cancel();
            }
        }
        else
        {
            // 終了条件がない場合は通常実行
            await ExecuteImpl(controller, token);
        }

        if (token.IsCancellationRequested) return;
        
        // 事後待機
        if (_afterAwaitSeconds.Value > 0) 
            await UniTask.Delay(System.TimeSpan.FromSeconds(_afterAwaitSeconds.Value), cancellationToken: token);
    }

    // MovementStateを受け取るExecuteメソッドのオーバーロード
    public virtual async UniTask Execute(MovementState state, CancellationToken token)
    {
        // 開始条件チェック
        if (!string.IsNullOrEmpty(_startConditionExpression))
        {
            if (!EvaluateCondition(_startConditionExpression))
            {
                return; // 開始条件が満たされていない
            }
        }

        if (token.IsCancellationRequested) return;
        
        // 事前待機
        if (_beforeAwaitSeconds.Value > 0) 
            await UniTask.Delay(System.TimeSpan.FromSeconds(_beforeAwaitSeconds.Value), cancellationToken: token);
        
        if (token.IsCancellationRequested) return;

        // 終了条件がある場合は監視しながら実行
        if (!string.IsNullOrEmpty(_endConditionExpression))
        {
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                var executionTask = ExecuteImpl(state, linkedCts.Token);
                var monitorTask = MonitorEndCondition(linkedCts);
                
                await UniTask.WhenAny(executionTask, monitorTask);
                linkedCts.Cancel();
            }
        }
        else
        {
            // 終了条件がない場合は通常実行
            await ExecuteImpl(state, token);
        }

        if (token.IsCancellationRequested) return;
        
        // 事後待機
        if (_afterAwaitSeconds.Value > 0) 
            await UniTask.Delay(System.TimeSpan.FromSeconds(_afterAwaitSeconds.Value), cancellationToken: token);
    }

    // サブクラスで具体的な処理を実装するための抽象メソッド
    public abstract UniTask ExecuteImpl(GameEntityController controller, CancellationToken token);

    // MovementStateを受け取るExecuteImplのオーバーロード（デフォルト実装は例外をスロー）
    public virtual UniTask ExecuteImpl(MovementState state, CancellationToken token)
    {
        // このメソッドはMovePattern系のクラスでoverrideされることを想定
        throw new System.NotImplementedException($"{this.GetType().Name} does not implement ExecuteImpl for MovementState.");
    }

    /// <summary>
    /// 条件式を評価してbool値を返す（BranchPatternの実装を参考）
    /// </summary>
    /// <param name="conditionExpression">評価する条件式</param>
    /// <returns>条件式の評価結果</returns>
    private bool EvaluateCondition(string conditionExpression)
    {
        if (string.IsNullOrEmpty(conditionExpression))
        {
            return true; // 条件式が空の場合はtrue
        }

        try
        {
            Expression expression = new Expression(conditionExpression);

            // 参照パラメータを設定
            for (int i = 0; i < _conditionParameters.Count; i++)
            {
                var param = _conditionParameters[i];
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

            // グローバルなゲーム状態も利用可能にする（FixedUpdateの原則に従う）
            expression.addArguments(new Argument("time", Time.fixedTime));
            expression.addArguments(new Argument("deltaTime", Time.fixedDeltaTime));

            // 計算結果が0でなければtrue、0ならfalseとする
            double result = expression.calculate();
            return result != 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to evaluate condition expression '{conditionExpression}' in {this.name}: {e.Message}", this);
            return false; // エラー時はfalse
        }
    }

    /// <summary>
    /// 終了条件を監視する
    /// </summary>
    /// <param name="cts">キャンセルトークンソース</param>
    private async UniTask MonitorEndCondition(CancellationTokenSource cts)
    {
        while (!cts.Token.IsCancellationRequested)
        {
            if (EvaluateCondition(_endConditionExpression))
            {
                cts.Cancel(); // 終了条件が満たされたらキャンセル
                break;
            }
            
            // FixedUpdateタイミングで監視（Patternの原則に従う）
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);
        }
    }
}
