using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 共有されるリソースの基底となる抽象クラス
/// </summary>
public abstract class GameParameter : ScriptableObject
{
    /// <summary>
    /// すべてのGameParameterインスタンスを静的に管理
    /// </summary>
    private static HashSet<GameParameter> allInstances = new HashSet<GameParameter>();

    /// <summary>
    /// 値を初期状態にリセットする
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// インスタンスが有効になった時に登録
    /// </summary>
    protected virtual void OnEnable()
    {
        allInstances.Add(this);
    }

    /// <summary>
    /// インスタンスが無効になった時に登録解除
    /// </summary>
    protected virtual void OnDisable()
    {
        allInstances.Remove(this);
    }

    /// <summary>
    /// すべてのGameParameterを静的にリセット
    /// </summary>
    public static void ResetAll()
    {
        // nullチェックしつつリセット
        var validInstances = allInstances.Where(instance => instance != null).ToList();
        foreach (var instance in validInstances)
        {
            instance.Reset();
        }
        
        Debug.Log($"GameParameter: {validInstances.Count} parameters reset to initial values.");
    }

    /// <summary>
    /// 登録されているGameParameterの数を取得
    /// </summary>
    public static int GetInstanceCount()
    {
        return allInstances.Count(instance => instance != null);
    }

    /// <summary>
    /// 登録されているすべてのGameParameterを取得（デバッグ用）
    /// </summary>
    public static List<GameParameter> GetAllInstances()
    {
        return allInstances.Where(instance => instance != null).ToList();
    }
}

/// <summary>
/// 型を指定できるジェネリック版の共有リソース
/// </summary>
/// <typeparam name="T">共有したい値の型</typeparam>
public abstract class GameParameter<T> : GameParameter
{
    [Header("現在の値（実行時に変化）")]
    [SerializeField] protected T currentValue;

    [Header("初期化時の値")]
    [SerializeField] protected T initialValue;

    [Header("初期化設定")]
    [SerializeField] 
    [Tooltip("OnEnable時にcurrentValueをinitialValueで自動的にリセットするか")]
    private bool autoResetOnEnable = true;

    /// <summary>
    /// 現在の値
    /// </summary>
    public virtual T Value
    {
        get => currentValue;
        set => currentValue = value;
    }

    /// <summary>
    /// OnEnable時の自動リセットを有効/無効にする
    /// </summary>
    public bool AutoResetOnEnable
    {
        get => autoResetOnEnable;
        set => autoResetOnEnable = value;
    }

    /// <summary>
    /// 実行開始時に呼ばれ、値を初期値にリセットする
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable(); // 基底クラスの登録処理を呼ぶ
        
        // プレイモード中かつ自動リセットが有効な場合のみリセットする
        if (Application.isPlaying && autoResetOnEnable)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"GameParameter OnEnable Reset: {name} from {currentValue} to {initialValue}");
            #endif
            Reset();
        }
    }

    /// <summary>
    /// 値を初期値にリセットする
    /// </summary>
    public override void Reset()
    {
        currentValue = initialValue;
    }
}