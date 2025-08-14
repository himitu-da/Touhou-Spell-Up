using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LINE_", menuName = "Danmaku/Pattern/Emission/Line")]
public class LineEmissionShape : EmissionShape
{
    [SerializeField] private Vector3Reference startOffset = new Vector3Reference { useConstant = true, constantValue = new Vector3(-1f, 0, 0) };  // 始点オフセット
    [SerializeField] private Vector3Reference endOffset = new Vector3Reference { useConstant = true, constantValue = new Vector3(1f, 0, 0) };     // 終点オフセット
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 5 };             // 分割数

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float step = 1f / (pointCount.Value - 1);
        float currentAngle = baseAngleOffset.Value;
        if (sharedAngle != null && sharedAngle.Value != null) currentAngle = sharedAngle.Value.Value;

        for (int i = 0; i < pointCount.Value; i++)
        {
            Vector3 localPos = Vector3.Lerp(startOffset.Value, endOffset.Value, i * step);
            float angle = currentAngle;

            switch (angleMode.Value)
            {
                case AngleMode.AimToPlayer:
                    Vector3 worldPos = movable.transform.position + movable.transform.rotation * localPos;
                    angle = CalculateAimAngle(movable, worldPos);
                    break;
                case AngleMode.Radial:
                    angle += i * (360f / pointCount.Value);  // 放射状例
                    break;
                // Fixedはangleそのまま
            }

            yield return new EmissionData { localPosition = localPos, localAngle = angle };
        }

        UpdateSharedAngle(currentAngle + baseAngleOffset.Value);  // 次回のための更新
    }
}
