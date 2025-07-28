using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LINE_", menuName = "Touhou Spell Up/Danmaku/Emission/Line")]
public class LineEmissionShape : EmissionShape
{
    [SerializeField] private Vector3 startOffset = new Vector3(-1f, 0, 0);  // 始点オフセット
    [SerializeField] private Vector3 endOffset = new Vector3(1f, 0, 0);     // 終点オフセット
    [SerializeField, Range(2, 100)] private int pointCount = 5;             // 分割数

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float step = 1f / (pointCount - 1);
        float currentAngle = baseAngleOffset;
        if (sharedAngle != null) currentAngle = sharedAngle.Value;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 localPos = Vector3.Lerp(startOffset, endOffset, i * step);
            float angle = currentAngle;

            switch (angleMode)
            {
                case AngleMode.AimToPlayer:
                    Vector3 worldPos = movable.transform.position + movable.transform.rotation * localPos;
                    angle = CalculateAimAngle(movable, worldPos);
                    break;
                case AngleMode.Radial:
                    angle += i * (360f / pointCount);  // 放射状例
                    break;
                // Fixedはangleそのまま
            }

            yield return new EmissionData { localPosition = localPos, localAngle = angle };
        }

        UpdateSharedAngle(currentAngle + baseAngleOffset);  // 次回のための更新
    }
}
