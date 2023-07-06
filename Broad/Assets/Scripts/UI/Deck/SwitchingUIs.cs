using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SwitchingUIs : MonoBehaviour
{
    [SerializeField] float m_SwitchTime;

    [SerializeField] Ease m_OpenEase;
    [SerializeField] Ease m_CloseEase;

    [Header("動かすためのデッキ・ブロックリストのRectTransform")]
    public RectTransform decksTransform;
    public RectTransform blocksListTransform;
    [SerializeField] RectTransform m_LeftDeckGroup;
    [SerializeField] RectTransform m_RightDeckGroup;
    [SerializeField] Vector2 m_DecksMoveDirection;
    [SerializeField] Vector2 m_BlocksListMoveDirection;
    [SerializeField] Vector2 m_DecksButtonScaleDirection;

    [Header("シーン遷移用に保持するコンポーネント")]
    [SerializeField] AudioListener m_AudioListener;
    [SerializeField] UnityEngine.EventSystems.EventSystem m_EventSystem;

    Vector3 m_BasisPosition;

    RectTransform m_RectTransform;

    TitleState m_TitleState;

    Vector3 m_MoveDistance;

    // Start is called before the first frame update
    void Start()
    {
        m_TitleState = FindObjectOfType<TitleState>();

        m_RectTransform = GetComponent<RectTransform>();
        m_BasisPosition = m_RectTransform.position;

        m_MoveDistance.y = m_RectTransform.localPosition.y + m_RectTransform.sizeDelta.y;

        m_RectTransform.position = m_BasisPosition + m_MoveDistance;

        DoOpen();
    }

    public void DoOpen()
    {
        m_AudioListener.enabled = true;
        m_EventSystem.enabled = true;

        m_RectTransform.DOMove(m_BasisPosition, m_SwitchTime).SetEase(m_OpenEase);
    }

    public void DoClose(UnityEngine.UI.Text text)
    {
        text.rectTransform.DoHighlight();

        m_AudioListener.enabled = false;
        m_EventSystem.enabled = false;

        m_RectTransform.DOMove(m_BasisPosition + m_MoveDistance, m_SwitchTime).SetEase(m_CloseEase)
            .OnComplete(() =>
            {
                m_TitleState.OnClosedDeckScene();

                SceneManager.UnloadSceneAsync(Scene.DeckScene);
            });
    }

    public void DoLeft()
    {
        decksTransform     .DOSizeDelta(-m_DecksMoveDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
        blocksListTransform.DOSizeDelta( m_DecksMoveDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);

        m_LeftDeckGroup .DOSizeDelta(-m_DecksButtonScaleDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
        m_RightDeckGroup.DOSizeDelta(-m_DecksButtonScaleDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
    }

    public void DoRight()
    {
        decksTransform     .DOSizeDelta( m_DecksMoveDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
        blocksListTransform.DOSizeDelta(-m_DecksMoveDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);

        m_LeftDeckGroup .DOSizeDelta(m_DecksButtonScaleDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
        m_RightDeckGroup.DOSizeDelta(m_DecksButtonScaleDirection, m_SwitchTime).SetRelative().SetEase(m_OpenEase);
    }
}
