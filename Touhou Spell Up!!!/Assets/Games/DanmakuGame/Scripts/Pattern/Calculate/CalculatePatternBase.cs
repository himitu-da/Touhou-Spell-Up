using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

/// <summary>
/// パラメータ計算パターンのためのジェネリックな基底クラス。
/// </summary>
/// <typeparam name="T">計算結果の型</typeparam>
public abstract class CalculatePatternBase<T> : PatternBase
{
    [Tooltip("計算結果を代入する対象のパラメータ")]
    [SerializeField] protected GameParameter<T> _targetParameter;

    [Tooltip("計算に使用する他のGameParameter")]
    [SerializeField] protected List<GameParameter> _referencedParameters = new List<GameParameter>();

    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (_targetParameter == null)
        {
            Debug.LogWarning($"TargetParameter is not set in {this.name}", this);
            return UniTask.CompletedTask;
        }

        try
        {
            // サブクラスで定義された計算処理を実行し、結果を代入
            _targetParameter.Value = CalculateValue();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to calculate value in {this.name}: {e.Message}", this);
        }

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// サブクラスで具体的な計算ロジックを実装するための抽象メソッド。
    /// </summary>
    /// <returns>計算結果</returns>
    protected abstract T CalculateValue();

    public override UniTask ExecuteImpl(GameEntityState state, CancellationToken token)
    {
        Debug.LogWarning($"{this.GetType().Name} does not support ExecuteImpl for GameEntityState.");
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 参照パラメータをインデックスで取得するヘルパーメソッド。
    /// </summary>
    protected U GetReferencedValue<U>(int index, U defaultValue = default)
    {
        if (index < 0 || index >= _referencedParameters.Count)
        {
            Debug.LogWarning($"Index {index} is out of range for ReferencedParameters in {this.name}", this);
            return defaultValue;
        }

        var param = _referencedParameters[index];
        if (param is GameParameter<U> typedParam)
        {
            return typedParam.Value;
        }
        
        Debug.LogWarning($"ReferencedParameter at index {index} is not of type {typeof(U)} in {this.name}", this);
        return defaultValue;
    }
}
