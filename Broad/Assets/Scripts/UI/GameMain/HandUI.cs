using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class HandUI : MonoBehaviour
{
    [Chapter("UI")]
    [SerializeField] MoveButton m_ButtonPrefab;
    [SerializeField] Sprite m_BlockSprite;

    [Header("ブロックスの影")]
    [SerializeField] Vector2 m_ShadowDistance = Vector2.zero;

    [Header("手札全体の横幅")]
    public float buttonGroupWidth = Screen.width * 0.8f;

    [Chapter("コンポーネント")]
    [SerializeField] BlockManager m_BlockManager;

    Deck m_Deck;
    Hand m_Hand;

    public MoveButton[] moveButtons;

    // Start is called before the first frame update
    void Awake()
    {
        Blocks[] deck = new Blocks[SaveSystem.data.deck.Length];
        moveButtons = new MoveButton[GameSetting.hand_blocks];

        for (int i = 0; i < deck.Length; ++i) deck[i] = SaveSystem.data.deck[i];

        m_Deck = new Deck(deck);
        m_Hand = new Hand(m_Deck, GameSetting.hand_blocks);

        BuildButton();
    }

    void BuildButton()
    {
        // ボタンの生成
        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            var button = Instantiate(m_ButtonPrefab.gameObject).GetComponent<MoveButton>();

            button.gameObject.name = $"Button[{i}]";
            button.transform.SetParent(transform);

            var rectTransform = button.GetComponent<RectTransform>();

            // ボタンの横幅と位置を演算
            float width = buttonGroupWidth / GameSetting.hand_blocks;
            float x = i * width;

            // ボタンの横幅を設定
            rectTransform.offsetMin = rectTransform.offsetMin.Setter(x: x);
            rectTransform.offsetMax = rectTransform.offsetMax.Setter(x: x + width);

            // ボタンの名前を設定
            button.transform.GetChild(0).GetComponent<Text>().text = $"Blocks[{i}]";

            // BlocksUI
            ButtonBlocksUI blocksUI = button.GetComponentInChildren<ButtonBlocksUI>();

            blocksUI.Setup(m_Hand.GetBlocksAt(i), m_BlockSprite, false, false);
            blocksUI.SetupShadow(m_ShadowDistance);

            moveButtons[i] = button;
        }

        // ボタンを押したときの処理
        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            int handIndex = i;
            moveButtons[i].onClick.AddListener(() => DrawHandToBoard(handIndex));
        }

        // 左端の手札ボタンを選択
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtons[0].gameObject);
    }

    /// <summary>盤面にブロックスを生成するUI処理</summary>
    public void DrawHandToBoard(int handIndex)
    {
        // いずれかのボタンが操作不能・いずれかのボタンが移動処理中
        foreach (var mb in moveButtons) if (!mb.interactable || DOTween.IsTweening(mb.gameObject)) return;

        Vector2Int pos = GameManager.boardSize / 2;

        var blocks = m_Hand.GetBlocksAt(handIndex);

        // ブロックの生成
        m_BlockManager.CreateBlock(this, handIndex, blocks.shape, pos, blocks.density);

        // すべてのボタンを操作不能にする
        Uninteractate();
    }

    /// <summary>山札からドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawDeckToHand(int handIndex)
    {
        moveButtons[handIndex].DoMove(new Vector2(moveButtons[handIndex].basisPosition.x, 0f), Ease.InOutCubic, Replace);

        void Replace()
        {
            m_Hand.SetBlocksAt(handIndex, m_Deck.Draw());

            moveButtons[handIndex].DoMove(moveButtons[handIndex].basisPosition, Ease.InOutCubic);
        }
    }

    public void Interactate()
    {
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtons[i].Interactate();
    }

    public void Uninteractate()
    {
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtons[i].Uninteractate(true);
    }
}
