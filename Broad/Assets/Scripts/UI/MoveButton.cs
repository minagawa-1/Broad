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
    public Vector2 moveDistance = new Vector2(0f, 300f);
    public float moveTime = 0.25f;

    public RectTransform rectTransform;

    public Vector2 basisPosition;

    protected override void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        basisPosition = rectTransform.position;

        base.Start();
    }

    // 選択開始
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        DoMove(basisPosition + moveDistance);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        DoMove(basisPosition + moveDistance);
    }

    // 選択終了
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        DoMove(basisPosition);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        DoMove(basisPosition);
    }

    /// <summary>移動処理</summary>
    /// <param name="position">移動する位置</param>
    /// <param name="ease">移動イージング</param>
    /// <param name="action">移動後に行う処理</param>
    public void DoMove(Vector2 position, Ease ease = Ease.Unset, System.Action action = null)
    {
        if (!interactable) return;

        rectTransform.DOMove(position, moveTime).SetEase(ease)
            .OnComplete(() => { if (action != null) action.Invoke(); });
    }

    /// <summary>操作不能にする</summary>
    /// <param name="move">basisPositionに移動するか</param>
    public void Uninteractate(bool move = true)
    {
        if (move) DoMove(basisPosition, action: () => interactable = false);
        else interactable = false;
    }

    /// <summary>操作可能にする</summary>
    public void Interactate()
    {
        interactable = true;
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