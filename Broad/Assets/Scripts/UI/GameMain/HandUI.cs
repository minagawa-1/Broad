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

    [Header("初期の高さ")]
    public float initHeight = 80f;

    [Chapter("コンポーネント")]
    [SerializeField] BlockManager m_BlockManager;

    public Hand hand;

    public List<MoveButton> moveButtonList;

    // Start is called before the first frame update
    void Awake()
    {
        Blocks[] deckBlocks = new Blocks[SaveSystem.saveData.deck.Length];
        moveButtonList = new List<MoveButton>();

        for (int i = 0; i < deckBlocks.Length; ++i) deckBlocks[i] = SaveSystem.saveData.deck[i];

        deckBlocks.Shuffle();

        hand = new Hand(new Deck(deckBlocks), GameSetting.hand_blocks);

        for (int i = 0; i < GameSetting.hand_blocks; ++i) BuildButton(i, true);
    }

    public void BuildButton(int handIndex, bool firstBuild = false)
    {
        // ボタン単体の横幅
        float width = buttonGroupWidth / GameSetting.hand_blocks;

        // ボタンの生成
        var button = Instantiate(m_ButtonPrefab.gameObject).GetComponent<MoveButton>();
        button.transform.SetParent(transform);

        // ボタンの横幅と位置を演算
        float x = handIndex * width;

        // ボタンの横幅を設定
        var rectTransform = button.GetComponent<RectTransform>();
        rectTransform.offsetMin = rectTransform.offsetMin.Setter(x: x);
        rectTransform.offsetMax = rectTransform.offsetMax.Setter(x: x + width);

        // 初期位置の設定
        bool selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == button.gameObject;

        // ボタンの名前を設定
        button.transform.GetChild(0).GetComponent<Text>().text = $"Blocks";

        // BlocksUI
        ButtonBlocksUI blocksUI = button.GetComponentInChildren<ButtonBlocksUI>();

        blocksUI.Setup(hand.GetBlocksAt(handIndex), m_BlockSprite, false, false);
        blocksUI.SetupShadow(m_ShadowDistance);

        moveButtonList.Add(button);
        

        // ボタンを押したときの処理
        {
            moveButtonList[handIndex].name = $"Button[{handIndex}]";

            moveButtonList[handIndex].basisPosition = moveButtonList[handIndex].rectTransform.position;

            // 開始時の移動表示DOTween
            moveButtonList[handIndex].rectTransform.DOMove(moveButtonList[handIndex].basisPosition.Setter(y: -40f), moveButtonList[handIndex].moveTime)
                .SetEase(Ease.InOutBack).SetDelay(firstBuild ? 0.06f * handIndex : 0f);

            moveButtonList[handIndex].onClick.AddListener(() => PlayAt(handIndex));
        }

        // ゲーム開始時の生成後は左端の手札ボタンを選択
        if(hand.deck.deck.Count == GameSetting.deck_blocks - GameSetting.hand_blocks)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtonList[0].gameObject);
    }

    /// <summary>手札から盤面にブロックスを生成するUI処理</summary>
    public void PlayAt(int handIndex)
    {
        // いずれかのボタンが操作不能・いずれかのボタンが移動処理中
        foreach (var mb in moveButtonList) if (!mb.interactable || DOTween.IsTweening(mb.gameObject)) return;

        Vector2Int pos = GameManager.boardSize / 2;

        var blocks = hand.GetBlocksAt(handIndex);

        // ブロックの生成
        m_BlockManager.CreateBlock(this, handIndex, blocks.shape, pos, blocks.density);

        // すべてのボタンを操作不能にする
        Uninteractate();
    }

    /// <summary>山札から手札にドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawAt(int handIndex)
    {
        // ① handIndexと一致するUIを下方向に移動
        // ② 手札UIのドロー処理
        Vector3 spos = moveButtonList[handIndex].basisPosition;
        Vector3 fpos = moveButtonList[handIndex].basisPosition.Setter(y: -40f);

        moveButtonList[handIndex].rectTransform.DOMove(spos, moveButtonList[handIndex].moveTime).SetEase(Ease.InCubic)
            .OnComplete(() => Rebuild(handIndex));
    }

    public void OnSet(int handIndex)
    {
        hand.hand[handIndex] = null;
    }

    public void Rebuild(int handIndex)
    {
        if (hand.deck.deck.Count == 0) return;

        Blocks blocks = hand.deck.Draw();

        hand.hand[handIndex] = blocks;

        // ボタンのブロックス画像を更新
        moveButtonList[handIndex].GetComponentInChildren<ButtonBlocksUI>().ResetBlocks(blocks);

        BuildButton(handIndex);
    }

    /// <summary>選択可能にする</summary>
    /// <param name="selectHandIndex">選択する手札番号</param>
    public void Interactate(int selectHandIndex)
    {
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtonList[i].interactable = true;

        Debug.Log($"-Count:{moveButtonList.Count}-------------------------------------------------");
        for (int i = 0; i < GameSetting.hand_blocks; ++i) Debug.Log($"\t{i}: {moveButtonList[i].gameObject}");
        Debug.Log("---------------------------------------------------------------");

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(moveButtonList[selectHandIndex].gameObject);
    }

    /// <summary>選択不可能にする</summary>
    public void Uninteractate()
    {
        // すべてのボタンを操作可能にする
        for (int i = 0; i < GameSetting.hand_blocks; ++i) moveButtonList[i].interactable = false;
    }
}
