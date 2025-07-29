using UnityEngine;
using System.Collections.Generic;

public abstract class EmissionShape : ScriptableObject
{
    [Tooltip("共有角度（オプション）。これを設定すると、生成される角度に適用。")]
    [SerializeField] protected SharedAngle sharedAngle;

    [Tooltip("角度モード: Fixed（固定）、AimToPlayer（各ポイントで自機狙い）、Radial（放射状）")]
    [SerializeField] protected AngleMode angleMode = AngleMode.Fixed;

    [Tooltip("ベース角度オフセット")]
    [SerializeField] protected float baseAngleOffset = 0f;

    public enum AngleMode { Fixed, AimToPlayer, Radial }

    // 抽象メソッド: 発射データを生成。movableの位置/回転を基準にローカルデータを計算
    public abstract IEnumerable<EmissionData> GetEmissions(IMovable movable);

    // ヘルパー: 自機狙い角度を計算（既存のGetAimAngleを再利用可能）
    protected float CalculateAimAngle(IMovable movable, Vector3 position)
    {
        // TODO: 既存のGetAimAngleなどを参考に、正しい自機狙い角度計算を実装する
        // 現時点ではダミー値を返す
        return 0f;
    }

    // 共有角度の更新ロジック（オプション、オーバーライド可能）
    protected void UpdateSharedAngle(float newValue)
    {
        if (sharedAngle != null)
        {
            sharedAngle.Value = newValue;
        }
    }
}
