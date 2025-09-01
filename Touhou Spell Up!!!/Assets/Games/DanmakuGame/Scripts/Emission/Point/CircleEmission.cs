using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 円形発射パターン - 新システムの拡張例
/// </summary>
[CreateAssetMenu(fileName = "CircleEmission_", menuName = "Danmaku/Pattern/Emission/Point/Circle")]
public class CircleEmission : PointEmissionBase
{
    [Header("円形発射設定")]
    [Tooltip("円の半径")]
    [SerializeField] private FloatReference radius = new FloatReference { useConstant = true, constantValue = 1f };
    
    [Tooltip("円周上の発射点数")]
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 8 };
    
    [Tooltip("開始角度（度）")]
    [SerializeField] private FloatReference startAngle = new FloatReference { useConstant = true, constantValue = 0f };
    
    [Tooltip("終了角度（度）- startAngleと同じ場合は全周")]
    [SerializeField] private FloatReference endAngle = new FloatReference { useConstant = true, constantValue = 360f };
    
    [Tooltip("中心点のオフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetPointPositions(IMovable movable)
    {
        int count = Mathf.Max(1, pointCount.Value);
        float rad = radius.Value;
        float startRad = startAngle.Value * Mathf.Deg2Rad;
        float endRad = endAngle.Value * Mathf.Deg2Rad;
        Vector3 center = centerOffset.Value;

        // 角度範囲を計算
        float angleRange = endRad - startRad;
        if (Mathf.Approximately(angleRange, 0f) || Mathf.Approximately(Mathf.Abs(angleRange), 2 * Mathf.PI))
        {
            // 全周の場合
            angleRange = 2 * Mathf.PI;
        }

        for (int i = 0; i < count; i++)
        {
            float t = count > 1 ? (float)i / count : 0f;
            float angle = startRad + angleRange * t;
            
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * rad,
                Mathf.Sin(angle) * rad,
                0
            );
            
            yield return position;
        }
    }

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
                // 円の場合、中心からの放射角度
                Vector3 relative = position - centerOffset.Value;
                float radialAngle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
                angle += radialAngle;
                break;
        }

        return angle;
    }
}
