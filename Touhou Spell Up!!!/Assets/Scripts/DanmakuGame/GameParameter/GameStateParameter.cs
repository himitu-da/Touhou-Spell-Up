using UnityEngine;

/// <summary>
/// ゲームの状態（例: 敵のHP、経過時間）を動的に値として提供するゲームパラメータ。
/// （注意：このクラスは現在プレースホルダーであり、値の取得ロジックは未実装です）
/// </summary>
[CreateAssetMenu(fileName = "GPC_State_", menuName = "Danmaku/GameParameter/GameState/Float (Placeholder)")]
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

    public override void Reset()
    {
        // ゲーム状態は実行時に決まるため、リセットは通常不要
        // ただし、デバッグ用に初期値を設定することは可能
        base.Reset();
    }

    // TODO:
    // 実行時に毎フレーム、あるいは値が要求されたタイミングで、
    // valueToTrackに応じて適切なマネージャーやコントローラーから値を取得し、
    // currentValueを更新するロジックを実装する必要がある。
    // (例: GameTimeならTime.timeSinceLevelLoad, EnemyHealthならtargetEntityから取得)
}
