using UnityEngine;

/// <summary>
/// 新システム用のAngleMode参照
/// </summary>
[System.Serializable]
public class NewAngleModeReference : GameParameterReference<AngleMode>
{
    public NewAngleModeReference()
    {
        useConstant = true;
        constantValue = AngleMode.Fixed;
    }
}

[System.Serializable]
public class EmissionTimingReference : GameParameterReference<EmissionTiming>
{
    public EmissionTimingReference()
    {
        useConstant = true;
        constantValue = EmissionTiming.Simultaneous;
    }
}

[System.Serializable]
public class LineOrderReference : GameParameterReference<LineOrder>
{
    public LineOrderReference()
    {
        useConstant = true;
        constantValue = LineOrder.Sequential;
    }
}

[System.Serializable]
public class PlaneOrderReference : GameParameterReference<PlaneOrder>
{
    public PlaneOrderReference()
    {
        useConstant = true;
        constantValue = PlaneOrder.Sequential;
    }
}

[System.Serializable]
public class EmissionBaseReference : GameParameterReference<EmissionBase>
{
    public EmissionBaseReference()
    {
        useConstant = true;
        constantValue = null;
    }
}
