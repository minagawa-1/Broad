using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>プログレスバーを超過したときの処理</summary>
[System.Serializable]
public enum ExcessType
{
    /// <summary>無処理</summary>
    None,

    /// <summary>範囲内に固定</summary>
    Clamp,

    /// <summary>過剰分を周回させる</summary>
    Around,
}


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

#if UNITY_EDITOR
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
                string warningText = $"{property.propertyType}は未対応です。対応: float, int";
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