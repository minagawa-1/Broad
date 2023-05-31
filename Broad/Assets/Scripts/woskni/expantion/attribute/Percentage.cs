using UnityEngine;
using UnityEditor;

public class PercentageAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(PercentageAttribute))]
public class PercentageDrawer : PropertyDrawer
{
    private bool isEditing;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // �v���p�e�B�̒l���擾
        float value = property.floatValue;

        // ���̓t�B�[���h�̏�Ԃ��擾(null���Ԃ��ꂽ�炻�̂܂�)
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

        // ���N���b�N�������u��
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (position.Contains(e.mousePosition)) return true;
            
            GUI.changed = true;
            return false;
        }
        // Enter�L�[���������u��
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
        {
            GUI.changed = true;
            return false;
        }

        return null;
    }
}
