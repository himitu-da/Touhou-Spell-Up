using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// シンプルなライン発射パターン
/// </summary>
[CreateAssetMenu(fileName = "SimpleLineEmission_", menuName = "Danmaku/Pattern/Emission/Line/Simple")]
public class SimpleLineEmission : LineEmissionBase
{
    [Header("シンプルライン設定")]
    [Tooltip("ラインの長さ")]
    [SerializeField] private FloatReference lineLength = new FloatReference { useConstant = true, constantValue = 2f };
    
    [Tooltip("ライン上の発射点数")]
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 5 };
    
    [Tooltip("ラインの角度（度）")]
    [SerializeField] private FloatReference lineAngle = new FloatReference { useConstant = true, constantValue = 0f };
    
    [Tooltip("中心オフセット")]
    [SerializeField] private Vector3Reference centerOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetLinePositions(IMovable movable)
    {
        int count = Mathf.Max(1, pointCount.Value);
        float length = lineLength.Value;
        float angle = lineAngle.Value * Mathf.Deg2Rad;
        Vector3 center = centerOffset.Value;
        
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        Vector3 startPos = center - direction * (length / 2f);
        Vector3 endPos = center + direction * (length / 2f);

        if (count == 1)
        {
            yield return center;
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                yield return Vector3.Lerp(startPos, endPos, t);
            }
        }
    }
}
