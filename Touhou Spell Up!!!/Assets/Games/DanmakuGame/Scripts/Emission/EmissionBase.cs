using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 角度モードの種類
/// </summary>
public enum AngleMode 
{ 
    /// <summary>固定角度</summary>
    Fixed, 
    /// <summary>各ポイントで自機狙い</summary>
    AimToPlayer, 
    /// <summary>放射状（中心からの角度）</summary>
    Radial 
}

/// <summary>
/// 発射形状の新基底クラス - 拡張されたタイミング制御と階層構造を提供
/// </summary>
public abstract class EmissionBase : ScriptableObject
{
    [Header("基本設定")]
    [Tooltip("共有角度パラメータ（オプション）")]
    [SerializeField] protected AngleParameterReference sharedAngle;
    
    [Tooltip("角度モード")]
    [SerializeField] protected NewAngleModeReference angleMode = new NewAngleModeReference { useConstant = true, constantValue = AngleMode.Fixed };
    
    [Tooltip("基本角度オフセット")]
    [SerializeField] protected FloatReference baseAngleOffset = new FloatReference { useConstant = true, constantValue = 0f };

    [Header("タイミング制御")]
    [Tooltip("発射タイミングの種類")]
    [SerializeField] protected EmissionTimingReference emissionTiming = new EmissionTimingReference { useConstant = true, constantValue = EmissionTiming.Simultaneous };
    
    [Tooltip("発射間隔（Sequential等で使用）")]
    [SerializeField] protected FloatReference emissionInterval = new FloatReference { useConstant = true, constantValue = 0.1f };
    
    [Tooltip("発射順序をランダム化するか")]
    [SerializeField] protected BoolReference randomizeOrder = new BoolReference { useConstant = true, constantValue = false };
    
    [Tooltip("重複発射を許可するか")]
    [SerializeField] protected BoolReference allowDuplicates = new BoolReference { useConstant = true, constantValue = false };

    // 抽象メソッド: 発射データを生成
    public abstract IEnumerable<EmissionData> GetEmissions(IMovable movable);

    // 自機狙い角度を計算するヘルパー
    protected float CalculateAimAngle(IMovable movable, Vector3 worldPosition)
    {
        // TODO: 実際の自機狙い計算を実装
        return 0f;
    }

    // 共有角度の更新
    protected void UpdateSharedAngle(float newValue)
    {
        if (sharedAngle != null && sharedAngle.Value != null)
        {
            sharedAngle.Value.Value = newValue;
        }
    }

    // ランダム順序の適用
    protected virtual IEnumerable<EmissionData> ApplyRandomOrder(List<EmissionData> emissions)
    {
        var shuffled = emissions.ToList();
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }
        return shuffled;
    }

    // 重複除去（位置ベース）
    protected virtual IEnumerable<EmissionData> RemoveDuplicates(IEnumerable<EmissionData> emissions)
    {
        var seen = new HashSet<Vector3>();
        foreach (var emission in emissions)
        {
            if (seen.Add(emission.localPosition))
            {
                yield return emission;
            }
        }
    }
}
