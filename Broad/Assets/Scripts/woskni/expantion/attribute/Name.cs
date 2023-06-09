using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class NameAttribute : PropertyAttribute
{
    public readonly string label;

    /// <summary>フィールド名の表示を変更</summary>
    /// <param name="label">表示させる文字列</param>
    public NameAttribute(string label = "") => this.label = label;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NameAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    /// <summary>エディタ上に反映させる</summary>
    /// <param name="position">表示位置</param>
    /// <param name="property">プロパティ</param>
    /// <param name="label">文字内容</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = ((NameAttribute)attribute).label;

        // エディタ上の表示をここで変換
        EditorGUI.PropertyField(position, property, label, true);
    }

    /// <summary>プロパティ自体の高さを取得</summary>
    /// <param name="property">プロパティ</param>
    /// <param name="label">文字内容</param>
    /// <returns>プロパティ自体の高さ</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
#endif
