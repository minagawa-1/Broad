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
public class ExtendButton : Button
{
    public Vector2 extendSize;
    public float extendTime;

    public RectTransform rectTransform;

    public Vector2 basisSize;

    protected override void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        basisSize = rectTransform.sizeDelta;

        base.Start();
    }

// �I���J�n
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        DoExtend(basisSize + extendSize);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        DoExtend(basisSize + extendSize);
    }

    // �I���I��
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        DoExtend(basisSize);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        DoExtend(basisSize);
    }

    public void DoExtend(Vector2 size, Ease ease = Ease.Unset, System.Action action = null)
    {
        rectTransform.DOSizeDelta(size, extendTime).SetEase(ease)
            .OnComplete(() => { if (action != null) action.Invoke(); });
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ExtendButton))]
public class ExtendButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var component = (ExtendButton)target;

        EditorGUILayout.Space(10);

        component.extendSize = EditorGUILayout.Vector2Field("�L�k����", component.extendSize);
        component.extendTime = EditorGUILayout.FloatField  ("�L�k����", component.extendTime);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif