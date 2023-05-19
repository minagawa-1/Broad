using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class NameAttribute : PropertyAttribute
{
    public readonly string label;

    /// <summary>�t�B�[���h���̕\����ύX</summary>
    /// <param name="label">�\�������镶����</param>
    public NameAttribute(string label = "") => this.label = label;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NameAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    /// <summary>�G�f�B�^��ɔ��f������</summary>
    /// <param name="position">�\���ʒu</param>
    /// <param name="property">�v���p�e�B</param>
    /// <param name="label">�������e</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = ((NameAttribute)attribute).label;

        // �G�f�B�^��̕\���������ŕϊ�
        EditorGUI.PropertyField(position, property, label, true);
    }

    /// <summary>�v���p�e�B���̂̍������擾</summary>
    /// <param name="property">�v���p�e�B</param>
    /// <param name="label">�������e</param>
    /// <returns>�v���p�e�B���̂̍���</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
#endif
