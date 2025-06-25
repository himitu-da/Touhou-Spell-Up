using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでオブジェクトを保持
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は新しいインスタンスを破棄
            return;
        }

        InitializeApplication();
    }

    /// <summary>
    /// アプリケーションの初期化処理を行う。
    void InitializeApplication()
    {
        Application.targetFrameRate = 60; // フレームレートを設定
    }
}
