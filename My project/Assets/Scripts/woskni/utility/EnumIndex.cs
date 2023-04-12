using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace woskni
{
    /// <summary>�z��E���X�g�̗v�f����񋓎q�ɂ���</summary>
    public class EnumIndexAttribute : PropertyAttribute
    {
        // �񋓎q���i�[���镶����z��
        private string[] m_Names;

        /// <summary>�R���X�g���N�^</summary>
        /// <param name="enumType">�񋓌^���w�� (typeof�Ŋ���)</param>
        public EnumIndexAttribute(Type enumType) => m_Names = System.Enum.GetNames(enumType);

        /// <summary>�R���X�g���N�^</summary>
        /// <param name="names">�����񃊃X�g</param>
        public EnumIndexAttribute(params object[] names) => m_Names = (string[])names;

#if UNITY_EDITOR
        /// <summary>�C���X�y�N�^�\���̏���</summary>
        [CustomPropertyDrawer(typeof(EnumIndexAttribute))]
        private class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                string[] names = ((EnumIndexAttribute)attribute).m_Names;

                // �z��E���X�g�̓Y�����擾���� (propertyPath����́A�z��"hoge.m_Array[0]"�̂悤�ȕ����񂪕Ԃ�)
                int index = int.Parse(property.propertyPath.Split('[', ']').Where(c => !string.IsNullOrEmpty(c)).Last());

                // �Y�����񋓎q�������̂Ƃ��Aindex�Ԗڂ̗񋓎q�����x���e�L�X�g�ɐݒ�
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