using UnityEngine;

public class GameEntityState
{
    public Vector3 Position;
    public Quaternion Rotation = Quaternion.identity;
    public Vector3 Velocity;
    public Vector3 InitialScale = Vector3.one;
    public Vector3 ScaleMultiplier = Vector3.one;
}
