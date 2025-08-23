using UnityEngine;
using UnityEngine.SceneManagement;

public class DanmakuGameManager : MonoBehaviour
{
    public static DanmakuGameManager Instance { get; private set; }

    [Header("GameParameter設定")]
    [Tooltip("ゲーム再起動時にGameParameterをリセットするか")]
    [SerializeField] private bool resetGameParametersOnRestart = true;

    [Header("デバッグ")]
    [Tooltip("システム初期化ログを表示するか")]
    [SerializeField] private bool showDebugLog = true;

    private bool _systemReady = false;
    public bool IsSystemReady => _systemReady;

    private float _gameTime = 0f;
    public float GameTime => _gameTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            EnsureRequiredManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (GameParameterManager.IsInitialized)
        {
            OnSystemReady();
        }
        else
        {
            GameParameterManager.OnInitializationComplete += OnSystemReady;
        }

        // リスタート後のシーンリロード検知
        CheckForRestartReload();
    }

    void Update()
    {
        if (_systemReady)
        {
            _gameTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// リスタート後のシーンリロードをチェック
    /// </summary>
    private void CheckForRestartReload()
    {
        // PlayerPrefsでリスタートフラグをチェック
        if (PlayerPrefs.GetInt("GameRestarted", 0) == 1)
        {
            PlayerPrefs.DeleteKey("GameRestarted");
            
            // GameParameterManagerにシーンリロード後処理を通知
            if (GameParameterManager.Instance != null)
            {
                GameParameterManager.Instance.OnSceneReloaded();
            }

            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Restart scene reload detected, parameters reset.");
            }
        }
    }

    /// <summary>
    /// 必要なマネージャーの存在を確認・作成
    /// </summary>
    private void EnsureRequiredManagers()
    {
        // GameParameterManagerが存在しない場合は作成
        if (GameParameterManager.Instance == null)
        {
            var managerObj = new GameObject("GameParameterManager");
            managerObj.AddComponent<GameParameterManager>();
            
            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Created GameParameterManager.");
            }
        }
    }

    /// <summary>
    /// システム準備完了時のコールバック
    /// </summary>
    private void OnSystemReady()
    {
        _systemReady = true;
        
        if (showDebugLog)
        {
            Debug.Log("DanmakuGameManager: All systems ready, game can start.");
        }
    }

    public void GameOver()
    {
        // ゲームオーバー処理
        Debug.Log("Game Over");
        // 2秒後にリスタート
        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        // リスタート前にGameParameterManagerに通知
        if (GameParameterManager.Instance != null)
        {
            GameParameterManager.Instance.OnGameRestart();
        }

        // システム状態をリセット
        _systemReady = false;
        _gameTime = 0f;

        // リスタートフラグを設定（シーンリロード後の検知用）
        PlayerPrefs.SetInt("GameRestarted", 1);

        if (showDebugLog)
        {
            Debug.Log("DanmakuGameManager: Restarting game...");
        }

        // シーンリロード
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// GameParameterのリセット設定を変更
    /// </summary>
    public void SetResetGameParametersOnRestart(bool value)
    {
        resetGameParametersOnRestart = value;
        
        // GameParameterManagerにも反映
        if (GameParameterManager.Instance != null)
        {
            GameParameterManager.Instance.SetResetOnRestart(value);
        }
    }

    /// <summary>
    /// 手動でGameParameterをリセット
    /// </summary>
    public void ManualResetGameParameters()
    {
        if (GameParameterManager.Instance != null)
        {
            GameParameterManager.Instance.ResetAllGameParameters();
            
            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Manual parameter reset completed.");
            }
        }
        else
        {
            // GameParameterManagerがない場合は静的メソッドを使用
            GameParameter.ResetAll();
            
            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Manual static parameter reset completed.");
            }
        }
    }

    /// <summary>
    /// 静的メソッドを使用してGameParameterをリセット
    /// </summary>
    public void ManualResetGameParametersStatic()
    {
        GameParameter.ResetAll();
        
        if (showDebugLog)
        {
            Debug.Log("DanmakuGameManager: Manual static reset completed.");
        }
    }

    /// <summary>
    /// 即座にGameParameterのリセットを実行（デバッグ用）
    /// </summary>
    public void ForceResetGameParameters()
    {
        if (GameParameterManager.Instance != null)
        {
            GameParameterManager.Instance.ResetGameParametersWithAutoResetEnabled();
            
            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Force reset completed.");
            }
        }
        else
        {
            GameParameter.ResetAll();
            
            if (showDebugLog)
            {
                Debug.Log("DanmakuGameManager: Static force reset completed.");
            }
        }
    }
}
