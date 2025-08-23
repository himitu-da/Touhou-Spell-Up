using UnityEngine;

/// <summary>
/// ゲームの状態（例: 敵のHP、経過時間）を動的に値として提供するゲームパラメータ。
/// （注意：このクラスは現在プレースホルダーであり、値の取得ロジックは未実装です）
/// </summary>
[CreateAssetMenu(fileName = "GPC_State_", menuName = "GameParameter/GameState/Float (Placeholder)")]
public class GameStateParameter : GameParameter<float>
{
    public enum GameStateValue
    {
        GameTime,           // 経過時間
        EnemyHealth,        // 敵の現在HP
        EnemyHealthPercent, // 敵の残りHP割合
        PlayerPower,        // プレイヤーのパワー
        PlayerGraze,        // プレイヤーのグレイズ数
        // ... 今後さらに追加
    }

    [Header("監視するゲームの状態")]
    [SerializeField]
    private GameStateValue valueToTrack;

    [Header("対象の指定（任意）")]
    [Tooltip("特定の敵のHPなどを参照する場合に設定")]
    [SerializeField]
    private GameEntityController targetEntity;

    public override float Value
    {
        get
        {
            if (!Application.isPlaying)
            {
                return initialValue;
            }

            switch (valueToTrack)
            {
                case GameStateValue.GameTime:
                    return DanmakuGameManager.Instance != null ? DanmakuGameManager.Instance.GameTime : 0f;

                case GameStateValue.EnemyHealth:
                    if (targetEntity != null)
                    {
                        return targetEntity.CurrentHealth;
                    }
                    Debug.LogWarning($"GameStateParameter ({name}): targetEntity is not set for EnemyHealth tracking.");
                    return 0f;

                case GameStateValue.EnemyHealthPercent:
                    if (targetEntity != null && targetEntity.MaxHealth > 0)
                    {
                        return targetEntity.CurrentHealth / targetEntity.MaxHealth;
                    }
                    if (targetEntity == null)
                    {
                        Debug.LogWarning($"GameStateParameter ({name}): targetEntity is not set for EnemyHealthPercent tracking.");
                    }
                    return 0f;

                case GameStateValue.PlayerPower:
                    // TODO: Playerの状態を管理するクラスから取得
                    Debug.LogWarning("PlayerPower tracking is not yet implemented.");
                    return 0f;

                case GameStateValue.PlayerGraze:
                    // TODO: Playerの状態を管理するクラスから取得
                    Debug.LogWarning("PlayerGraze tracking is not yet implemented.");
                    return 0f;

                default:
                    return base.Value;
            }
        }
        set => base.Value = value; // setは基本的に使わないが、念のため
    }

    public override void Reset()
    {
        // ゲーム状態は実行時に決まるため、リセットは通常不要
        // ただし、デバッグ用に初期値を設定することは可能
        base.Reset();
    }
}
