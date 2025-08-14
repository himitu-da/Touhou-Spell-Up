using UnityEngine;
using UnityEngine.SceneManagement;

public class DanmakuGameManager : MonoBehaviour
{
    public static DanmakuGameManager Instance { get; private set; }

    [Header("GameParameter設定")]
    [Tooltip("ゲーム再起動時にGameParameterをリセットするか")]
    [SerializeField] private bool resetGameParametersOnRestart = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // GameParameterManagerが存在しない場合は作成
        if (GameParameterManager.Instance == null)
        {
            var managerObj = new GameObject("GameParameterManager");
            managerObj.AddComponent<GameParameterManager>();
            
            // 設定を同期
            if (GameParameterManager.Instance != null)
            {
                GameParameterManager.Instance.SetResetOnRestart(resetGameParametersOnRestart);
            }
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
        // ゲームパラメータのリセット処理
        if (resetGameParametersOnRestart && GameParameterManager.Instance != null)
        {
            GameParameterManager.Instance.OnGameRestart();
        }

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
        }
        else
        {
            // GameParameterManagerがない場合は静的メソッドを使用
            GameParameter.ResetAll();
        }
    }

    /// <summary>
    /// 静的メソッドを使用してGameParameterをリセット
    /// </summary>
    public void ManualResetGameParametersStatic()
    {
        GameParameter.ResetAll();
    }
}