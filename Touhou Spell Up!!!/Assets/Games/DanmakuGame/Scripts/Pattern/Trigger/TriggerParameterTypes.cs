using UnityEngine;

/// <summary>
/// ParameterTriggerPatternで使用する具体的なGameParameter型のエイリアス
/// </summary>

/// <summary>
/// float型のゲームパラメータ
/// </summary>
public class FloatParameter : GameParameter<float> { }

/// <summary>
/// int型のゲームパラメータ
/// </summary>
public class IntParameter : GameParameter<int> { }

/// <summary>
/// bool型のゲームパラメータ
/// </summary>
public class BoolParameter : GameParameter<bool> { }

/// <summary>
/// Vector3型のゲームパラメータ
/// </summary>
public class Vector3Parameter : GameParameter<Vector3> { }
