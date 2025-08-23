using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// シンプルなグリッド発射パターン
/// </summary>
[CreateAssetMenu(fileName = "SimpleGridEmission_", menuName = "Danmaku/Pattern/Emission/Plane/Grid")]
public class SimpleGridEmission : PlaneEmissionBase
{
    [Header("シンプルグリッド設定")]
    [Tooltip("グリッドのサイズ（幅x高さ）")]
    [SerializeField] private Vector2Reference gridSize = new Vector2Reference { useConstant = true, constantValue = new Vector2(2f, 2f) };
    
    [Tooltip("X方向の分割数")]
    [SerializeField] private IntReference countX = new IntReference { useConstant = true, constantValue = 3 };
    
    [Tooltip("Y方向の分割数")]
    [SerializeField] private IntReference countY = new IntReference { useConstant = true, constantValue = 3 };
    
    [Tooltip("中心オフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetPlanePositions(IMovable movable)
    {
        int xCount = Mathf.Max(1, countX.Value);
        int yCount = Mathf.Max(1, countY.Value);
        Vector2 size = gridSize.Value;
        Vector3 center = centerOffset.Value;

        float stepX = xCount > 1 ? size.x / (xCount - 1) : 0f;
        float stepY = yCount > 1 ? size.y / (yCount - 1) : 0f;

        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                Vector3 position = center + new Vector3(
                    -size.x / 2f + x * stepX,
                    -size.y / 2f + y * stepY,
                    0
                );
                yield return position;
            }
        }
    }
}
