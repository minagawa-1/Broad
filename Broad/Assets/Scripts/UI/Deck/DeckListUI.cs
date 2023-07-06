using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DeckListUI : MonoBehaviour
{
    [Header("影の距離")]
    [SerializeField] Vector2 m_ShadowDistance;

    [Header("ブロックの画像")]
    [SerializeField] Sprite m_BlockSprite;

    [Space(20)]
    [Header("コンポーネント・プレファブ")]
    [SerializeField] GameObject          m_ButtonPrefab;
    [SerializeField] BlocksListUI        m_BlocksListUI;
    [SerializeField] VerticalLayoutGroup m_ContentLeft;
    [SerializeField] VerticalLayoutGroup m_ContentRight;
    [SerializeField] Button              m_ReturnButton;
    [SerializeField] SwitchingUIs        m_SwitchingUIs;

    [HideInInspector] public ButtonBlocksUI[] deckUIs;

    [ReadOnly] public int selectBlocksIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        deckUIs = new ButtonBlocksUI[GameSetting.deck_blocks];

        for (int i = 0; i < GameSetting.deck_blocks; ++i) AddContent(i);

        // カーソル移動先の設定
        SetupNavigation();

        // 初期選択の設定
        EventSystem.current.firstSelectedGameObject = deckUIs[0].transform.parent.gameObject;
    }

    void AddContent(int index)
    {
        var blocksUI = Instantiate(m_ButtonPrefab).GetComponentInChildren<ButtonBlocksUI>();
        blocksUI.transform.parent.name = $"Blocks[{index}]";

        bool isLeft = (float)index / GameSetting.deck_blocks < 0.5f;
        blocksUI.transform.parent.SetParent(isLeft ? m_ContentLeft.transform : m_ContentRight.transform);

        Blocks blocks = SaveSystem.saveData.deck[index];

        if (blocks.shape != null)
        {
            blocksUI.Setup(blocks, m_BlockSprite);
            blocksUI.SetupShadow(m_ShadowDistance);
        }
        else
        {
            blocksUI.Setup(null, null);
        }

        blocksUI.button.onClick.AddListener(() => m_BlocksListUI.OnSelectDeck(index));

        deckUIs[index] = blocksUI;
    }

    /// <summary>どのブロックスを設定するかが決定したとき</summary>
    /// <param name="index">押されたボタンの番号</param>
    public void OnDecisionBlocks(int index)
    {
        // イージング処理中はreturn
        if (DOTween.IsTweening(m_SwitchingUIs.decksTransform)) return;

        // ブロックスの番号を取得して格納
        selectBlocksIndex = index;

        m_SwitchingUIs.DoRight();

        SetBlocks(m_BlocksListUI.editingDeckIndex, selectBlocksIndex);

        SaveSystem.Save();
    }

    void SetBlocks(int deckIndex, int blocksIndex)
    {
        // 設定するブロックス情報
        var blocks = m_BlocksListUI.blocksList[blocksIndex].blocks;

        // デッキ内の同じ番号のブロックスを列挙
        var duplicates = deckUIs.Where(ui => ui.blocks.index == blocks.index).ToArray();

        Debug.Log(duplicates.Length);

        // 入れ替え (もともとデッキに入っていたブロックスと入れ替え)
        for(int i = 0; i < duplicates.Length; ++i)
        {
            Debug.Log(duplicates[i].blocks.index);

            duplicates[i].Setup(deckUIs[deckIndex].blocks, m_BlockSprite);
            duplicates[i].SetupShadow(m_ShadowDistance);
        }

        deckUIs[deckIndex].Setup(blocks, m_BlockSprite);
        deckUIs[deckIndex].SetupShadow(m_ShadowDistance);

        EventSystem.current.SetSelectedGameObject(deckUIs[deckIndex].button.gameObject);
    }

    /// <summary>現在、どのブロックスを選択しているか</summary>
    public ButtonBlocksUI GetSelection()
    {
        if (EventSystem.current == null) return null;

        var select = EventSystem.current.currentSelectedGameObject;

        if (select == null) return null;

        for (int i = 0; i < deckUIs.Length; ++i)
            if (deckUIs[i].gameObject == select) return deckUIs[i];

        return null;
    }


    void SetupNavigation()
    {
        int n = GameSetting.deck_blocks / 2;

        // ブロックスのNavigationをリセット
        for (int i = 0; i < GameSetting.deck_blocks; ++i) deckUIs[i].button.navigation = new Navigation();

        // 戻るボタン→ブロックスのNavigationを設定
        m_ReturnButton.navigation         = Nav(m_ReturnButton        , down: deckUIs[0].button);

        // 上下のNavigation（最上列）
        for (int i = 0; i < 2; ++i)
            deckUIs[i * n].button.navigation = Nav(deckUIs[i * n].button, up: m_ReturnButton, down: deckUIs[i * n + 1].button);

        // 上下のNavigation（中間）
        for (int i = 1; i < n - 1; ++i)
        {
            // 上へのNavigationを設定
            deckUIs[i    ].button.navigation = Nav(deckUIs[i    ].button, up  : deckUIs[i     - 1].button);
            deckUIs[i + n].button.navigation = Nav(deckUIs[i + n].button, up  : deckUIs[i + n - 1].button);

            // 下へのNavigationを設定
            deckUIs[i    ].button.navigation = Nav(deckUIs[i    ].button, down: deckUIs[i     + 1].button);
            deckUIs[i + n].button.navigation = Nav(deckUIs[i + n].button, down: deckUIs[i + n + 1].button);
        }

        // 上下のNavigation（最下列）
        for (int i = 0; i < 2; ++i)
            deckUIs[i * n + (n - 1)].button.navigation = Nav(deckUIs[i * n + (n - 1)].button, up: deckUIs[i * n + (n - 1) - 1].button);


        // 左右のNavigation
        for (int i = 0; i < n; ++i)
        {
            deckUIs[i    ].button.navigation = Nav(deckUIs[i    ].button, right: deckUIs[i + n].button);
            deckUIs[i + n].button.navigation = Nav(deckUIs[i + n].button, left : deckUIs[i    ].button);
        }
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
}
