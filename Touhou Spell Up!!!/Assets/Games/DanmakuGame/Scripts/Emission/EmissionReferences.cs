using UnityEngine;

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
