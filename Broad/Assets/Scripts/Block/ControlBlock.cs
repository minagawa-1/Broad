using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Mirror;
using Cysharp.Threading.Tasks;
using Microsoft.VisualBasic;

public class ControlBlock : MonoBehaviour
{
    const float m_operatable_interval   = 0.2f;
    const float m_move_time             = 0.05f;
    const float m_set_time              = 0.25f;

// public:

    // ブロック情報
    public Blocks blocks;

    // 手札UI
    public HandUI handUI;
    
    // 手札番号
    public int handIndex;

    ///<summary>設置後のオブジェクト</summary>
    public Transform afterSetParent;

    ///<summary>設置後のマテリアル</summary>
    public Material afterSetMaterial;

    // private:

    /// <summary> ブロックの状態</summary>
    enum BlocksState
    {
        /// <summary> 待機 </summary>
        Wait,
        /// <summary> 移動 </summary>
        Move,
        /// <summary> 移動後待機 </summary>
        Delay,
        /// <summary> 振動 </summary>
        Vibrate,
        /// <summary> 回転 </summary>
        Rotate,
        /// <summary>設置位置の決定</summary>
        Decision,
        /// <summary>他のプレイヤーの設置を待機</summary>
        WaitOther,
        /// <summary> 設置 </summary>
        Set,
        /// <summary>オブジェクトの破棄</summary>
        Discard
    }

    // ブロックの状態
    BlocksState m_BlocksState = BlocksState.Wait;

    // コンポーネント
    CustomNetworkManager m_NetworkManager = null;
    GameManager          m_GameManager    = null;
    BlockManager         m_BlockManager   = null;
    DonutChart           m_DonutChart     = null;

    // 操作可能か
    bool m_IsOperatable = true;

    // 移動方向
    Vector2Int m_MoveDirection;
    float m_RotateDirection;

    woskni.Timer m_DelayTimer = new woskni.Timer();

    bool m_IsSet;   // 設置したかどうか

    int m_SetCompletedCount;

    GameObject[] m_Duplicates;

    // ターン開始時のボード情報
    int[,] m_InitialBoardData;

    // Start is called before the first frame update
    void Start()
    {
        // クライアント側がデータを受信したときに対応した関数を実行するように登録
        NetworkClient.ReplaceHandler<ReadyData>(ClientReceivedReadyData);
        NetworkClient.ReplaceHandler<DuplicateData>(ClientReceivedDuplicateData);

        Setup();

        m_InitialBoardData = GameManager.board.GetBoard();
    }

    // Update is called once per frame
    void Update()
    {
        // BlockStateに応じた関数を呼ぶ
        switch (m_BlocksState)
        {
            case BlocksState.Wait:      WaitState();                    break;
            case BlocksState.Move:      MoveState();                    break;
            case BlocksState.Delay:     DelayState();                   break;
            case BlocksState.Vibrate:   VibrateState();                 break;
            case BlocksState.Decision:  DecisionState();                break;
            case BlocksState.WaitOther: WaitOtherState().Preserve();    break;
            case BlocksState.Set:       SetState().Preserve();          break;
            case BlocksState.Rotate:    RotateState();                  break;
            case BlocksState.Discard:   DiscardState();                 break;
        }

        //Debug.Log("current state : " + m_BlocksState);
    }

    /// <summary>準備</summary>
    void Setup()
    {
        // コンポーネント取得
        m_NetworkManager    = GameObject.Find(nameof(NetworkManager)        ).GetComponent<CustomNetworkManager>();
        m_GameManager       = GameObject.Find(nameof(GameManager)           ).GetComponent<GameManager>();
        m_BlockManager      = GameObject.Find(nameof(BlockManager)          ).GetComponent<BlockManager>();
        m_DonutChart        = GameObject.Find($"Canvas/{nameof(DonutChart)}").GetComponent<DonutChart>();

        m_BlocksState = BlocksState.Wait;

        m_DelayTimer.Setup(Mathf.Abs(m_operatable_interval - m_move_time));

        m_MoveDirection = Vector2Int.zero;

        m_IsSet = false;
        m_SetCompletedCount = 0;
    }

    void WaitState()
    {
        // ブロックスの半透明・不透明切り替え（ Cキー | Xボタン | △ボタン ）
        if (Keyboard.current.cKey.wasPressedThisFrame || Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            var children = transform.GetChildren();
            GetComponent<ChangeTransparency>().Change(ref children);
        }

        // キャンセル（ Xキー | Escキー | Bボタン | ×ボタン ）
        if (Keyboard.current.xKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame
            || Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            m_BlocksState = BlocksState.Discard;
        }

        // ブロックスを設置（ Zキー | Enterキー | Aボタン | ○ボタン ）
        if (Keyboard.current.zKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame
            || Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            // 設置判定をしてtrueならBlocksStateをSetに変更
            if (blocks.IsSetable(GameManager.board, GameSetting.instance.selfIndex))
                m_BlocksState = BlocksState.Decision;

            // 設置ができない場合は振動
            else
            {
                m_MoveDirection = Vector2Int.one;
                m_BlocksState = BlocksState.Vibrate;
            }
        }

        // 回転処理（Ease.OutCubic　相対回転）
        int rotate = GetInputRotation();
        if (rotate < 0) ChangeRotateState(-90f);
        if (rotate > 0) ChangeRotateState( 90f);

        // 移動処理（Ease.OutCubic　相対移動)
        Vector2Int move = GetInputDirection();

        if(move != Vector2Int.zero) ChangeMoveState(move);

        // 終了 (ここから下は関数内関数)
        return;

        // 状態を変更 (BlocksState.Rotate)
        void ChangeRotateState(float rotate)
        {
            m_RotateDirection = rotate;

            m_BlocksState = BlocksState.Rotate;
        }

        // 状態を変更 (BlocksState.Move)
        void ChangeMoveState(Vector2Int move)
        {
            m_MoveDirection = move;

            int num = -1;
            if (move == Vector2Int.up   ) num = 0;
            if (move == Vector2Int.left ) num = 1;
            if (move == Vector2Int.down ) num = 2;
            if (move == Vector2Int.right) num = 3;

            // 入力された方向の壁検知をして状態を変える
            m_BlocksState = IsCollisionMoveLimit()[num] ? BlocksState.Vibrate : BlocksState.Move;
        }

        // キーボード・ゲームパッドの操作から移動方向を返す
        Vector2Int GetInputDirection()
        {
            if(Keyboard.current != null)
            { 
                // W・A・S・Dキー
                if (Keyboard.current.wKey.isPressed) return Vector2Int.up;
                if (Keyboard.current.aKey.isPressed) return Vector2Int.left;
                if (Keyboard.current.sKey.isPressed) return Vector2Int.down;
                if (Keyboard.current.dKey.isPressed) return Vector2Int.right;

                // 矢印キー
                if (Keyboard.current.upArrowKey   .isPressed) return Vector2Int.up;
                if (Keyboard.current.leftArrowKey .isPressed) return Vector2Int.left;
                if (Keyboard.current.downArrowKey .isPressed) return Vector2Int.down;
                if (Keyboard.current.rightArrowKey.isPressed) return Vector2Int.right;
            }

            if(Gamepad.current != null)
            {
                // 左スティック
                if (Gamepad.current.leftStick.ReadValue().y >  0.5f) return Vector2Int.up;
                if (Gamepad.current.leftStick.ReadValue().x < -0.5f) return Vector2Int.left;
                if (Gamepad.current.leftStick.ReadValue().y < -0.5f) return Vector2Int.down;
                if (Gamepad.current.leftStick.ReadValue().x >  0.5f) return Vector2Int.right;

                // 十字キー
                if (Gamepad.current.dpad.ReadValue().y >  0.5f) return Vector2Int.up;
                if (Gamepad.current.dpad.ReadValue().x < -0.5f) return Vector2Int.left;
                if (Gamepad.current.dpad.ReadValue().y < -0.5f) return Vector2Int.down;
                if (Gamepad.current.dpad.ReadValue().x >  0.5f) return Vector2Int.right;
            }

            return Vector2Int.zero;
        }

        // キーボード・ゲームパッドの操作から回転方向を返す
        // マイナス値: 反時計回り
        // 0: 回転しない
        // プラス値: 時計回り
        int GetInputRotation()
        {
            if (Keyboard.current != null)
            {
                // Q・Eキー
                if (Keyboard.current.qKey.wasPressedThisFrame) return -1;
                if (Keyboard.current.eKey.wasPressedThisFrame) return  1;
            }

            if (Gamepad.current != null)
            {
                // ショルダーボタン (L2・R2 or ZL・ZR）
                if (Gamepad.current.leftShoulder .wasPressedThisFrame) return -1;
                if (Gamepad.current.rightShoulder.wasPressedThisFrame) return  1;

                // トリガーボタン (L・R or LT・RT）
                if (Gamepad.current.leftTrigger.wasPressedThisFrame) return -1;
                if (Gamepad.current.rightTrigger.wasPressedThisFrame) return 1;
            }

            return 0;
        }
    }

    /// <summary> 移動状態 </summary>
    void MoveState()
    {
        if (DOTween.IsTweening(transform)) return;

        transform.DOLocalMove(GetVector3Board(m_MoveDirection), m_move_time).SetEase(Ease.OutCubic).SetRelative()
            .OnComplete(() =>
            {
                blocks.position = GetBoardPosition(transform.position).Offset((int)-blocks.center.x, (int)-blocks.center.y);

                // 移動方向をゼロにする
                m_MoveDirection = Vector2Int.zero;

                m_DelayTimer.Reset();

                m_BlocksState = BlocksState.Delay;
            });
    }

    void DelayState()
    {
        m_DelayTimer.Update();

        if(m_DelayTimer.IsFinished()) m_BlocksState = BlocksState.Wait;
    }

    /// <summary> 振動状態 </summary>
    void VibrateState()
    {
        if (DOTween.IsTweening(transform)) return;

        Vector3 vibrateRate = new Vector3(m_MoveDirection.x, 0.2f, m_MoveDirection.y);

        // 振動処理
        transform.OnVibrate(0.2f, vibrateRate, m_operatable_interval)
            .OnComplete(() =>
            {
                // 移動可能にし、移動方向をゼロにする
                m_MoveDirection = Vector2Int.zero;

                // StateをWaitに戻してreturn
                m_BlocksState = BlocksState.Wait;
            }
            );
    }

    /// <summary>回転状態</summary>
    void RotateState()
    {
        if (DOTween.IsTweening(transform)) return;

        var rot = Vector3.zero.Offset(y: m_RotateDirection);

        transform.DOLocalRotate(rot, m_operatable_interval, RotateMode.FastBeyond360).SetEase(Ease.OutCubic)
            .SetRelative().OnComplete(() =>
            {
                // 配列の回転処理
                if (m_RotateDirection < 0f) blocks.RotateLeft();
                if (m_RotateDirection > 0f) blocks.RotateRight();

                blocks.position = GetBoardPosition(transform.position).Offset((int)-blocks.center.x, (int)-blocks.center.y);

#if UNITY_EDITOR
                OutputDebugText(false, "blockShape[,].txt");
#endif

                // 移動方向をゼロにする
                m_RotateDirection = 0f;

                // StateをWaitに戻す
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary>設置位置の決定状態</summary>
    void DecisionState()
    {
        // 操作不能なら終了
        if (!m_IsOperatable) return;

        // 操作不能にする
        m_IsOperatable = false;

        // 現在のターンでの設置データ
        bool[,] setData = new bool[GameManager.boardSize.x, GameManager.boardSize.y];

        // ブロックを設置しようとしている盤面座標にtrueを入れる
        for (int y = 0; y < blocks.height; ++y)
            for (int x = 0; x < blocks.width; ++x)
                if (blocks.shape[x, y]) setData[blocks.position.x + x, blocks.position.y + y] = true;

        // 設置データから現在のターンの盤面メッセージデータを作って送信
        Board board = new Board(setData);

        BoardData boardData = new BoardData(board, GameSetting.instance.selfIndex + 1);
        NetworkClient.Send(boardData);

        // Stateを切り替え
        m_BlocksState = BlocksState.WaitOther;
    }

    /// <summary>他プレイヤー待機状態</summary>
    async UniTask WaitOtherState()
    {

///////////////// ▼ホストの処理▼ //////////////////////////////////////////////////////////////////////////////////////////////////

        // 自分がホスト
        if (NetworkClient.activeHost)
        {
            // プレイヤー全員の盤面情報が揃うまで待機
            await UniTask.WaitUntil(() => m_GameManager.orderBoardDataList.Count == NetworkServer.connections.Count,
                cancellationToken: this.GetCancellationTokenOnDestroy());

            // 重複除去処理
            {
                // RidDuplicateを呼ぶ準備(タプルデータに直す)
                var setDatas = new BoardData[NetworkServer.connections.Count];

                for (int i = 0; i < setDatas.Length; ++i)
                    setDatas[i] = new BoardData(m_GameManager.orderBoardDataList[i].board, m_GameManager.orderBoardDataList[i].player);

                // 重複除去
                if (NetworkClient.activeHost) m_Duplicates = m_GameManager.RidDuplicate(setDatas).ToArray();

                // 重複しているブロックの情報を全クライアントに送信
                DuplicateData duplicateData = new DuplicateData(m_Duplicates.ToArray());
                NetworkServer.SendToAll(duplicateData);
            }

            // 次のターン以降も使いまわせるようにリストをクリア
            m_GameManager.orderBoardDataList.Clear();
        }

///////////////// ▼共通の処理▼ ////////////////////////////////////////////////////////////////////////////////////////////////////

        // 新しい盤面情報を受信できるまで待機
        await UniTask.WaitUntil(() => m_GameManager.isReceived, cancellationToken: this.GetCancellationTokenOnDestroy());

        // falseにすることで次のターン以降も使いまわせるようにする
        m_GameManager.isReceived = false;

        // 他プレイヤーのブロックを生成
        m_GameManager.ComplementBoard(GameManager.board.GetBoard(), m_InitialBoardData);

        // 準備完了したことをホストに伝える
        ReadyData readyData = new ReadyData(true);
        NetworkClient.Send(readyData);

///////////////// ▼ホストの処理▼ //////////////////////////////////////////////////////////////////////////////////////////////////

        // 自身がホスト
        if (NetworkClient.activeHost)
        {
            // クライアント全員の準備が完了できるまで待機
            await UniTask.WaitUntil(() => m_GameManager.readyCount == NetworkServer.connections.Count);

            // 次のターン以降も使いまわせるようにリセット
            m_GameManager.readyCount -= NetworkServer.connections.Count;

            ReadyData ready = new ReadyData(true);
            NetworkServer.SendToAll(ready);
        }

////////////// ▼共通の処理▼ ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Stateを切り替える
        m_BlocksState = BlocksState.Set;
    }

    /// <summary> 設置状態 </summary>
    async UniTask SetState()
    {
        // DOTween処理の終了検知
        if (m_SetCompletedCount >= m_GameManager.createdBlockList.Count)
        {
            // 前のターンに設置した重複ブロックを削除
            DestroyOldBlocks();

            // 名前の後ろの"new"を削除する
            for (int i = 0; i < m_GameManager.createdBlockList.Count; ++i)
                m_GameManager.createdBlockList[i].name = m_GameManager.createdBlockList[i].name.Replace("new", "");

            // ドーナツチャートを更新
            m_DonutChart.UpdateDonut();

            // ヒエラルキ上のブロックスをソート
            m_BlockManager.SortBlocks();

            // 現在のターンで生成したブロックの情報を次のターンに持ち越さないようにリストをクリア
            m_GameManager.createdBlockList.Clear();

            // 手札UIに選択を合わせる
            handUI.OnSet(handIndex);

            handUI.DrawAt(handIndex);

            m_BlocksState = BlocksState.Discard;
        }

        if (!m_IsSet) return;

        // ホストから許可が降りるまで待機
        await UniTask.WaitUntil(() => m_IsSet, cancellationToken: this.GetCancellationTokenOnDestroy());

        m_IsSet = false;

        // 非アクティブ化して子オブジェクトの見た目を消す
        var children = transform.GetChildren();
        for (int i = 0; i < children.Length; ++i) children[i].gameObject.SetActive(false);

#if UNITY_EDITOR
        OutputDebugText(true, "board[,].txt");
#endif

        // イージング (終了後、盤面に反映させて終了)
        for (int i = 0; i < m_GameManager.createdBlockList.Count; ++i)
        {
            m_GameManager.createdBlockList[i].transform.DOMoveY(0f, m_set_time).SetEase(Ease.InQuart)
                .OnComplete(() => m_SetCompletedCount++);
        }

        // 重複しているブロックを上に飛ばす
        for (int i = 0; i < m_Duplicates.Length; ++i)
        {
            m_Duplicates[i].transform.DOMoveY(8f, 1f).SetDelay(m_set_time).SetEase(Ease.OutCirc)
                        .OnComplete(() => Destroy(m_Duplicates[i].gameObject)).SetLink(m_Duplicates[i].gameObject);
        }
    }

    /// <summary>ブロックの破棄</summary>
    void DiscardState()
    {
        // 操作可能にする
        handUI.Interactate(handIndex);

        // 操作用のブロックを破棄
        var children = transform.GetChildren();

        for (int i = 0; i < children.Length; ++i) {
            DOTween.Kill(children[i].gameObject);
            Destroy(children[i].gameObject);
        }

        DOTween.Kill(gameObject);
        Destroy(gameObject);
    }

    /// <summary>Vector3を盤面座標に変換</summary>
    /// <param name="position">３次元座標</param>
    /// <returns>盤面の２次元座標</returns>
    Vector2Int GetBoardPosition(Vector3 pos)
    {
        return new Vector2Int((int)pos.x, (int)-pos.z);
    }

    /// <summary>盤面座標をVector3に変換</summary>
    /// <param name="pos">盤面の２次元座標</param>
    /// <returns>３次元座標</returns>
    Vector3 GetVector3Board(Vector2Int pos)
    {
        return new Vector3(pos.x, 0f, pos.y);
    }

    void OutputDebugText(bool boardDebug, string filePath = "debugText.txt")
    {
        string debugText = "";

        if (boardDebug)
        {
            for (int y = 0; y < GameManager.boardSize.y; ++y)
            {
                for (int x = 0; x < GameManager.boardSize.x; ++x)
                {
                    int n = GameManager.board.GetBoardData(x, y);

                    switch (n)
                    {
                        case -1: debugText += "　"; break;
                        case 0: debugText  += "・"; break;
                        default: debugText += n.ToString().ToFullWidth(); break;
                    }
                }

                debugText += "\n";
            }
        }
        else
        {
            bool isEven = blocks.width % 2 == 0 && blocks.height % 2 == 0;

            for (int y = 0; y < blocks.height; ++y)
            {
                for (int x = 0; x < blocks.width; ++x)
                {
                    // center表示
                    if (!isEven && new Vector2(x, y) == blocks.center)
                    {
                        debugText += blocks.shape[x, y] ? "●" : "○";
                        continue;
                    }

                    debugText += blocks.shape[x, y] ? "■" : "・";
                }

                debugText += "\n";
            }
        }

        TextOperate.WriteFile(filePath, debugText);
    }

    /// <summary> 壁検知 </summary>
    /// <returns>壁の有無　[0]: 上, [1]: 左, [2]: 下, [3]: 右</returns>
    private bool[] IsCollisionMoveLimit()
    {
        // 当たり判定用配列
        bool[] isCollision = new bool[4];

        // 移動限界
        Vector2 limitLeftTop     = Vector2.zero          - blocks.center;   // 左上
        Vector2 limitRightBottom = GameManager.boardSize - blocks.center;   // 右下

        if (blocks.width  % 2 == 1) limitRightBottom.x -= 1;
        if (blocks.height % 2 == 1) limitRightBottom.y -= 1;

        if (blocks.width % 2 == 0 && blocks.height % 2 == 0) limitRightBottom -= Vector2.one;

        isCollision[0] = transform.position.z >=  limitLeftTop.y;       // 上
        isCollision[1] = transform.position.x <= -limitLeftTop.x;       // 左
        isCollision[2] = transform.position.z <= -limitRightBottom.y;   // 下
        isCollision[3] = transform.position.x >=  limitRightBottom.x;   // 右

        return isCollision;
    }

    /// <summary>前のターンに設置した重複ブロックを削除</summary>
    void DestroyOldBlocks()
    {
        var newBlocks = m_GameManager.createdBlockList.ToArray();

        for (int i = 0; i < newBlocks.Length; ++i)
        {
            var pos = GetBoardPosition(newBlocks[i].transform.position);

            // 設置するブロックの座標に、前のターンの時点でブロックが存在する
            if (m_InitialBoardData[pos.x, pos.y] > 0)
            {
                var block = m_BlockManager.blockParent.transform.Find($"Block[{pos.x}, {pos.y}]");

                // 破棄するべきオブジェクトを見つけたらデバッグログを出して削除
                if (block != null)
                {
                    Debug.Log($"block: {block.name}");
                    Destroy(block.gameObject);
                }
            }
        }
    }

    /// <summary>キャンセル用トークンソースをキャンセル・廃棄</summary>
    /// <param name="tokenSource">キャンセルトークンソース</param>
    void Cancel(CancellationTokenSource tokenSource)
    {
        // キャンセル
        tokenSource.Cancel();

        // 廃棄
        tokenSource.Dispose();
    }

    /// <summary>ホストからの準備完了の合図を受信</summary>
    /// <param name="readyData">合図用メッセージデータ</param>
    void ClientReceivedReadyData(ReadyData readyData)
    {
        // 念のため、データがtrueなことを確認
        if (readyData.isReady) m_IsSet = true;
    }

    /// <summary>ホストからの重複しているブロックの情報を受信</summary>
    /// <param name="duplicateData">重複情報</param>
    void ClientReceivedDuplicateData(DuplicateData duplicateData)
    {
        m_Duplicates = duplicateData.duplicates;
    }
}
