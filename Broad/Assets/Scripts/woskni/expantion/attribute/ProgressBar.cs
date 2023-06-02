using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>�v���O���X�o�[�𒴉߂����Ƃ��̏���</summary>
[System.Serializable]
public enum ExcessType
{
    /// <summary>������</summary>
    None,

    /// <summary>�͈͓��ɌŒ�</summary>
    Clamp,

    /// <summary>�ߏ蕪�����񂳂���</summary>
    Around,
}

#if UNITY_EDITOR
public class ProgressBarAttribute : PropertyAttribute
{
    public float min;
    public float max;
    public ExcessType type;

    public ProgressBarAttribute(float min, float max, ExcessType type = ExcessType.None)
    {
        this.min = min;
        this.max = max;
        this.type = type;
    }
}

[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
public class ProgressBarDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ProgressBarAttribute progressBarAttribute = (ProgressBarAttribute)attribute;
        float min = progressBarAttribute.min;
        float max = progressBarAttribute.max;


        float value;

        switch(property.propertyType)
        {
            case SerializedPropertyType.Float:   value = property.floatValue;  break;
            case SerializedPropertyType.Integer: value = property.intValue;    break;

            default:
                string warningText = $"{property.propertyType}�͖��Ή��ł��B�Ή�: float, int";
                EditorGUI.LabelField(position, label.text, warningText);
                return;
        }

        value = GetExcessed(value, min, max, progressBarAttribute.type);

        EditorGUI.ProgressBar(position, Mathf.Clamp01(((float)value - min) / (max - min)), label.text);

        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:   property.floatValue =      value; break;
            case SerializedPropertyType.Integer: property.intValue   = (int)value; break;
        }
    }

    public float GetExcessed(float value, float min, float max, ExcessType type)
    {
        switch (type)
        {
            case ExcessType.None: return value;
            case ExcessType.Clamp: return Mathf.Clamp(value, min, max);
            case ExcessType.Around: return value < min ? value + (max - min) : value > max ? value - (max - min) : value;
            default: return float.NaN;
        }
    }
}
#endif