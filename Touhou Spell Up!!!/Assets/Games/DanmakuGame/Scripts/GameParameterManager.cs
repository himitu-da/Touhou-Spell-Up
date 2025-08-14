using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// すべてのGameParameterを管理し、一括でリセットする機能を提供するマネージャー
/// </summary>
public class GameParameterManager : MonoBehaviour
{
    public static GameParameterManager Instance { get; private set; }

    [Header("リセット設定")]
    [Tooltip("ゲーム開始時にGameParameterをリセットするか")]
    [SerializeField] private bool resetOnStart = true;
    
    [Tooltip("ゲーム再起動時にGameParameterをリセットするか")]
    [SerializeField] private bool resetOnRestart = true;
    
    [Header("デバッグ")]
    [Tooltip("リセット時にログを出力するか")]
    [SerializeField] private bool showDebugLog = true;

    // 実行時にロードされたすべてのGameParameterを管理
    private List<GameParameter> allGameParameters = new List<GameParameter>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ゲーム開始時にすべてのGameParameterを収集
            CollectAllGameParameters();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (resetOnStart)
        {
            ResetAllGameParameters();
        }
    }

    /// <summary>
    /// プロジェクト内のすべてのGameParameterアセットを収集する
    /// </summary>
    private void CollectAllGameParameters()
    {
        allGameParameters.Clear();

#if UNITY_EDITOR
        // エディタモードでは AssetDatabase を使用して検索
        var guids = UnityEditor.AssetDatabase.FindAssets("t:GameParameter");
        foreach (var guid in guids)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var gameParameter = UnityEditor.AssetDatabase.LoadAssetAtPath<GameParameter>(path);
            if (gameParameter != null)
            {
                allGameParameters.Add(gameParameter);
            }
        }
#else
        // ビルド版では Resources.FindObjectsOfTypeAll を使用
        var foundParameters = Resources.FindObjectsOfTypeAll<GameParameter>();
        allGameParameters.AddRange(foundParameters);
#endif

        if (showDebugLog)
        {
            Debug.Log($"GameParameterManager: {allGameParameters.Count} GameParameters collected.");
        }
    }

    /// <summary>
    /// すべてのGameParameterを初期値にリセットする
    /// </summary>
    public void ResetAllGameParameters()
    {
        if (allGameParameters == null || allGameParameters.Count == 0)
        {
            CollectAllGameParameters();
        }

        int resetCount = 0;
        foreach (var parameter in allGameParameters)
        {
            if (parameter != null)
            {
                parameter.Reset();
                resetCount++;
            }
        }

        if (showDebugLog)
        {
            Debug.Log($"GameParameterManager: {resetCount} GameParameters reset to initial values.");
        }
    }

    /// <summary>
    /// 静的メソッドを使用してリセット（GameParameter.ResetAll()を呼び出し）
    /// </summary>
    public void ResetAllGameParametersStatic()
    {
        GameParameter.ResetAll();
    }

    /// <summary>
    /// 特定の型のGameParameterのみをリセット
    /// </summary>
    public void ResetGameParametersOfType<T>() where T : GameParameter
    {
        var targetParameters = allGameParameters.OfType<T>().ToList();
        int resetCount = 0;

        foreach (var parameter in targetParameters)
        {
            if (parameter != null)
            {
                parameter.Reset();
                resetCount++;
            }
        }

        if (showDebugLog)
        {
            Debug.Log($"GameParameterManager: {resetCount} GameParameters of type {typeof(T).Name} reset.");
        }
    }

    /// <summary>
    /// 再起動時にリセットするかどうかを設定
    /// </summary>
    public void SetResetOnRestart(bool value)
    {
        resetOnRestart = value;
    }

    /// <summary>
    /// 開始時にリセットするかどうかを設定
    /// </summary>
    public void SetResetOnStart(bool value)
    {
        resetOnStart = value;
    }

    /// <summary>
    /// 再起動時に呼ばれる（DanmakuGameManagerから呼び出される）
    /// </summary>
    public void OnGameRestart()
    {
        if (resetOnRestart)
        {
            ResetAllGameParameters();
        }
    }

    /// <summary>
    /// GameParameterの一覧を取得（デバッグ用）
    /// </summary>
    public List<GameParameter> GetAllGameParameters()
    {
        return new List<GameParameter>(allGameParameters);
    }

    /// <summary>
    /// GameParameterの再収集（動的に追加された場合など）
    /// </summary>
    public void RefreshGameParameters()
    {
        CollectAllGameParameters();
    }

    /// <summary>
    /// Inspector上で設定を変更した時の処理
    /// </summary>
    void OnValidate()
    {
        if (Application.isPlaying && showDebugLog)
        {
            Debug.Log($"GameParameterManager Settings - ResetOnStart: {resetOnStart}, ResetOnRestart: {resetOnRestart}");
        }
    }
}