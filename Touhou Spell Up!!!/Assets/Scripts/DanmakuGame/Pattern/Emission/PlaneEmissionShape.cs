using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PLANE_", menuName = "Danmaku/Pattern/Emission/Plane")]
public class PlaneEmissionShape : EmissionShape
{
    [SerializeField] private Vector2 size = new Vector2(2f, 2f);            // 幅x高さ
    [SerializeField, Range(1, 50)] private int countX = 3;                 // X方向分割
    [SerializeField, Range(1, 50)] private int countY = 3;                 // Y方向分割
    [SerializeField] private bool randomPositions = false;                 // ランダム配置か

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float stepX = (countX > 1) ? size.x / (countX - 1) : 0;
        float stepY = (countY > 1) ? size.y / (countY - 1) : 0;
        float currentAngle = baseAngleOffset;
        if (sharedAngle != null) currentAngle = sharedAngle.Value;

        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                Vector3 localPos = new Vector3(
                    -size.x / 2 + (randomPositions ? Random.Range(0f, size.x) : x * stepX),
                    -size.y / 2 + (randomPositions ? Random.Range(0f, size.y) : y * stepY),
                    0
                );
                float angle = currentAngle;

                // angleModeに応じた計算（Line同様）
                switch (angleMode)
                {
                    case AngleMode.AimToPlayer:
                        Vector3 worldPos = movable.transform.position + movable.transform.rotation * localPos;
                        angle = CalculateAimAngle(movable, worldPos);
                        break;
                    case AngleMode.Radial:
                        // 面での放射状は複雑なので、ここでは単純なオフセットに留めるか、
                        // 中心からの角度として計算するなど、別途定義が必要
                        break;
                }

                yield return new EmissionData { localPosition = localPos, localAngle = angle };
            }
        }

        UpdateSharedAngle(currentAngle + baseAngleOffset);
    }
}
