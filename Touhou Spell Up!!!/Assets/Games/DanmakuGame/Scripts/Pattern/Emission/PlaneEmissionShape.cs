using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PLANE_", menuName = "Danmaku/Pattern/Emission/Plane")]
public class PlaneEmissionShape : EmissionShape
{
    [SerializeField] private Vector2Reference size = new Vector2Reference { useConstant = true, constantValue = new Vector2(2f, 2f) };            // 幅x高さ
    [SerializeField] private IntReference countX = new IntReference { useConstant = true, constantValue = 3 };                 // X方向分割
    [SerializeField] private IntReference countY = new IntReference { useConstant = true, constantValue = 3 };                 // Y方向分割
    [SerializeField] private BoolReference randomPositions = new BoolReference { useConstant = true, constantValue = false };                 // ランダム配置か

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        float stepX = (countX.Value > 1) ? size.Value.x / (countX.Value - 1) : 0;
        float stepY = (countY.Value > 1) ? size.Value.y / (countY.Value - 1) : 0;
        float currentAngle = baseAngleOffset.Value;
        if (sharedAngle != null && sharedAngle.Value != null) currentAngle = sharedAngle.Value.Value;

        for (int y = 0; y < countY.Value; y++)
        {
            for (int x = 0; x < countX.Value; x++)
            {
                Vector3 localPos = new Vector3(
                    -size.Value.x / 2 + (randomPositions.Value ? Random.Range(0f, size.Value.x) : x * stepX),
                    -size.Value.y / 2 + (randomPositions.Value ? Random.Range(0f, size.Value.y) : y * stepY),
                    0
                );
                float angle = currentAngle;

                // angleModeに応じた計算（Line同様）
                switch (angleMode.Value)
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

        UpdateSharedAngle(currentAngle + baseAngleOffset.Value);
    }
}
