using UnityEditor;
using UnityEngine;

/// <summary>
/// GameParameterReferenceクラスとその派生クラスのためのカスタムPropertyDrawer。
/// Inspector上で、定数とGameParameter参照の切り替えを直感的に行えるようにする。
/// </summary>
[CustomPropertyDrawer(typeof(GameParameterReference<>), true)]
public class GameParameterReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // プロパティを取得
        var useConstant = property.FindPropertyRelative("useConstant");
        var constantValue = property.FindPropertyRelative("constantValue");
        var parameter = property.FindPropertyRelative("parameter");

        // ラベルを描画
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // ポップアップボタンのスタイル
        var popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
        {
            imagePosition = ImagePosition.ImageOnly
        };

        // ポップアップボタン用のRectを確保
        var buttonRect = new Rect(position.x, position.y, 20, position.height);
        position.xMin += buttonRect.width;

        // ポップアップメニューを表示
        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(""), FocusType.Keyboard, popupStyle))
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Use Constant"), useConstant.boolValue, () =>
            {
                useConstant.boolValue = true;
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("Use Reference"), !useConstant.boolValue, () =>
            {
                useConstant.boolValue = false;
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.ShowAsContext();
        }

        // useConstantの値に応じて、定数フィールドまたは参照フィールドを描画
        if (useConstant.boolValue)
        {
            EditorGUI.PropertyField(position, constantValue, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(position, parameter, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
