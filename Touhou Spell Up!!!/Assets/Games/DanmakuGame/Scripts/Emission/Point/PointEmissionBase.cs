using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ポイント系発射の基底クラス
/// </summary>
public abstract class PointEmissionBase : EmissionBase
{
    // 抽象メソッド: ポイントの位置リストを取得
    protected abstract IEnumerable<Vector3> GetPointPositions(IMovable movable);

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        var positions = GetPointPositions(movable);
        var emissions = new List<EmissionData>();
        
        float currentAngle = baseAngleOffset.Value;
        if (sharedAngle != null && sharedAngle.Value != null) 
            currentAngle = sharedAngle.Value.Value;

        int index = 0;
        foreach (Vector3 localPos in positions)
        {
            float angle = CalculateAngleForPosition(movable, localPos, index, -1); // totalCountは後で計算
            emissions.Add(new EmissionData { localPosition = localPos, localAngle = angle });
            index++;
        }

        UpdateSharedAngle(currentAngle + baseAngleOffset.Value);
        
        // 重複除去
        if (!allowDuplicates.Value)
        {
            emissions = RemoveDuplicates(emissions).ToList();
        }
        
        // ランダム順序
        if (randomizeOrder.Value)
        {
            return ApplyRandomOrder(emissions);
        }

        return emissions;
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
                // ポイントの場合、位置ベースの放射角度
                float radialAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
                angle += radialAngle;
                break;
        }
        
        return angle;
    }
}
