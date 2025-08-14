using UnityEngine;

/// <summary>
/// 共有される角度（float）のゲームパラメータ
/// </summary>
[CreateAssetMenu(fileName = "GPA_", menuName = "GameParameter/Angle (float)")]
public class AngleParameter : GameParameter<float>
{
    public override float Value
    {
        get => base.Value;
        set
        {
            // 角度を0-360の範囲に正規化
            float normalizedValue = value % 360f;
            if (normalizedValue < 0)
            {
                normalizedValue += 360f;
            }
            base.Value = normalizedValue;
        }
    }

    /// <summary>
    /// 角度に値を加算します。結果は自動的に0-360の範囲に正規化されます。
    /// </summary>
    /// <param name="amount">加算する角度</param>
    public void Add(float amount)
    {
        Value += amount;
    }
}
