using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(RectTransform))]
public class MoveButton : Button
{
    public Vector2 moveDistance = new Vector2(0f, 380f);
    public float moveTime = 0.25f;

    public RectTransform rectTransform;

    public Vector2 basisPosition;

    protected override void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        base.Start();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoveButton))]
public class MoveButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var component = (MoveButton)target;

        EditorGUILayout.Space(10);

        component.moveDistance = EditorGUILayout.Vector2Field("移動距離", component.moveDistance);
        component.moveTime     = EditorGUILayout.FloatField  ("移動時間", component.moveTime);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif