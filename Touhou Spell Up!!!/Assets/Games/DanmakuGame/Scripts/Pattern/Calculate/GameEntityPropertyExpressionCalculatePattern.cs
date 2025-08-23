using UnityEngine;

/// <summary>
/// 式評価によってGameEntityPropertyを選択する計算パターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Expr_GameEntityProperty_", menuName = "Danmaku/Pattern/Calculate/Expression/GameEntityProperty")]
public class GameEntityPropertyExpressionCalculatePattern : ExpressionCalculatePatternBase<GameEntityProperty>
{
    // ジェネリックな基底クラスの実装をそのまま利用するため、このクラスは空でよい
}
