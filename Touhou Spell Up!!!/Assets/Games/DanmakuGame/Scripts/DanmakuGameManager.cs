using UnityEngine;
using UnityEngine.SceneManagement;

public class DanmakuGameManager : MonoBehaviour
{
    public static DanmakuGameManager Instance { get; private set; }

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

    public void GameOver()
    {
        // ゲームオーバー処理
        Debug.Log("Game Over");
        // 2秒後にリスタート
        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
