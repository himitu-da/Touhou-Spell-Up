using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "POINT_", menuName = "Danmaku/Pattern/Emission/Point")]
[Obsolete("PointEmissionShape is deprecated. Use classes derived from PointEmissionShapeBase instead for enhanced functionality.", false)]
public class PointEmissionShape : EmissionShape
{
    [SerializeField] private Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float angle = baseAngleOffset.Value;
        if (angleMode.Value == AngleMode.AimToPlayer)
        {
            angle = CalculateAimAngle(movable, movable.transform.position + positionOffset.Value);
        }
        // Radialは点では無意味なので無視

        yield return new EmissionData { localPosition = positionOffset.Value, localAngle = angle };
        UpdateSharedAngle(angle);  // 共有更新（もし必要）
    }
}
