using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleLayout))]
public class CircleLayoutInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var angleIntervalProp = serializedObject.FindProperty("angleInterval");
        var radiusProp = serializedObject.FindProperty("radius");

        using (var changeCheck = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.PropertyField(angleIntervalProp);
            EditorGUILayout.PropertyField(radiusProp);

            if (changeCheck.changed)
            {
                (target as CircleLayout).ManualUpdateLayout();

                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("GenarateChild"))
                (target as CircleLayout).Alignment();
        }
    }
}
