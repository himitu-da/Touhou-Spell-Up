using UnityEditor;
using UnityEngine;

// GameParameter<T>のジェネリックな性質に対応するため、
// カスタムエディタの対象を 'GameParameter' とし、'true' を渡して派生クラスにも適用する
[CustomEditor(typeof(GameParameter), true)]
public class GameParameterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ターゲットオブジェクト
        var gameParameter = (GameParameter)target;

        // デフォルトのインスペクタを描画
        base.OnInspectorGUI();

        // GUIが有効な場合のみボタンを表示
        if (GUI.enabled)
        {
            // スペースを追加
            EditorGUILayout.Space();

            // リセットボタンを追加
            if (GUILayout.Button("Reset Value"))
            {
                gameParameter.Reset();
            }
        }
    }
}
