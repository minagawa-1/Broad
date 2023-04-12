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

        /// <summary>�t�B�[���h����\�����Ȃ�</summary>
        public Name() => m_Label = "";

        /// <summary>�t�B�[���h���̕\����ύX</summary>
        /// <param name="label">�\�������镶����</param>
        public Name(string label) => m_Label = label;
    }
}

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(woskni.Name))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        /// <summary>�G�f�B�^��ɔ��f������</summary>
        /// <param name="position">�\���ʒu</param>
        /// <param name="property">�v���p�e�B</param>
        /// <param name="label">�������e</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // PropertyDrawer��attribute��Name�N���X�ɕϊ����đ��
            woskni.Name newLabel = attribute as woskni.Name;

            // �G�f�B�^��̕\���������ŕϊ�
            EditorGUI.PropertyField(position, property, new GUIContent(newLabel.m_Label), true);
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
