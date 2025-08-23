using UnityEngine;

/// <summary>
/// 式評価によってPatternBaseを選択する計算パターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Expr_PatternBase_", menuName = "Danmaku/Pattern/Calculate/Expression/PatternBase")]
public class PatternBaseExpressionCalculatePattern : ExpressionCalculatePatternBase<PatternBase>
{
    // ジェネリックな基底クラスの実装をそのまま利用するため、このクラスは空でよい
}
