using UnityEngine;

/// <summary>
/// 式評価によってGameEntityを選択する計算パターン。
/// </summary>
[CreateAssetMenu(fileName = "CALC_Expr_GameEntity_", menuName = "Danmaku/Pattern/Calculate/Expression/GameEntity")]
public class GameEntityExpressionCalculatePattern : ExpressionCalculatePatternBase<GameEntity>
{
    // ジェネリックな基底クラスの実装をそのまま利用するため、このクラスは空でよい
}
