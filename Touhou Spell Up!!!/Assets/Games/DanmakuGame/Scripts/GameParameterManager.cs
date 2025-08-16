using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// すべてのGameParameterを管理し、一括でリセットする機能を提供するマネージャー
/// </summary>
public class GameParameterManager : MonoBehaviour
{
    public static GameParameterManager Instance { get; private set; }
    public static bool IsInitialized { get; private set; } = false;
    public static event System.Action OnInitializationComplete;

    [Header("リセット設定")]
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
            InitializeGameParameters();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 旧式の遅延リセット処理は削除
        // 初期化はAwakeで同期的に完了済み
        if (showDebugLog)
        {
            Debug.Log("GameParameterManager: Start() called, initialization already completed in Awake().");
        }
    }

    /// <summary>
    /// GameParameterの同期初期化を実行
    /// </summary>
    private void InitializeGameParameters()
    {
        CollectAllGameParameters();
        ResetGameParametersWithAutoResetEnabled();
        IsInitialized = true;
        OnInitializationComplete?.Invoke();
        
        if (showDebugLog)
        {
            Debug.Log("GameParameterManager: Initialization completed synchronously.");
        }
    }

    /// <summary>
    /// 遅延リセット処理
    /// </summary>
    private void DelayedReset()
    {
        if (resetOnRestart)
        {
            ResetGameParametersWithAutoResetEnabled();
            
            if (showDebugLog)
            {
                Debug.Log("GameParameterManager: Delayed reset completed after scene reload.");
            }
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
    /// autoResetOnEnableがtrueのGameParameterのみをリセットする
    /// </summary>
    public void ResetGameParametersWithAutoResetEnabled()
    {
        // 最新のGameParameterを再収集
        CollectAllGameParameters();

        int resetCount = 0;
        foreach (var parameter in allGameParameters)
        {
            if (parameter != null && ShouldAutoReset(parameter))
            {
                parameter.Reset();
                resetCount++;
                
                if (showDebugLog)
                {
                    Debug.Log($"Reset GameParameter: {parameter.name} (Type: {parameter.GetType().Name})");
                }
            }
        }

        if (showDebugLog)
        {
            Debug.Log($"GameParameterManager: {resetCount} GameParameters with autoResetOnEnable=true reset to initial values.");
        }
    }

    /// <summary>
    /// GameParameterがautoResetOnEnableを有効にしているかチェック
    /// </summary>
    private bool ShouldAutoReset(GameParameter parameter)
    {
        // リフレクションを使用してAutoResetOnEnableプロパティにアクセス
        var autoResetProperty = parameter.GetType().GetProperty("AutoResetOnEnable");
        if (autoResetProperty != null)
        {
            return (bool)autoResetProperty.GetValue(parameter);
        }
        // プロパティが見つからない場合はデフォルトでリセットする
        return true;
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
    /// 再起動時に呼ばれる（DanmakuGameManagerから呼び出される）
    /// </summary>
    public void OnGameRestart()
    {
        if (resetOnRestart)
        {
            // 即座にリセットを実行
            ResetGameParametersWithAutoResetEnabled();
            
            if (showDebugLog)
            {
                Debug.Log("GameParameterManager: Restart reset completed.");
            }
        }
    }

    /// <summary>
    /// シーンリロード後の初期化処理
    /// リスタート時の実行順序を保証するため
    /// </summary>
    public void OnSceneReloaded()
    {
        // 既にAwakeで初期化されているが、リスタート時は再度リセット
        if (resetOnRestart)
        {
            ResetGameParametersWithAutoResetEnabled();
            
            if (showDebugLog)
            {
                Debug.Log("GameParameterManager: Scene reloaded, parameters reset for restart.");
            }
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
            Debug.Log($"GameParameterManager Settings - ResetOnRestart: {resetOnRestart}, Debug Log: {showDebugLog}");
        }
    }
}