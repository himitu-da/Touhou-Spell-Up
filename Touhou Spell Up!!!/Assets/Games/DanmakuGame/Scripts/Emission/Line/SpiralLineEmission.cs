using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 螺旋ライン発射パターン - 新システムの拡張例
/// </summary>
[CreateAssetMenu(fileName = "SpiralLineEmission_", menuName = "Danmaku/Pattern/Emission/Line/Spiral")]
public class SpiralLineEmission : LineEmissionBase
{
    [Header("螺旋ライン設定")]
    [Tooltip("螺旋の開始半径")]
    [SerializeField] private FloatReference startRadius = new FloatReference { useConstant = true, constantValue = 0.5f };
    
    [Tooltip("螺旋の終了半径")]
    [SerializeField] private FloatReference endRadius = new FloatReference { useConstant = true, constantValue = 2f };
    
    [Tooltip("螺旋の回転数")]
    [SerializeField] private FloatReference rotations = new FloatReference { useConstant = true, constantValue = 2f };
    
    [Tooltip("螺旋上の発射点数")]
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 16 };
    
    [Tooltip("螺旋の中心オフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetLinePositions(IMovable movable)
    {
        int count = Mathf.Max(1, pointCount.Value);
        float startRad = startRadius.Value;
        float endRad = endRadius.Value;
        float totalRotations = rotations.Value;
        Vector3 center = centerOffset.Value;

        for (int i = 0; i < count; i++)
        {
            float t = count > 1 ? (float)i / (count - 1) : 0f;
            
            // 半径を線形補間
            float currentRadius = Mathf.Lerp(startRad, endRad, t);
            
            // 角度を計算（回転数に基づく）
            float angle = t * totalRotations * 2 * Mathf.PI;
            
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * currentRadius,
                Mathf.Sin(angle) * currentRadius,
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
                // 螺旋の場合、中心からの放射角度
                Vector3 relative = position - centerOffset.Value;
                float radialAngle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
                angle += radialAngle;
                break;
        }

        return angle;
    }
}
