using UnityEngine;

/// <summary>
/// GameParameterへの参照、または定数を保持するためのジェネリックな基底クラス。
/// [System.Serializable]属性により、Inspectorで表示・編集が可能になる。
/// </summary>
/// <typeparam name="T">扱う値の型</typeparam>
[System.Serializable]
public abstract class GameParameterReference<T>
{
    [SerializeField]
    public bool useConstant = true;

    [SerializeField]
    public T constantValue;

    [SerializeField]
    public GameParameter<T> parameter;

    /// <summary>
    /// 現在の有効な値を取得する。
    /// useConstantがtrueの場合は定数を、falseの場合は参照しているGameParameterの値を返す。
    /// </summary>
    public T Value
    {
        get { return useConstant ? constantValue : (parameter != null ? parameter.Value : default(T)); }
    }
}

// 以下、具体的な型のための参照クラス

[System.Serializable]
public class FloatReference : GameParameterReference<float> { }

[System.Serializable]
public class IntReference : GameParameterReference<int> { }

[System.Serializable]
public class StringReference : GameParameterReference<string> { }

[System.Serializable]
public class Vector2Reference : GameParameterReference<Vector2> { }

[System.Serializable]
public class Vector3Reference : GameParameterReference<Vector3> { }

[System.Serializable]
public class BoolReference : GameParameterReference<bool> { }
