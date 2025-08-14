using UnityEngine;

/// <summary>
/// 固定値を表すジェネリックなゲームパラメータ。
/// Inspectorから直接値を設定して使用します。
/// </summary>
/// <typeparam name="T">扱う値の型</typeparam>
public class ConstantParameter<T> : GameParameter<T>
{
    // 機能はすべて基底クラスのGameParameter<T>に実装されているため、
    // このクラスは型を具体化し、CreateAssetMenuを提供するために存在する。
    // 今後、定数値に特化したロジックが必要であればここに追加する。
}

// ジェネリッククラスは直接CreateAssetMenuをつけられないため、具体的な型ごとにクラスを定義する

// もしConstantParameterをScriptableObjectとして使用したい場合は、各クラスを独立したファイルに保存する（ファイル分割）
// 現状は独立させる予定はないので、このままにした上で、menuNameはコメントアウトする
// 将来的に定数を変数として扱いたくなった場合は、ファイル分割をして、各型ごとに個別のクラスを作成する


/// <summary>
/// float型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_Float_", menuName = "Danmaku/GameParameter/Constant/Float")]
public class ConstantFloatParameter : ConstantParameter<float> { }

/// <summary>
/// int型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_Int_"/*, menuName = "Danmaku/GameParameter/Constant/Int"*/)]
public class ConstantIntParameter : ConstantParameter<int> { }

/// <summary>
/// string型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_String_"/*, menuName = "Danmaku/GameParameter/Constant/String"*/)]
public class ConstantStringParameter : ConstantParameter<string> { }

/// <summary>
/// Vector2型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_Vector2_"/*, menuName = "Danmaku/GameParameter/Constant/Vector2"*/)]
public class ConstantVector2Parameter : ConstantParameter<Vector2> { }

/// <summary>
/// Vector3型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_Vector3_"/*, menuName = "Danmaku/GameParameter/Constant/Vector3"*/)]
public class ConstantVector3Parameter : ConstantParameter<Vector3> { }

/// <summary>
/// bool型の固定値を扱うゲームパラメータ
/// </summary>
//[CreateAssetMenu(fileName = "GPC_Bool_"/*, menuName = "Danmaku/GameParameter/Constant/Bool"*/)]
public class ConstantBoolParameter : ConstantParameter<bool> { }
