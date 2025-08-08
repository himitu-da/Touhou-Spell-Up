using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 他のGameParameterの値から計算によって動的に値を決定するゲームパラメータ。
/// （注意：このクラスは現在プレースホルダーであり、計算ロジックは未実装です）
/// </summary>
[CreateAssetMenu(fileName = "GPC_Calc_", menuName = "Danmaku/GameParameter/Calculated/Float (Placeholder)")]
public class CalculatedParameter : GameParameter<float>
{
    [Header("計算に使用するパラメータ")]
    [SerializeField]
    private List<GameParameter<float>> sourceParameters;

    [Header("計算式（将来的に実装）")]
    [Tooltip("将来的に、ここで数式（例: param[0] * param[1] + 2.0）を定義できるようにする")]
    [SerializeField]
    private string formula;

    public override void Reset()
    {
        // 計算の起点となるパラメータもリセットするのが望ましい場合がある
        foreach (var p in sourceParameters)
        {
            if (p != null) p.Reset();
        }
        // このパラメータ自体の値も初期化する
        // (計算ロジック実装時に、初期値の計算を行う)
        base.Reset();
    }

    // TODO:
    // 実行時に毎フレーム、あるいは値が要求されたタイミングで、
    // sourceParametersとformulaを元にcurrentValueを計算するロジックを実装する必要がある。
    // NCalcやRoslynなどの数式パーサーライブラリの導入を検討する。
}
