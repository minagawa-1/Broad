using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

[RequireComponent(typeof(RectTransform))]
public class SelectCursor : MonoBehaviour
{
    class TweenInfo
    {
        public TweenerCore<Vector3, Vector3, VectorOptions> move      = null;
        public TweenerCore<Vector2, Vector2, VectorOptions> sizeDelta = null;
        public TweenerCore<Vector3, Vector3, VectorOptions> scale     = null;
        public TweenerCore<Vector2, Vector2, VectorOptions> pivot     = null;

        public RectTransform tweening = null;

        public void DOSelectChange(RectTransform @this, RectTransform end, float time, Ease ease)
        {
            // すでにDOTween中だったら
            if (DOTween.IsTweening(@this))
            {
                if (move      != null) move     .Kill();
                if (sizeDelta != null) sizeDelta.Kill();
                if (scale     != null) scale    .Kill();
                if (pivot     != null) pivot    .Kill();
            }

            tweening = @this;

            move      = tweening.DOMove     (end.position  , time).SetEase(ease);
            sizeDelta = tweening.DOSizeDelta(end.sizeDelta , time).SetEase(ease);
            scale     = tweening.DOScale    (end.localScale, time).SetEase(ease);
            pivot     = tweening.DOPivot    (end.pivot     , time).SetEase(ease);
        }

        public void UpdateEndValue(RectTransform end)
        {
            move      .ChangeEndValue(end.position  , true);
            sizeDelta .ChangeEndValue(end.sizeDelta , true);
            scale     .ChangeEndValue(end.localScale, true);
            pivot     .ChangeEndValue(end.pivot     , true);
        }
    }

    [SerializeField] float m_MoveTime;

    GameObject lastSelectedGameObject;

    RectTransform m_RectTransform;
    RectTransform m_EndTransform;

    TweenInfo m_Tween;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        transform.SetParent(Transition.instance.fadeCanvasGroup.transform.parent);
        transform.SetAsFirstSibling();

        m_RectTransform = GetComponent<RectTransform>();

        m_Tween = new TweenInfo();
    }

    void Update()
    {
        if (EventSystem.current == null) return;

        var current = EventSystem.current.currentSelectedGameObject;
        if (current == null) return;

        // カーソルの移動処理
        if (lastSelectedGameObject != current)
        {
            m_EndTransform = current.GetComponent<RectTransform>();

            // イージング前の位置にイージングしようとしている問題
            m_Tween.DOSelectChange(m_RectTransform, m_EndTransform, m_MoveTime, Ease.OutCubic);
        }

        if (DOTween.IsTweening(m_RectTransform)) m_Tween.UpdateEndValue(m_EndTransform);

        lastSelectedGameObject = current;
    }
}
