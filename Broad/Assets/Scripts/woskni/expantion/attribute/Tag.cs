using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>�^�O�̎w��</summary>
/// <remarks>�Ή��f�[�^�^: int, string</remarks>
public class TagAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // string[]�^��int[]�^�ȊO�͌x����\�����ďI��
        if (property.propertyType != SerializedPropertyType.Integer && property.propertyType != SerializedPropertyType.String)
        {
            string warningText = $"{property.propertyType}�͖��Ή��ł��B�Ή�: int[], string[]";
            EditorGUI.LabelField(position, label.text, warningText);
            return;
        }

        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        int index = 0;
        switch (property.propertyType) {
            case SerializedPropertyType.Integer: index = property.intValue;                                break;
            case SerializedPropertyType.String:  index = System.Array.IndexOf(tags, property.stringValue); break;
            default: break;
        }

        // 0 �` tags.Length ���Ɏ��߂�
        index = Mathf.Max(0, Mathf.Min(index, tags.Length - 1));

        EditorGUI.BeginChangeCheck();

        int newIndex = EditorGUI.Popup(position, label.text, index, tags);
        if (EditorGUI.EndChangeCheck())
        {
            switch (property.propertyType) {
                case SerializedPropertyType.Integer: property.intValue    =      newIndex ; break;
                case SerializedPropertyType.String:  property.stringValue = tags[newIndex]; break;
                default:                                                                    break;
            }
        }
    }
}
#endif

/// <summary>���C���[�̎w��</summary>
/// <remarks>�Ή��f�[�^�^: int, string</remarks>
public class LayerAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // string�^��int�^�ȊO�͌x����\�����ďI��
        if (property.propertyType != SerializedPropertyType.Integer && property.propertyType != SerializedPropertyType.String)
        {
            string warningText = $"{property.propertyType}�͖��Ή��ł��B�Ή�: int, string";
            EditorGUI.LabelField(position, label.text, warningText);
            return;
        }

        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;

        int index = 0;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer: index = property.intValue;                                  break;
            case SerializedPropertyType.String:  index = System.Array.IndexOf(layers, property.stringValue); break;
            default:                                                                                         break;
        }

        // 0 �` tags.Length ���Ɏ��߂�
        index = Mathf.Max(0, Mathf.Min(index, layers.Length - 1));

        EditorGUI.BeginChangeCheck();

        int newIndex = EditorGUI.Popup(position, label.text, index, layers);
        if (EditorGUI.EndChangeCheck())
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: property.intValue    =        newIndex ; break;
                case SerializedPropertyType.String:  property.stringValue = layers[newIndex]; break;
                default:                                                                     break;
            }
        }
    }
}
#endif