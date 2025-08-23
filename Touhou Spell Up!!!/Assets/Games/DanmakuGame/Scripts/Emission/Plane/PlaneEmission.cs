using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 面発射パターン - 新階層システム版
/// </summary>
[CreateAssetMenu(fileName = "PlaneEmission_", menuName = "Danmaku/Pattern/Emission/New/Plane")]
public class PlaneEmission : PlaneEmissionBase
{
    [Header("面発射設定")]
    [Tooltip("面のサイズ（幅x高さ）")]
    [SerializeField] private Vector2Reference size = new Vector2Reference { useConstant = true, constantValue = new Vector2(2f, 2f) };
    
    [Tooltip("X方向の分割数")]
    [SerializeField] private IntReference countX = new IntReference { useConstant = true, constantValue = 3 };
    
    [Tooltip("Y方向の分割数")]
    [SerializeField] private IntReference countY = new IntReference { useConstant = true, constantValue = 3 };
    
    [Tooltip("ランダム配置を使用するか")]
    [SerializeField] private BoolReference randomPositions = new BoolReference { useConstant = true, constantValue = false };

    protected override IEnumerable<Vector3> GetPlanePositions(IMovable movable)
    {
        int xCount = Mathf.Max(1, countX.Value);
        int yCount = Mathf.Max(1, countY.Value);
        Vector2 planeSize = size.Value;

        if (randomPositions.Value)
        {
            // ランダム配置
            int totalCount = xCount * yCount;
            for (int i = 0; i < totalCount; i++)
            {
                float x = UnityEngine.Random.Range(-planeSize.x / 2, planeSize.x / 2);
                float y = UnityEngine.Random.Range(-planeSize.y / 2, planeSize.y / 2);
                yield return new Vector3(x, y, 0);
            }
        }
        else
        {
            // グリッド配置
            float stepX = xCount > 1 ? planeSize.x / (xCount - 1) : 0f;
            float stepY = yCount > 1 ? planeSize.y / (yCount - 1) : 0f;

            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    Vector3 position = new Vector3(
                        -planeSize.x / 2 + x * stepX,
                        -planeSize.y / 2 + y * stepY,
                        0
                    );
                    yield return position;
                }
            }
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
                // 面の場合、中心からの放射角度
                float radialAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
                angle += radialAngle;
                break;
        }

        return angle;
    }
}
