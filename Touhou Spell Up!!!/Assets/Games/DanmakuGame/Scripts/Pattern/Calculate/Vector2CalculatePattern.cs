using UnityEngine;

/// <summary>
/// 参照パラメータからVector2値を組み立て、GameParameterに代入するパターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Vector2_", menuName = "Danmaku/Pattern/Calculate/Vector2")]
public class Vector2CalculatePattern : CalculatePatternBase<Vector2>
{
    [Tooltip("X成分として使用する参照パラメータのインデックス")]
    [SerializeField] private int _xSourceIndex = 0;

    [Tooltip("Y成分として使用する参照パラメータのインデックス")]
    [SerializeField] private int _ySourceIndex = 1;

    protected override Vector2 CalculateValue()
    {
        // ヘルパーメソッドを使い、インデックス指定でfloat値を取得
        float x = GetReferencedValue<float>(_xSourceIndex);
        float y = GetReferencedValue<float>(_ySourceIndex);

        return new Vector2(x, y);
    }
}
