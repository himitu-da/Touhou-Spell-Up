/// <summary>
/// 面発射の順序制御
/// </summary>
public enum PlaneOrder
{
    /// <summary>同時発射</summary>
    Simultaneous,
    
    /// <summary>順次発射（左から右、上から下）</summary>
    Sequential,
    
    /// <summary>螺旋状発射</summary>
    Spiral,
    
    /// <summary>中心から放射状発射</summary>
    Radial,
    
    /// <summary>外側から中心へ放射状発射</summary>
    RadialReverse,
    
    /// <summary>波状発射</summary>
    Wave,
    
    /// <summary>市松模様発射</summary>
    Checkerboard,
    
    /// <summary>対角線発射</summary>
    Diagonal,
    
    /// <summary>ランダム発射</summary>
    Random
}
