using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "POINT_", menuName = "Touhou Spell Up/Danmaku/Emission/Point")]
public class PointEmissionShape : EmissionShape
{
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float angle = baseAngleOffset;
        if (angleMode == AngleMode.AimToPlayer)
        {
            angle = CalculateAimAngle(movable, movable.transform.position + positionOffset);
        }
        // Radialは点では無意味なので無視

        yield return new EmissionData { localPosition = positionOffset, localAngle = angle };
        UpdateSharedAngle(angle);  // 共有更新（もし必要）
    }
}
