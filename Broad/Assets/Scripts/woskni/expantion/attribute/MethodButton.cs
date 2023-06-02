using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MethodButtonAttribute : PropertyAttribute
{
    public (string method, string buttonTitle)[] m_Buttons;

    /// <summary>���\�b�h�����s����{�^�����C���X�y�N�^�ɕ\��</summary>
    /// <param name="method">���s���郁�\�b�h (��: new string[] { �֐���1, �֐���2 })</param>
    /// <param name="buttonTitle">�{�^���ɕ\������e�L�X�g (�ȗ�: ���\�b�h��)</param>
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

        // �{�^���S�̂�Rect��ݒ肷��
        Rect rect = EditorGUI.IndentedRect(position);


        float interval = 0f;

        if (buttonAttribute.m_Buttons.Length > 1)
            interval = 20f / buttonAttribute.m_Buttons.Length;

        float buttonWidth = rect.width / buttonAttribute.m_Buttons.Length;

        for (int i = 0; i < buttonAttribute.m_Buttons.Length; ++i)
        {
            // �e�{�^���������тɕ`��
            Rect buttonRect = new Rect(rect.x + (i * buttonWidth), rect.y, buttonWidth - interval, rect.height);

            if (GUI.Button(buttonRect, buttonAttribute.m_Buttons[i].buttonTitle))
            {
                try
                {
                    // ���\�b�h�����烁�\�b�h���擾�A���s
                    MethodInfo method = property.serializedObject.targetObject.GetType().GetMethod(buttonAttribute.m_Buttons[i].method);
                    method.Invoke(property.serializedObject.targetObject, null);
                }
                catch
                {
                    Debug.LogError(buttonAttribute.m_Buttons[i].method + " �����s�ł��܂���");
                }
            }
        }
    }
}
#endif