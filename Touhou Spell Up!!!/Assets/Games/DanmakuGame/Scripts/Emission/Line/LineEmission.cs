using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ライン発射パターン - 新階層システム版
/// </summary>
[CreateAssetMenu(fileName = "LineEmission_", menuName = "Danmaku/Pattern/Emission/Line/Basic")]
public class LineEmission : LineEmissionBase
{
    [Header("ライン発射設定")]
    [Tooltip("ラインの開始点オフセット")]
    [SerializeField] private Vector3Reference startOffset = new Vector3Reference { useConstant = true, constantValue = new Vector3(-1f, 0, 0) };
    
    [Tooltip("ラインの終了点オフセット")]
    [SerializeField] private Vector3Reference endOffset = new Vector3Reference { useConstant = true, constantValue = new Vector3(1f, 0, 0) };
    
    [Tooltip("ライン上の発射点数")]
    [SerializeField] private IntReference pointCount = new IntReference { useConstant = true, constantValue = 5 };

    protected override IEnumerable<Vector3> GetLinePositions(IMovable movable)
    {
        int count = Mathf.Max(1, pointCount.Value);
        Vector3 start = startOffset.Value;
        Vector3 end = endOffset.Value;

        if (count == 1)
        {
            // 1点の場合は中点を返す
            yield return Vector3.Lerp(start, end, 0.5f);
        }
        else
        {
            // 複数点の場合は等間隔で配置
            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0f;
                yield return Vector3.Lerp(start, end, t);
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
                // ラインの場合、ライン方向に垂直な角度を基準とする
                Vector3 lineDirection = (endOffset.Value - startOffset.Value).normalized;
                float lineAngle = Mathf.Atan2(lineDirection.y, lineDirection.x) * Mathf.Rad2Deg;
                angle += lineAngle + 90f; // 垂直方向
                break;
        }

        return angle;
    }
}
