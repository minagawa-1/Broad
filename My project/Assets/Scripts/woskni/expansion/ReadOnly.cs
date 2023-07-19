using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace woskni
{
    public class ReadOnly : PropertyAttribute
    {
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(woskni.ReadOnly))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndDisabledGroup();
    }
}
#endif
