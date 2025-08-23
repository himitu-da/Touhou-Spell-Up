using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 円形面発射パターン
/// </summary>
[CreateAssetMenu(fileName = "CirclePlaneEmission_", menuName = "Danmaku/Pattern/Emission/Plane/Circle")]
public class CirclePlaneEmission : PlaneEmissionBase
{
    [Header("円面設定")]
    [Tooltip("円の半径")]
    [SerializeField] private FloatReference radius = new FloatReference { useConstant = true, constantValue = 1f };
    
    [Tooltip("同心円の数")]
    [SerializeField] private IntReference ringCount = new IntReference { useConstant = true, constantValue = 3 };
    
    [Tooltip("各リング上のポイント数")]
    [SerializeField] private IntReference pointsPerRing = new IntReference { useConstant = true, constantValue = 8 };
    
    [Tooltip("中心にもポイントを配置するか")]
    [SerializeField] private BoolReference includeCenterPoint = new BoolReference { useConstant = true, constantValue = true };
    
    [Tooltip("中心オフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetPlanePositions(IMovable movable)
    {
        int rings = Mathf.Max(1, ringCount.Value);
        int pointsPerRingValue = Mathf.Max(1, pointsPerRing.Value);
        float maxRadius = radius.Value;
        Vector3 center = centerOffset.Value;

        // 中心点
        if (includeCenterPoint.Value)
        {
            yield return center;
        }

        // 同心円上のポイント
        for (int ring = 1; ring <= rings; ring++)
        {
            float currentRadius = maxRadius * (float)ring / rings;
            int pointsInThisRing = ring == rings ? pointsPerRingValue : Mathf.Max(1, pointsPerRingValue * ring / rings);
            
            for (int i = 0; i < pointsInThisRing; i++)
            {
                float angle = 2 * Mathf.PI * i / pointsInThisRing;
                Vector3 position = center + new Vector3(
                    Mathf.Cos(angle) * currentRadius,
                    Mathf.Sin(angle) * currentRadius,
                    0
                );
                yield return position;
            }
        }
    }
}
