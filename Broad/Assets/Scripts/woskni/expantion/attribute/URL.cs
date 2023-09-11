using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class URLAttribute : PropertyAttribute
{
    public string url { get; private set; }

    /// <summary>フィールド上にコメントを付加</summary>
    /// <param name="comment">コメントテキスト</param>
    /// <param name="space">上のフィールドとコメントの余白幅</param>
    public URLAttribute(string url)
    {
        this.url = url;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(URLAttribute))]
public class URLDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        URLAttribute urlAttribute = (URLAttribute)attribute;

        float line = EditorGUIUtility.singleLineHeight * 1.2f;

        // プロパティフィールドの幅
        Rect fieldRect = new Rect(position.x, position.y, position.width - line, position.height);
        Rect buttonRect = new Rect(position.x + position.width - line, position.y, line, position.height);

        // プロパティフィールドを表示
        EditorGUI.PropertyField(fieldRect, property, label);

        GUIStyle helpButtonStyle = EditorStyles.miniButton;

        if (GUI.Button(buttonRect, new GUIContent("？", $"以下のURLを開きます:\n{urlAttribute.url}"), helpButtonStyle))
            Application.OpenURL(urlAttribute.url);

        EditorGUI.EndProperty();
    }
}
#endif