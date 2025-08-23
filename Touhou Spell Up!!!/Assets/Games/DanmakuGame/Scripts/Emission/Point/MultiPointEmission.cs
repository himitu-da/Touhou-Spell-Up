using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 複数ポイント発射パターン
/// </summary>
[CreateAssetMenu(fileName = "MultiPointEmission_", menuName = "Danmaku/Pattern/Emission/Point/Multi")]
public class MultiPointEmission : PointEmissionBase
{
    [Header("マルチポイント設定")]
    [Tooltip("発射ポイントのリスト")]
    [SerializeField] private Vector3[] points = new Vector3[] 
    {
        new Vector3(-1f, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(1f, 0, 0)
    };
    
    [Tooltip("ランダムに配置するか")]
    [SerializeField] private BoolReference randomizePositions = new BoolReference { useConstant = true, constantValue = false };
    
    [Tooltip("ランダム配置時の範囲")]
    [SerializeField] private Vector3Reference randomRange = new Vector3Reference { useConstant = true, constantValue = new Vector3(2f, 2f, 0) };

    protected override IEnumerable<Vector3> GetPointPositions(IMovable movable)
    {
        if (randomizePositions.Value)
        {
            // ランダム配置
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-randomRange.Value.x / 2, randomRange.Value.x / 2),
                    UnityEngine.Random.Range(-randomRange.Value.y / 2, randomRange.Value.y / 2),
                    UnityEngine.Random.Range(-randomRange.Value.z / 2, randomRange.Value.z / 2)
                );
                yield return points[i] + randomOffset;
            }
        }
        else
        {
            // 固定配置
            foreach (Vector3 point in points)
            {
                yield return point;
            }
        }
    }
}
