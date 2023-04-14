using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace woskni
{
    public class Name : PropertyAttribute
    {
        public readonly string m_Label;

        /// <summary>フィールド名を表示しない</summary>
        public Name() => m_Label = "";

        /// <summary>フィールド名の表示を変更</summary>
        /// <param name="label">表示させる文字列</param>
        public Name(string label) => m_Label = label;
    }
}

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(woskni.Name))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        /// <summary>エディタ上に反映させる</summary>
        /// <param name="position">表示位置</param>
        /// <param name="property">プロパティ</param>
        /// <param name="label">文字内容</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // PropertyDrawerのattributeをNameクラスに変換して代入
            woskni.Name newLabel = attribute as woskni.Name;

            // エディタ上の表示をここで変換
            EditorGUI.PropertyField(position, property, new GUIContent(newLabel.m_Label), true);
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
