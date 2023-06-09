using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PercentageAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PercentageAttribute))]
public class PercentageDrawer : PropertyDrawer
{
    private bool isEditing;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // プロパティの値を取得
        float value = property.floatValue;

        // 入力フィールドの状態を取得(nullが返されたらそのまま)
        isEditing = HandleInputEvents(position, property) ?? isEditing;

        if (!isEditing)
            EditorGUI.TextField(EditorGUI.PrefixLabel(position, label), $"{value * 100f}%");
        else
        {
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUI.FloatField(position, label, value);

            if (EditorGUI.EndChangeCheck()) property.floatValue = newValue;
        }

        EditorGUI.EndProperty();
    }

    private bool? HandleInputEvents(Rect position, SerializedProperty property)
    {
        Event e = Event.current;

        // 左クリックをした瞬間
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (position.Contains(e.mousePosition)) return true;
            
            GUI.changed = true;
            return false;
        }
        // Enterキーを押した瞬間
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
        {
            GUI.changed = true;
            return false;
        }

        return null;
    }
}
#endif