using UnityEngine;

[System.Serializable]
public struct EmissionData
{
    public Vector3 localPosition;  // ローカルオフセット（基準位置からの相対）
    public float localAngle;       // ローカル角度オフセット（基準角度からの相対）
}
