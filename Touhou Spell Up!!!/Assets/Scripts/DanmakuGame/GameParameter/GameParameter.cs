using UnityEngine;

/// <summary>
/// 共有されるリソースの基底となる抽象クラス
/// </summary>
public abstract class GameParameter : ScriptableObject
{
    /// <summary>
    /// 値を初期状態にリセットする
    /// </summary>
    public abstract void Reset();
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

    /// <summary>
    /// 現在の値
    /// </summary>
    public virtual T Value
    {
        get => currentValue;
        set => currentValue = value;
    }

    /// <summary>
    /// 実行開始時に呼ばれ、値を初期値にリセットする
    /// </summary>
    protected virtual void OnEnable()
    {
        // プレイモード中のみリセットする（エディタでの意図しない値のリセットを防ぐ）
        if (Application.isPlaying)
        {
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
