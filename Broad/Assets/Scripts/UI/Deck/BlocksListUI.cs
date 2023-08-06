using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

public partial class BlocksListUI : MonoBehaviour
{
    [Header("ブロックの画像")]
    [SerializeField] Sprite m_BlockSprite;

    [Header("影の距離")]
    [SerializeField] Vector2 m_ShadowDistance;

    [Header("ゲームパッドでのスクロール速度")]
    [SerializeField] float m_GamePadScrollRate = 1.5f;
    [SerializeField] float m_ScrollFollowTime = 0.2f;

    [Space(20)]
    [Header("コンポーネント・プレファブ")]
    [SerializeField] GameObject      m_ButtonPrefab;
    [SerializeField] DeckListUI      m_DeckListUI;
    [SerializeField] GridLayoutGroup m_Content;
    [SerializeField] Scrollbar       m_Scrollbar;
    [SerializeField] ScrollRect      m_ScrollRect;
    [SerializeField] SwitchingUIs    m_SwitchingUIs;

    [HideInInspector] public List<ButtonBlocksUI> blocksList;

    [ReadOnly] public int editingDeckIndex = 0;

    RectTransform m_ScrollRectTransform;

    ButtonBlocksUI lastSelectedButton;

    // Start is called before the first frame update
    void Start()
    {
        blocksList = new List<ButtonBlocksUI>();
        m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();

        // ボタンの配置
        for (int i = 0; i < SaveSystem.data.blocksList.Count; ++i) blocksList.Add(AddContent());

        // カーソル移動先の設定
        SetupNavigation();
    }

    private void Update()
    {
        var selection = GetSelection();

        if (selection == null) return;
        if (Gamepad.current == null) return;

        float y = Gamepad.current.rightStick.ReadValue().y;

        // スクロール処理 (デッドゾーン: 0.05)
        if (y < -0.05f) m_ScrollRect.verticalNormalizedPosition = Mathf.Lerp(m_ScrollRect.verticalNormalizedPosition, y, m_GamePadScrollRate * Time.deltaTime);
        if (y >  0.05f) m_ScrollRect.verticalNormalizedPosition = Mathf.Lerp(m_ScrollRect.verticalNormalizedPosition, y + 1f, m_GamePadScrollRate * Time.deltaTime);

        // FollowRectのDOTween処理中はreturnして終了
        if (DOTween.IsTweening(m_Content)) return;

        // 描画範囲を移動先に合わせる
        FollowRect();

        // 選択座標が描画されていない位置だったら、選択項目を変更する
        if (!IsVisible(selection, true))
        {
            if(IsUpperPosition(selection)) selection.button.navigation.selectOnDown.Select();
            else                           selection.button.navigation.selectOnUp  .Select();
        }
    }

    ButtonBlocksUI AddContent()
    {
        int index = blocksList.Count;

        var blocksUI = Instantiate(m_ButtonPrefab).GetComponentInChildren<ButtonBlocksUI>();
        blocksUI.transform.parent.name = $"Blocks[{index}]";
        blocksUI.transform.parent.SetParent(m_Content.transform);

        // 位置を上方向に調整
        blocksUI.rectTransform.anchoredPosition = new Vector2(0f, 10f);

        // セーブデータからブロックスを取得
        Blocks blocks = SaveSystem.data.blocksList[index];
        if (blocks != null)
        {
            blocksUI.Setup(blocks, m_BlockSprite);
            blocksUI.SetupShadow(m_ShadowDistance);
        }

        // ボタンを押したときブロックスの決定をさせる
        blocksUI.button.onClick.AddListener(() => m_DeckListUI.OnDecisionBlocks(index));

        

        return blocksUI;
    }

    /// <summary>新規Navigationの取得（省略した項目は既存のNavigationに依存する）</summary>
    /// <param name="selectable">既存のNavigation</param>
    /// <param name="up">上方向（↑）</param>
    /// <param name="down">下方向（↓）</param>
    /// <param name="left">左方向（←）</param>
    /// <param name="right">右方向（→）</param>
    Navigation Nav(Selectable selectable = null, 
        Selectable up = null, Selectable down = null, Selectable left = null, Selectable right = null)
    {
        Navigation navigation = selectable != null ? selectable.navigation : new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp    = up    ?? navigation.selectOnUp;
        navigation.selectOnDown  = down  ?? navigation.selectOnDown;
        navigation.selectOnLeft  = left  ?? navigation.selectOnLeft;
        navigation.selectOnRight = right ?? navigation.selectOnRight;
        return navigation;
    }

    /// <summary>ボタンが描画範囲に存在するか</summary>
    /// <param name="ui">範囲内かを調べるボタン</param>
    /// <param name="whole">true: 全体が描画されているか
    /// <br></br>false: 一部でも描画されているか</param>
    /// 
    /// <returns>true: 範囲内
    /// <br></br>false: 範囲外</returns>
    bool IsVisible(ButtonBlocksUI ui, bool whole = false)
    {
        float selectTop = ui.buttonTransform.position.y;
        float selectBottom = ui.buttonTransform.position.y - ui.buttonTransform.rect.height;

        // 範囲内判定
        bool top    = (whole ? selectBottom : selectTop) <= m_ScrollRectTransform.position.y;
        bool bottom = m_ScrollRectTransform.position.y - m_ScrollRectTransform.rect.height <= (whole ? selectTop : selectBottom);

        // どちらも範囲内ならtrue
        return top && bottom;
    }

    /// <summary>描画範囲を移動先に合わせる</summary>
    void FollowRect()
    {
        if (EventSystem.current == null) return;

        var current = GetSelection();
        if (current == null) return;

        // 描画範囲の移動処理
        if (lastSelectedButton != current)
        {
            if (!IsVisible(current, false))
            {
                float height = current.buttonTransform.rect.height;

                height = IsUpperPosition(current) ? -height : height;

                m_Content.transform.DOMoveY(height, m_ScrollFollowTime).SetRelative();
            }
        }

        lastSelectedButton = current;
    }

    /// <summary>デッキ内のどこかのボタンが押されたとき</summary>
    /// <param name="index">押されたボタンの番号</param>
    public void OnSelectDeck(int index)
    {
        // イージング処理中はreturn
        if (DOTween.IsTweening(m_SwitchingUIs.blocksListTransform)) return;

        editingDeckIndex = index;

        m_SwitchingUIs.DoLeft();

        var obj = blocksList[m_DeckListUI.selectBlocksIndex].button.gameObject;
        EventSystem.current.SetSelectedGameObject(obj);
    }

    /// <summary>現在、どのブロックスを選択しているか</summary>
    /// <returns></returns>
    public ButtonBlocksUI GetSelection()
    {
        if (EventSystem.current == null) return null;

        var selection = EventSystem.current.currentSelectedGameObject;

        if (selection == null) return null;

        for (int i = 0; i < blocksList.Count; ++i)
            if (blocksList[i].button.gameObject == selection)
                return blocksList[i];

        return null;
    }

    bool IsUpperPosition(ButtonBlocksUI ui)
    {
        // 選択中のボタンの中心y座標
        float selectionCenter = ui.buttonTransform.position.y - ui.buttonTransform.rect.height / 2f;

        // 描画範囲の中心y座標
        float rectCenter = m_ScrollRectTransform.position.y - m_ScrollRectTransform.rect.height / 2f;

        // 選択したボタンが描画外なら、画面内の方向に選択を移動
        return selectionCenter > rectCenter;
    }
}
