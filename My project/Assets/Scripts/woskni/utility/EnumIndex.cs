using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace woskni
{
    /// <summary>配列・リストの要素名を列挙子にする</summary>
    public class EnumIndexAttribute : PropertyAttribute
    {
        // 列挙子を格納する文字列配列
        private string[] m_Names;

        /// <summary>コンストラクタ</summary>
        /// <param name="enumType">列挙型を指定 (typeofで括る)</param>
        public EnumIndexAttribute(Type enumType) => m_Names = System.Enum.GetNames(enumType);

        /// <summary>コンストラクタ</summary>
        /// <param name="names">文字列リスト</param>
        public EnumIndexAttribute(params object[] names) => m_Names = (string[])names;

#if UNITY_EDITOR
        /// <summary>インスペクタ表示の処理</summary>
        [CustomPropertyDrawer(typeof(EnumIndexAttribute))]
        private class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                string[] names = ((EnumIndexAttribute)attribute).m_Names;

                // 配列・リストの添字を取得する (propertyPathからは、配列名"hoge.m_Array[0]"のような文字列が返る)
                int index = int.Parse(property.propertyPath.Split('[', ']').Where(c => !string.IsNullOrEmpty(c)).Last());

                // 添字が列挙子数未満のとき、index番目の列挙子をラベルテキストに設定
                if (index < names.Length) label.text = names[index];

                EditorGUI.PropertyField(rect, property, label, includeChildren: true);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
            }
        }
#endif
    }
}