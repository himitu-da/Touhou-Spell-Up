/// <summary>
/// 発射タイミングの種類
/// </summary>
public enum EmissionTiming
{
    /// <summary>同時発射 - すべての弾が同時に発射される</summary>
    Simultaneous,
    
    /// <summary>順次発射 - 順番に一つずつ発射される</summary>
    Sequential,
    
    /// <summary>ランダム発射 - ランダムな順序で発射される</summary>
    Random,
    
    /// <summary>バースト発射 - 短い間隔で連続発射される</summary>
    Burst,
    
    /// <summary>波状発射 - 波のような間隔で発射される</summary>
    Wave,
    
    /// <summary>カスケード発射 - 滝のように連続して発射される</summary>
    Cascade,
    
    /// <summary>リップル発射 - 波紋のように広がって発射される</summary>
    Ripple,
    
    /// <summary>交互発射 - 交互のパターンで発射される</summary>
    Alternating
}
