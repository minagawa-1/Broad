using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mirror;

[RequireComponent(typeof(RectTransform))]
public class HandUI : MonoBehaviour
{
    [Chapter("UI")]
    [SerializeField] Button  m_ButtonPrefab;
    [SerializeField] Sprite  m_BlockSprite;
    [SerializeField] PopupUI m_PopupUI;

    [Header("ブロックスの影")]
    [SerializeField] Vector2 m_ShadowDistance = Vector2.zero;

    [Header("手札全体の横幅")]
    public float buttonGroupWidth = Screen.width * 0.8f;

    [Header("初期の高さ")]
    public float initHeight = 80f;

    [Header("入れ替え時間")]
    public float changeTime = 0.25f;

    [Chapter("コンポーネント")]
    [SerializeField] BlockManager    m_BlockManager;
    [SerializeField] RemainingTurnUI m_TurnUI;

    public Hand hand;

    public class HandButton
    {
        public Button        button;
        public RectTransform rectTransform;
        public Vector3       basisPosition;
    }

    public List<HandButton> buttonList;
    public List<Text>       buttonTitleList;

    [ReadOnly] public int m_AddListenerCounter;

    // Start is called before the first frame update
    void Awake()
    {
        buttonList = new List<HandButton>();
        buttonTitleList = new List<Text>();

        Blocks[] deckBlocks = new Blocks[SaveSystem.data.deck.Length];

        for (int i = 0; i < deckBlocks.Length; ++i) deckBlocks[i] = SaveSystem.data.deck[i];

        deckBlocks.Shuffle();

        hand = new Hand(new Deck(deckBlocks), GameSetting.hand_blocks);

        for (int i = 0; i < GameSetting.hand_blocks; ++i) BuildButton(i, true);

        Interactate(0);
    }

    void Update()
    {
        // スキップ（ Sキー | Yボタン | □ボタン ）
        if (WasPressedKey(Key.S) || WasPressedButton(GamepadButton.West))
        {
            if (!m_PopupUI.showing) 
                ShowSkip(PopupUI.PopupType.Skip); 
            else 
                HideSkip();
        }

        // キャンセル（ Xキー | Bボタン | ×ボタン ）
        if (m_PopupUI.showing && (WasPressedKey(Key.X) || WasPressedButton(GamepadButton.South)))
            HideSkip(); 
    }

    public void BuildButton(int handIndex, bool firstBuild = false)
    {
        // ボタン単体の横幅
        float width = buttonGroupWidth / GameSetting.hand_blocks;

        // ボタンの生成
        HandButton button = null;
        if (!firstBuild) button = buttonList[handIndex];
        else
        {
            button = new HandButton();
            button.button = Instantiate(m_ButtonPrefab.gameObject).GetComponent<Button>();
            button.rectTransform = button.button.GetComponent<RectTransform>();
        }

        button.button.transform.SetParent(transform);

        // ボタンの横幅と位置を演算
        float x = handIndex * width;

        // ボタンの横幅を設定
        button.rectTransform.offsetMin = button.rectTransform.offsetMin.Setter(x: x);
        button.rectTransform.offsetMax = button.rectTransform.offsetMax.Setter(x: x + width);

        // ボタンの名前を設定
        button.button.transform.GetChild(0).GetComponent<Text>().text = $"Blocks";

        // BlocksUI
        ButtonBlocksUI blocksUI = button.button.GetComponentInChildren<ButtonBlocksUI>();

        blocksUI.Setup(hand.GetBlocksAt(handIndex), m_BlockSprite, false, false);
        blocksUI.SetupShadow(m_ShadowDistance);

        if (firstBuild) {
            buttonList.Add(button);
            buttonTitleList.Add(button.button.transform.GetChild(0).GetComponent<Text>());
        }

        // ボタンのタイトルを設定
        buttonTitleList[handIndex].text = $"Blocks";

        buttonList[handIndex].button.name = $"Button[{handIndex}]";
        buttonList[handIndex].basisPosition = buttonList[handIndex].rectTransform.position;

        // 開始時の移動表示DOTween
        buttonList[handIndex].rectTransform.DOMove(buttonList[handIndex].basisPosition.Setter(y: -40f), changeTime)
            .SetEase(Ease.InOutBack).SetDelay(firstBuild ? 0.06f * handIndex : 0f).OnComplete(ValidateSetable);

        // ボタンを押したときの処理
        buttonList[handIndex].button.onClick.RemoveAllListeners();
        buttonList[handIndex].button.onClick.AddListener(() => PlayAt(handIndex));

        // ゲーム開始時の生成後は左端の手札ボタンを選択
        if (firstBuild) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(buttonList[0].button.gameObject);
    }

    /// <summary>手札から盤面にブロックスを生成するUI処理</summary>
    public void PlayAt(int handIndex)
    {
        // ポップアップを表示している・いずれかのボタンが移動処理中
        foreach (var button in buttonList) if (m_PopupUI.showing || DOTween.IsTweening(button.button.gameObject)) return;

        Vector2Int pos = GameManager.boardSize / 2;

        var blocks = hand.GetBlocksAt(handIndex);

        // ブロックの生成
        m_BlockManager.CreateBlock(this, handIndex, blocks.shape, pos, blocks.density);

        // すべてのボタンを操作不能にする
        for(int i = 0; i < GameSetting.hand_blocks; ++i) Uninteractate(i);
    }

    public void ChangeAt(int handIndex)
    {
        for (int i = 0; i < buttonList.Count; ++i)
            buttonList[i].button.interactable = false;

        //UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(buttonList[3].gameObject);

        // どこにも設置できなかった場合、すべて0の盤面情報を作りサーバーに送信
        Board setData = new Board(0, 0);
        BoardData boardData = new BoardData(setData, GameSetting.instance.selfIndex + 1);

        NetworkClient.Send(boardData);

        // ① handIndexと一致するUIを下方向に移動
        // ② 手札UIのドロー処理
        Vector3 spos = buttonList[handIndex].basisPosition;
        Vector3 fpos = buttonList[handIndex].basisPosition.Setter(y: -40f);

        buttonList[handIndex].rectTransform.DOMove(spos, changeTime).SetEase(Ease.InCubic)
            .OnComplete(() => {

                // 手札を再生成してポップアップを非表示する
                Rebuild(handIndex);
                m_PopupUI.HidePopup();

                // 操作可能にする
                Interactate(handIndex);
            });
    }

    /// <summary>設置スキップ</summary>
    public void ShowSkip(PopupUI.PopupType type)
    {
        // 設置不可の時のポップアップ
        m_PopupUI.ShowPopup(type);

        // 各手札ボタンの処理内容を「手札交換処理」に変更
        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            buttonList[i].button.onClick.RemoveAllListeners();

            // COLUMN: AddListenerの引数インデックスはループ変数への対応力がないため、
            //         一度ローカル変数を介して引数を提示する必要がある
            int index = i;
            buttonList[i].button.onClick.AddListener(() => ChangeAt(index));
        }
    }

    public void HideSkip()
    {
        // 設置不可の時のポップアップ
        m_PopupUI.HidePopup();

        // 各手札ボタンの処理内容を元の「盤面上にブロックを生成」に戻す
        for (int i = 0; i < GameSetting.hand_blocks; ++i) 
        {
            buttonList[i].button.onClick.RemoveAllListeners();

            // QUIZ: AddListenerの関数内引数に対して i ではなく index を用いている理由を述べよ
            int index = i;
            buttonList[i].button.onClick.AddListener(() => PlayAt(index));
        }

        ValidateSetable();
    }

    /// <summary>山札から手札にドローするUI処理</summary>
    /// <param name="num">ドロー先の手札UIの番号</param>
    public void DrawAt(int handIndex)
    {
        // ① handIndexと一致するUIを下方向に移動
        // ② 手札UIのドロー処理
        Vector3 spos = buttonList[handIndex].basisPosition;
        Vector3 fpos = buttonList[handIndex].basisPosition.Setter(y: -40f);

        buttonList[handIndex].rectTransform.DOMove(spos, changeTime).SetEase(Ease.InCubic)
            .OnComplete(() => Rebuild(handIndex));
    }

    /// <summary>設置時のコールバック処理</summary>
    /// <param name="handIndex">設置したブロックスの手札番号</param>
    public void OnSet(int handIndex)
    {
        hand.hand[handIndex] = null;
    }

    /// <summary>手札を再生成</summary>
    /// <param name="handIndex">再生成する手札番号</param>
    public void Rebuild(int handIndex)
    {
        // 山札にブロックスが残っている場合はドロー処理と再生成処理を行う
        if (hand.deck.deck.Count != 0)
        {
            Blocks blocks = hand.deck.Draw();

            hand.hand[handIndex] = blocks;

            // ボタンのブロックス画像を更新
            buttonList[handIndex].button.GetComponentInChildren<ButtonBlocksUI>().ResetBlocks(blocks);

            // 再生成して表示する
            BuildButton(handIndex);
        }
        else
            hand.hand[handIndex] = null;

        // のこりターンUIの更新
        m_TurnUI.UpdateTurnUI(hand);
    }

    /// <summary>選択可能にする</summary>
    /// <param name="selectHandIndex">選択する手札番号（-1: 選択を変更しない）</param>
    public void Interactate(int selectHandIndex = -1)
    {
        for (int i = 0; i < GameSetting.hand_blocks; ++i)
        {
            // これ以上ドローできない場合はnull
            if (hand.deck.deck.Count == 0 && hand.hand[i] == null) continue;

            buttonList[i].button.interactable = true;

            var image = buttonList[i].button.GetComponentInChildren<ButtonBlocksUI>().image;
            image.DOFade(1f, buttonList[i].button.colors.fadeDuration).SetEase(Ease.Unset);

            buttonTitleList[i].DOFade(1f, buttonList[i].button.colors.fadeDuration).SetEase(Ease.Unset);
        }

        if (selectHandIndex != -1)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(buttonList[selectHandIndex].button.gameObject);
    }

    /// <summary>選択不可能にする</summary>
    /// <param name="handIndex">選択不可にする手札番号</param>
    public void Uninteractate(int handIndex)
    {
        buttonList[handIndex].button.interactable = false;

        var image = buttonList[handIndex].button.GetComponentInChildren<ButtonBlocksUI>().image;
        image.DOFade(0.25f, buttonList[handIndex].button.colors.fadeDuration).SetEase(Ease.Unset);

        buttonTitleList[handIndex] .DOFade(0.25f, buttonList[handIndex].button.colors.fadeDuration).SetEase(Ease.Unset);
    }

    /// <summary>設置できるか検証し、設置不可能なブロックスのボタンを操作不可能にする</summary>
    /// <remarks>全て設置不可能な場合、ボタンを操作可能にしてスキップ処理に移行する</remarks>
    void ValidateSetable()
    {
        List<int> unsetables = new List<int>();

        for (int i = 0; i < GameSetting.hand_blocks; ++i)
            if (hand.hand[i] == null || !hand.hand[i].ValidateSetable(GameManager.board, GameSetting.instance.selfIndex))
                unsetables.Add(i);

        //Debug.Log($"unsetables count: {unsetables.Count}");

        // 全て設置可能なら何もせず終了
        if (unsetables.Count == 0) return;

        // 全て操作不可能な場合はスキップ処理
        if (unsetables.Count == GameSetting.hand_blocks) ShowSkip(PopupUI.PopupType.Unsetable);

        // 一部が操作できない場合は設置不可手札を操作不可能にする
        else foreach (int unsetable in unsetables) Uninteractate(unsetable);

        // 最後に操作可能な手札UIにカーソルを合わせる
        for (int i = 0; i < GameSetting.hand_blocks; ++i) {
            if (buttonList[i].button.interactable) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(buttonList[i].button.gameObject);
                break;
            }
        }
    }

    /// <summary>キー検知</summary>
    bool WasPressedKey(params Key[] keys)
    {
        if (Keyboard.current == null) return false;

        foreach (Key key in keys)
            if (Keyboard.current[key].wasPressedThisFrame)
                return true;

        return false;
    }

    /// <summary>ボタン検知</summary>
    bool WasPressedButton(params GamepadButton[] buttons)
    {
        if (Gamepad.current == null) return false;

        foreach (GamepadButton button in buttons)
            if (Gamepad.current[button].wasPressedThisFrame)
                return true;

        return false;
    }
}
