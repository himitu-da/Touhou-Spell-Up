using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 円周上のライン発射パターン
/// </summary>
[CreateAssetMenu(fileName = "CircleLineEmission_", menuName = "Danmaku/Pattern/Emission/Line/Circle")]
public class CircleLineEmission : LineEmissionBase
{
    [Header("円ライン設定")]
    [Tooltip("円の半径")]
    [SerializeField] private FloatReference radius = new FloatReference { useConstant = true, constantValue = 1f };
    
    [Tooltip("円周上の発射点数")]
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 8 };
    
    [Tooltip("開始角度（度）")]
    [SerializeField] private FloatReference startAngle = new FloatReference { useConstant = true, constantValue = 0f };
    
    [Tooltip("終了角度（度）- 開始角度と同じ場合は全周")]
    [SerializeField] private FloatReference endAngle = new FloatReference { useConstant = true, constantValue = 360f };
    
    [Tooltip("中心オフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetLinePositions(IMovable movable)
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
}
