using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ライン系発射の基底クラス
/// </summary>
public abstract class LineEmissionBase : EmissionBase
{
    [Header("ライン制御")]
    [Tooltip("ライン発射の順序")]
    [SerializeField] protected LineOrderReference lineOrder = new LineOrderReference { useConstant = true, constantValue = LineOrder.Sequential };

    // 抽象メソッド: ライン上の位置リストを取得
    protected abstract IEnumerable<Vector3> GetLinePositions(IMovable movable);

    public override IEnumerable<EmissionData> GetEmissions(IMovable movable)
    {
        var positions = GetLinePositions(movable).ToList();
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
        
        // ライン順序制御を適用
        return ApplyLineOrder(emissions, positions);
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
                // ラインの場合、ライン方向を考慮
                float radialAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
                angle += radialAngle;
                break;
        }
        
        return angle;
    }
    
    // ライン順序制御を適用
    protected virtual IEnumerable<EmissionData> ApplyLineOrder(List<EmissionData> emissions, List<Vector3> positions)
    {
        if (randomizeOrder.Value)
        {
            return ApplyRandomOrder(emissions);
        }
        
        switch (lineOrder.Value)
        {
            case LineOrder.Simultaneous:
                return emissions;
            case LineOrder.Sequential:
                return emissions; // 既に順次順序
            case LineOrder.Skip:
                return ApplySkipOrder(emissions, 1);
            case LineOrder.Skip2:
                return ApplySkipOrder(emissions, 2);
            case LineOrder.CenterOut:
                return ApplyCenterOutOrder(emissions);
            case LineOrder.CenterIn:
                return ApplyCenterInOrder(emissions);
            case LineOrder.ToCenter:
                return ApplyToCenterOrder(emissions);
            case LineOrder.Alternating:
                return ApplyAlternatingOrder(emissions);
            default:
                return emissions;
        }
    }
    
    // スキップ順序の適用
    protected virtual IEnumerable<EmissionData> ApplySkipOrder(List<EmissionData> emissions, int skipCount)
    {
        // まず奇数インデックス、次に偶数インデックス（skipCount=1の場合）
        var odds = emissions.Where((item, index) => index % (skipCount + 1) == 0);
        var evens = emissions.Where((item, index) => index % (skipCount + 1) != 0);
        return odds.Concat(evens);
    }
    
    // 中央から外側への順序
    protected virtual IEnumerable<EmissionData> ApplyCenterOutOrder(List<EmissionData> emissions)
    {
        int center = emissions.Count / 2;
        var result = new List<EmissionData>();
        
        for (int i = 0; i <= center; i++)
        {
            if (center - i >= 0) result.Add(emissions[center - i]);
            if (center + i < emissions.Count && i > 0) result.Add(emissions[center + i]);
        }
        
        return result;
    }
    
    // 外側から中央への順序
    protected virtual IEnumerable<EmissionData> ApplyCenterInOrder(List<EmissionData> emissions)
    {
        return ApplyCenterOutOrder(emissions).Reverse();
    }
    
    // 両端から中央への順序
    protected virtual IEnumerable<EmissionData> ApplyToCenterOrder(List<EmissionData> emissions)
    {
        var result = new List<EmissionData>();
        int left = 0, right = emissions.Count - 1;
        
        while (left <= right)
        {
            result.Add(emissions[left++]);
            if (left <= right) result.Add(emissions[right--]);
        }
        
        return result;
    }
    
    // 交互順序の適用
    protected virtual IEnumerable<EmissionData> ApplyAlternatingOrder(List<EmissionData> emissions)
    {
        var odds = emissions.Where((item, index) => index % 2 == 1);
        var evens = emissions.Where((item, index) => index % 2 == 0);
        return evens.Concat(odds);
    }
}
