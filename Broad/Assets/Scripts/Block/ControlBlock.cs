using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    const float m_move_interval = 0.2f;
    const float m_move_time = 0.05f;

// public:

    // ブロック情報
    public Blocks blocks;

    // プレイヤー番号
    public int playerIndex;

    ///<summary>設置後のオブジェクト</summary>
    public GameObject afterSetParent;

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
        /// <summary> 振動 </summary>
        Vibrate,
        /// <summary> 設置 </summary>
        Set,
        /// <summary> 回転 </summary>
        Rotate,
    }

    // ブロックの状態
    BlocksState m_BlocksState = BlocksState.Wait;

    // GameManagerコンポーネント
    GameManager m_GameManager = null;
    BlockManager m_BlockManager = null;

    // 操作可能か
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // 移動方向
    Vector2Int m_MoveDirection;
    float      m_RotateDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_GameManager = GameObject.Find(nameof(GameManager)).GetComponent<GameManager>();
        m_BlockManager = GameObject.Find(nameof(BlockManager)).GetComponent<BlockManager>();

        m_KeyRepeatTimer.Setup(m_move_interval);
        m_BlocksState = BlocksState.Wait;

        m_MoveDirection = Vector2Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // BlockStateに応じた関数を呼ぶ
        switch (m_BlocksState)
        {
            case BlocksState.Wait:      WaitState();    break;
            case BlocksState.Move:      MoveState();    break;
            case BlocksState.Vibrate:   VibrateState(); break;
            case BlocksState.Set:       SetState();     break;
            case BlocksState.Rotate:    RotateState();  break;
        }
    }

    void WaitState()
    {
        // Xキー で、ブロックスを半透明にする
        if (Input.GetKeyDown(KeyCode.X))
        {
            var children = transform.GetChildren();
            GetComponent<ChangeTransparency>().Change(ref children);
        }

        // Zキー か Enterキー で、ブロックスを設置
        if (woskni.KeyBoard.GetOrKeyDown(KeyCode.Z, KeyCode.Return))
        {
            // 設置判定をしてtrueならBlocksStateをSetに変更
            if (blocks.IsSetable( m_GameManager.board, playerIndex))
                m_BlocksState = BlocksState.Set;

            // 設置ができない場合は振動
            else
            {
                m_BlocksState = BlocksState.Vibrate;
                m_MoveDirection = Vector2Int.one;
            }
        }

        if (KeyRepeat(KeyCode.Q)) ChangeRotateState(-90f);
        if (KeyRepeat(KeyCode.E)) ChangeRotateState( 90f);

        // 上下左右の移動処理（Ease.OutCubic・相対移動)
        if (KeyRepeat(KeyCode.UpArrow   , KeyCode.W)) ChangeMoveState(Vector2Int.up);
        if (KeyRepeat(KeyCode.LeftArrow , KeyCode.A)) ChangeMoveState(Vector2Int.left);
        if (KeyRepeat(KeyCode.DownArrow , KeyCode.S)) ChangeMoveState(Vector2Int.down);
        if (KeyRepeat(KeyCode.RightArrow, KeyCode.D)) ChangeMoveState(Vector2Int.right);


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
    }

    /// <summary> 移動状態 </summary>
    void MoveState()
    {
        // 移動処理
        Move(m_MoveDirection, m_move_time);
    }

    /// <summary> 振動状態 </summary>
    void VibrateState()
    {
        Vector3 vibrateRate = new Vector3(m_MoveDirection.x, 0.2f, m_MoveDirection.y);

        // 振動処理
        if (m_IsOperatable) transform.Vibrate(0.2f, vibrateRate, m_move_interval);

        // 操作不能にする
        m_IsOperatable = false;

        // 振動終了検知
        if (!transform.IsVibrating())
        {
            // 移動可能にし、移動方向をゼロにする
            m_IsOperatable = true;
            m_MoveDirection = Vector2Int.zero;

            // StateをWaitに戻してreturn
            m_BlocksState = BlocksState.Wait;
            return;
        }
    }

    /// <summary> 設置状態 </summary>
    void SetState()
    {
        // 操作不能なら終了
        if (!m_IsOperatable) return;

        // 操作不能にする
        m_IsOperatable = false;

        // イージング (終了後、盤面に反映させて終了)
        transform.DOMoveY(0f, m_move_interval).SetEase(Ease.InQuart).OnComplete(() =>
            {
                // 設置処理
                Set();

                // 設置状態にする
                m_IsOperatable = true;
            });
    }

    /// <summary>回転状態</summary>
    void RotateState()
    {
        if (!m_IsOperatable) return;

        m_IsOperatable = false;

        var rot = Vector3.zero.Offset(y: m_RotateDirection);

        transform.DOLocalRotate(rot, m_move_interval, RotateMode.FastBeyond360).SetEase(Ease.OutCubic)
            .SetRelative().OnComplete(() =>
            {
                // 配列の回転処理
                if (m_RotateDirection < 0f) blocks.RotateLeft();
                if (m_RotateDirection > 0f) blocks.RotateRight();

                blocks.position = GetBoardPosition(transform.position);

                OutputDebugText(false, "blockShape[,].txt");

                // 移動可能にし、移動方向をゼロにする
                m_IsOperatable = true;
                m_RotateDirection = 0f;

                // StateをWaitに戻す
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary>移動処理</summary>
    /// <param name="move">移動量（マス数）</param>
    void Move(Vector2Int move = new Vector2Int(), float moveTime = 0.1f)
    {
        // 移動中は何もせず終了
        if (!m_IsOperatable) return;

        // 移動
        bool[] isCollision = IsCollisionMoveLimit();

        // 操作不能にする
        m_IsOperatable = false;

        transform.DOLocalMove(GetVector3Board(move), moveTime).SetEase(Ease.OutCubic).SetRelative()
            .OnComplete(() =>
            {
                blocks.position = GetBoardPosition(transform.position);

                // 移動可能にし、移動方向をゼロにする
                m_IsOperatable = true;
                m_MoveDirection = Vector2Int.zero;

                // StateをWaitに戻す
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary> 設置処理 </summary>
    private void Set()
    {
        var pos = GetBoardPosition(transform.position);

        for (int y = 0; y < blocks.GetHeight(); ++y)
            for (int x = 0; x < blocks.GetWidth(); ++x)
                if (blocks.shape[x, y]) m_GameManager.board.SetBoardData(playerIndex, pos.x + x, pos.y + y);

        Transform[] children = transform.GetChildren();

        // ブロックの半透明化を解除
        GetComponent<ChangeTransparency>().Set(ref children, false);

        for (int i = 0; i < children.Length; ++i)
        {
            Vector3 local = children[i].transform.position - transform.position;

            int x = (int)( local.x + blocks.center.x + 0.51f);
            int y = (int)(-local.z + blocks.center.y + 0.51f);

            var boardPos = pos + new Vector2Int(x, y);

            string name = "Block[" + boardPos.x + ", " + boardPos.y + "]";

            // 元々その盤面にいたブロックを破棄する
            GameObject oldBlock = GameObject.Find(name);
            if (oldBlock) Destroy(oldBlock);

            // オブジェクト名を盤面座標の名前に変更する
            children[i].name = name;

            // 親を変える
            children[i].transform.SetParent(afterSetParent.transform);

            // マテリアルを設置後のものに変更
            children[i].GetComponent<Renderer>().material = afterSetMaterial;
        }

        m_BlockManager.SortBlocks();

        OutputDebugText(true, "board[,].txt");

        // コントロール用オブジェクトを破棄
        Destroy(gameObject);
    }

    /// <summary>Vector3を盤面座標に変換</summary>
    /// <param name="position">３次元座標</param>
    /// <returns>盤面の２次元座標</returns>
    Vector2Int GetBoardPosition(Vector3 pos)
    {
        var posBoard = new Vector2Int((int)pos.x, (int)-pos.z);
        int offsetX = (int)(-blocks.center.x * m_GameManager.m_SquareSize.x);
        int offsetY = (int)(-blocks.center.y * m_GameManager.m_SquareSize.y);

        return posBoard.Offset(offsetX, offsetY);
    }

    /// <summary>盤面座標をVector3に変換</summary>
    /// <param name="pos">盤面の２次元座標</param>
    /// <returns>３次元座標</returns>
    Vector3 GetVector3Board(Vector2Int pos)
    {
        return new Vector3(m_GameManager.m_SquareSize.x * pos.x, 0f, m_GameManager.m_SquareSize.y * pos.y);
    }

    /// <summary>キーリピート処理</summary>
    /// <param name="key">判定するキー</param>
    /// <returns>押した瞬間と押している間の一定間隔にtrue</returns>
    bool KeyRepeat(params KeyCode[] key)
    {
        // キーを押していなければタイマーをリセットしてfalseを返す
        if (!woskni.KeyBoard.GetOrKey(key)) {
            m_KeyRepeatTimer.Reset();
            return false;
        }

        // タイマー更新
        m_KeyRepeatTimer.Update();

        // リピート間隔に達したときタイマーをリセットしてtrueを返す。
        if (m_KeyRepeatTimer.IsFinished()) {
            m_KeyRepeatTimer.Reset();
            return true;
        }

        // どちらでもなければ、押した瞬間を検知
        return woskni.KeyBoard.GetOrKeyDown(key);
    }

    void OutputDebugText(bool boardDebug, string filePath = "debugText.txt")
    {
        string debugText = "";

        if (boardDebug)
        {
            for (int y = 0; y < m_GameManager.boardSize.y; ++y)
            {
                for (int x = 0; x < m_GameManager.boardSize.x; ++x)
                {
                    switch (m_GameManager.board.GetBoardData(x, y))
                    {
                        case -1: debugText += "　"; break;
                        case  0: debugText += "□"; break;
                        default: debugText += "■"; break;
                    }
                }

                debugText += "\n";
            }
        }
        else
        {
            bool isEven = blocks.GetWidth() % 2 == 0 && blocks.GetHeight() % 2 == 0;

            for (int y = 0; y < blocks.GetHeight(); ++y)
            {
                for (int x = 0; x < blocks.GetWidth(); ++x)
                {
                    // center表示
                    if (!isEven && new Vector2(x, y) == blocks.center) { 
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
        Vector2 limitLeftTop = Vector2.zero - blocks.center;                  // 左上
        Vector2 limitRightBottom = m_GameManager.boardSize - blocks.center;   // 右下

        if (blocks.GetWidth()  % 2 == 1) limitRightBottom.x -= 1;
        if (blocks.GetHeight() % 2 == 1) limitRightBottom.y -= 1;

        if (blocks.GetWidth() % 2 == 0 && blocks.GetHeight() % 2 == 0) limitRightBottom -= Vector2.one;

        isCollision[0] = transform.position.z >=  limitLeftTop.y;       // 上
        isCollision[1] = transform.position.x <= -limitLeftTop.x;       // 左
        isCollision[2] = transform.position.z <= -limitRightBottom.y;   // 下
        isCollision[3] = transform.position.x >=  limitRightBottom.x;   // 右

        return isCollision;
    }
}
