using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// シンプルなポイント発射パターン
/// </summary>
[CreateAssetMenu(fileName = "SimplePointEmission_", menuName = "Danmaku/Pattern/Emission/Point/Simple")]
public class SimplePointEmission : PointEmissionBase
{
    [Header("シンプルポイント設定")]
    [Tooltip("発射位置のオフセット")]
    [SerializeField] private Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };

    protected override IEnumerable<Vector3> GetPointPositions(IMovable movable)
    {
        yield return positionOffset.Value;
    }
}
