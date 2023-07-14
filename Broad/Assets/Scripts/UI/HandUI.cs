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

    MoveButton[] m_MoveButtons;

    // Start is called before the first frame update
    void Awake()
    {
        Blocks[] deck = new Blocks[SaveSystem.saveData.deck.Length];
        m_MoveButtons = new MoveButton[GameSetting.hand_blocks];

        for (int i = 0; i < deck.Length; ++i) deck[i] = LotteryBlocks.Lottery();

        m_Deck = new Deck(deck);
        m_Hand = new Hand(m_Deck, GameSetting.hand_blocks);

        BuildButton();
    }

    void BuildButton()
    {
        Color playerColor = GameSetting.instance.playerColors[GameSetting.instance.selfIndex];

        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            // ボタンの生成
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

            m_MoveButtons[i] = button;
        }

        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            // ボタンを押したときの処理
            int handIndex = i;
            m_MoveButtons[i].onClick.AddListener(() => DrawHandToBoard(handIndex));
        }

        // 左端の手札ボタンを選択
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_MoveButtons[0].gameObject);
    }

    /// <summary>盤面にブロックスを生成するUI処理</summary>
    public void DrawHandToBoard(int handIndex)
    {
        // いずれかのボタンが操作不能・いずれかのボタンが移動処理中
        foreach (var mb in m_MoveButtons) if (!mb.interactable || DOTween.IsTweening(mb.gameObject)) return;

        Vector2Int pos = GameManager.boardSize / 2;

        Debug.Log("山札の数: " + m_Deck.deck.Count);
        Debug.Log("手札の数: " + m_Hand.hand.Length);

        var blocks = m_Hand.PlayAt(handIndex);

        m_BlockManager.CreateBlock(GameSetting.instance.selfIndex, blocks.shape, pos, blocks.density);

        // すべてのボタンを操作不能にする
        for(int i = 0; i < GameSetting.hand_blocks; ++i) m_MoveButtons[i].Uninteractate(true);
    }

    /// <summary>山札からドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawDeckToHand(int handIndex)
    {
        m_MoveButtons[handIndex].DoMove(new Vector2(m_MoveButtons[handIndex].basisPosition.x, 0f), Ease.InOutCubic, Replace);

        void Replace()
        {
            m_Hand.SetBlocksAt(handIndex, m_Deck.Draw());

            m_MoveButtons[handIndex].DoMove(m_MoveButtons[handIndex].basisPosition, Ease.InOutCubic);
        }
    }

    private void Update()
    {
        // 右クリック or Yボタン or □ボタン
        if(Mouse.current.rightButton.wasPressedThisFrame || Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            // すべてのボタンを操作可能にする
            for (int i = 0; i < GameSetting.hand_blocks; ++i) m_MoveButtons[i].Interactate();

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_MoveButtons[0].gameObject);
        }
    }
}
