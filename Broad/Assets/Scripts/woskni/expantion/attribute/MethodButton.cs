using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MethodButtonAttribute : PropertyAttribute
{
    public (string method, string buttonTitle)[] m_Buttons;

    /// <summary>メソッドを実行するボタンをインスペクタに表示</summary>
    /// <param name="method">実行するメソッド (例: new string[] { 関数名1, 関数名2 })</param>
    /// <param name="buttonTitle">ボタンに表示するテキスト (省略: メソッド名)</param>
    public MethodButtonAttribute(string[] method, string[] buttonTitle = null)
    {
        m_Buttons = new (string method, string buttonTitle)[method.Length];

        for (int i = 0; i < m_Buttons.Length; ++i)
        {
            m_Buttons[i].method = method[i];

            bool existTitle = buttonTitle != null && i <= buttonTitle.Length - 1;

            m_Buttons[i].buttonTitle = existTitle ? buttonTitle[i] : method[i];
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MethodButtonAttribute))]
public class MethodButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MethodButtonAttribute buttonAttribute = attribute as MethodButtonAttribute;

        // ボタン全体のRectを設定する
        Rect rect = EditorGUI.IndentedRect(position);


        float interval = 0f;

        if (buttonAttribute.m_Buttons.Length > 1)
            interval = 20f / buttonAttribute.m_Buttons.Length;

        float buttonWidth = rect.width / buttonAttribute.m_Buttons.Length;

        for (int i = 0; i < buttonAttribute.m_Buttons.Length; ++i)
        {
            // 各ボタンを横並びに描画
            Rect buttonRect = new Rect(rect.x + (i * buttonWidth), rect.y, buttonWidth - interval, rect.height);

            if (GUI.Button(buttonRect, buttonAttribute.m_Buttons[i].buttonTitle))
            {
                try
                {
                    // メソッド名からメソッドを取得、実行
                    MethodInfo method = property.serializedObject.targetObject.GetType().GetMethod(buttonAttribute.m_Buttons[i].method);
                    method.Invoke(property.serializedObject.targetObject, null);
                }
                catch
                {
                    Debug.LogError(buttonAttribute.m_Buttons[i].method + " を実行できません");
                }
            }
        }
    }
}
#endif