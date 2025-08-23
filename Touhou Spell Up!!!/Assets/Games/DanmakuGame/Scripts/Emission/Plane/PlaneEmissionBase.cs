using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 面系発射の基底クラス
/// </summary>
public abstract class PlaneEmissionBase : EmissionBase
{
    [Header("面制御")]
    [Tooltip("面発射の順序")]
    [SerializeField] protected PlaneOrderReference planeOrder = new PlaneOrderReference { useConstant = true, constantValue = PlaneOrder.Sequential };

    // 抽象メソッド: 面上の位置リストを取得
    protected abstract IEnumerable<Vector3> GetPlanePositions(IMovable movable);

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        var positions = GetPlanePositions(movable).ToList();
        var emissions = new List<EmissionData>();
        
        float currentAngle = baseAngleOffset.Value;
        if (sharedAngle != null && sharedAngle.Value != null) 
            currentAngle = sharedAngle.Value.Value;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 localPos = positions[i];
            float angle = CalculateAngleForPosition(movable, localPos, i, positions.Count);
            
            emissions.Add(new EmissionData { localPosition = localPos, localAngle = angle });
        }

        UpdateSharedAngle(currentAngle + baseAngleOffset.Value);
        
        // 面順序制御を適用
        return ApplyPlaneOrder(emissions, positions);
    }
    
    // 位置とインデックスに対する角度を計算
    protected virtual float CalculateAngleForPosition(IMovable movable, Vector3 position, int index, int totalCount)
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
    
    // 面順序制御を適用
    protected virtual IEnumerable<EmissionData> ApplyPlaneOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        if (randomizeOrder.Value)
        {
            return ApplyRandomOrder(emissions);
        }
        
        switch (planeOrder.Value)
        {
            case PlaneOrder.Simultaneous:
                return emissions;
            case PlaneOrder.Sequential:
                return emissions;
            case PlaneOrder.Spiral:
                return ApplySpiralOrder(emissions, positions);
            case PlaneOrder.Radial:
                return ApplyRadialOrder(emissions, positions);
            case PlaneOrder.RadialReverse:
                return ApplyRadialOrder(emissions, positions).Reverse();
            case PlaneOrder.Wave:
                return ApplyWaveOrder(emissions, positions);
            case PlaneOrder.Checkerboard:
                return ApplyCheckerboardOrder(emissions, positions);
            case PlaneOrder.Diagonal:
                return ApplyDiagonalOrder(emissions, positions);
            case PlaneOrder.Random:
                return ApplyRandomOrder(emissions);
            default:
                return emissions;
        }
    }
    
    // 螺旋順序の適用
    protected virtual IEnumerable<EmissionData> ApplySpiralOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        var indexed = emissions.Zip(positions, (emission, position) => new { emission, position })
            .Select((item, index) => new { 
                item.emission, 
                item.position, 
                index,
                angle = Mathf.Atan2(item.position.y, item.position.x),
                distance = item.position.magnitude 
            })
            .OrderBy(x => x.distance)
            .ThenBy(x => x.angle)
            .Select(x => x.emission);
            
        return indexed;
    }
    
    // 放射状順序の適用
    protected virtual IEnumerable<EmissionData> ApplyRadialOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        var indexed = emissions.Zip(positions, (emission, position) => new { emission, position })
            .OrderBy(x => x.position.magnitude)
            .Select(x => x.emission);
            
        return indexed;
    }
    
    // 波状順序の適用
    protected virtual IEnumerable<EmissionData> ApplyWaveOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        var indexed = emissions.Zip(positions, (emission, position) => new { emission, position })
            .OrderBy(x => x.position.y)
            .ThenBy(x => x.position.x)
            .Select(x => x.emission);
            
        return indexed;
    }
    
    // 市松模様順序の適用
    protected virtual IEnumerable<EmissionData> ApplyCheckerboardOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        var indexed = emissions.Zip(positions, (emission, position) => new { emission, position })
            .Select((item, index) => new { 
                item.emission, 
                item.position, 
                index,
                isEven = (Mathf.FloorToInt(item.position.x) + Mathf.FloorToInt(item.position.y)) % 2 == 0
            });
            
        var evenFirst = indexed.Where(x => x.isEven).Select(x => x.emission);
        var oddSecond = indexed.Where(x => !x.isEven).Select(x => x.emission);
        
        return evenFirst.Concat(oddSecond);
    }
    
    // 対角線順序の適用
    protected virtual IEnumerable<EmissionData> ApplyDiagonalOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        var indexed = emissions.Zip(positions, (emission, position) => new { emission, position })
            .OrderBy(x => x.position.x + x.position.y)
            .ThenBy(x => x.position.x)
            .Select(x => x.emission);
            
        return indexed;
    }
}
