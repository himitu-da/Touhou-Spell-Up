using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 単一点発射パターン - 新階層システム版
/// </summary>
[CreateAssetMenu(fileName = "PointEmission_", menuName = "Danmaku/Pattern/Emission/Point/Basic")]
public class PointEmission : PointEmissionBase
{
    [Header("ポイント発射設定")]
    [Tooltip("発射位置のオフセット")]
    [SerializeField] private Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetPointPositions(IMovable movable)
    {
        yield return positionOffset.Value;
    }

    // 単一点なので特別な処理は不要、基底クラスの機能をそのまま使用
    protected override float CalculateAngleForPosition(IMovable movable, Vector3 position, int index, int totalCount)
    {
        float angle = baseAngleOffset.Value;
        if (sharedAngle != null && sharedAngle.Value != null)
            angle = sharedAngle.Value.Value;

        switch (angleMode.Value)
        {
            case AngleMode.AimToPlayer:
                Vector3 worldPos = movable.transform.position + movable.transform.rotation * position;
                angle = CalculateAimAngle(movable, worldPos);
                break;
            case AngleMode.Radial:
                // 単一点の場合、放射状は基本角度と同じ
                break;
        }

        return angle;
    }
}
