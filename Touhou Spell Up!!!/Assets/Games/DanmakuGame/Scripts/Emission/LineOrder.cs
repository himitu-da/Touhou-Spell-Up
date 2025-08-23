/// <summary>
/// ライン発射の順序制御
/// </summary>
public enum LineOrder
{
    /// <summary>同時発射</summary>
    Simultaneous,
    
    /// <summary>順次発射（開始点から終了点へ）</summary>
    Sequential,
    
    /// <summary>1つおきに発射</summary>
    Skip,
    
    /// <summary>2つおきに発射</summary>
    Skip2,
    
    /// <summary>中央から外側へ</summary>
    CenterOut,
    
    /// <summary>外側から中央へ</summary>
    CenterIn,
    
    /// <summary>両端から中央へ</summary>
    ToCenter,
    
    /// <summary>交互発射（奇数・偶数）</summary>
    Alternating
}
