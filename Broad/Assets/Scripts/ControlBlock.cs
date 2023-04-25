using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    const float m_move_interval = 0.2f;
    const float m_move_time = 0.05f;

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

    // 操作可能か
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // ブロック情報
    public Blocks m_Blocks;

    // プレイヤー番号
    public int playerIndex;

    ///<summary>設置後のオブジェクト</summary>
    public GameObject m_AfterSetParent;

    // 移動方向
    Vector2Int m_MoveDirection;



    // Start is called before the first frame update
    void Start()
    {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

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
            case BlocksState.Wait:      WaitState(); break;
            case BlocksState.Move:      MoveState(); break;
            case BlocksState.Vibrate:   VibrateState(); break;
            case BlocksState.Set:       SetState(); break;
        }
    }

    void WaitState()
    {
        // Zキー か Enterキー を押されたら ブロックスを設置
        if (woskni.KeyBoard.GetOrKeyDown(KeyCode.Z, KeyCode.Return))
        {
            // 設置判定をしてtrueならBlocksStateをSetに変更
            if ( m_Blocks.IsSetable( m_GameManager.m_Board, m_Blocks, GetBoardPosition(transform.position), playerIndex))
                m_BlocksState = BlocksState.Set;

            // 設置ができない場合は振動
            else
            {
                m_BlocksState = BlocksState.Vibrate;
                m_MoveDirection = Vector2Int.one;
            }
        }

        // 上下左右の移動処理（Ease.OutCubic・相対移動)
        if (KeyRepeat(KeyCode.UpArrow   , KeyCode.W)) ChangeState(Vector2Int.up);
        if (KeyRepeat(KeyCode.LeftArrow , KeyCode.A)) ChangeState(Vector2Int.left);
        if (KeyRepeat(KeyCode.DownArrow , KeyCode.S)) ChangeState(Vector2Int.down);
        if (KeyRepeat(KeyCode.RightArrow, KeyCode.D)) ChangeState(Vector2Int.right);


        // 状態を変更
        void ChangeState(Vector2Int move)
        {
            m_MoveDirection = move;

            int num = -1;
            if (move == Vector2Int.up) num = 0;
            if (move == Vector2Int.left) num = 1;
            if (move == Vector2Int.down) num = 2;
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
                // 設置状態にする
                m_IsOperatable = true;

                // 設置処理
                Set();
            });
    }

    bool KeyRepeat(params KeyCode[] key)
    {
        // キーを押していなければfalseを返す
        if (!woskni.KeyBoard.GetOrKey(key)) return false;

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

    /// <summary>移動処理</summary>
    /// <param name="move">移動量（マス数）</param>
    void Move(Vector2Int move = new Vector2Int(), float moveTime = 0.1f)
    {
        // 移動中は何もせず終了
        if (!m_IsOperatable) return;

        // 移動
        bool[] isCollision = IsCollisionMoveLimit();

        // Vector3に変換
        Vector3 moveVector3 = new Vector3(m_GameManager.m_SquareSize.x * move.x, 0f, m_GameManager.m_SquareSize.y * move.y);

        // 操作不能にする
        m_IsOperatable = false;

        transform.DOLocalMove(moveVector3, moveTime).SetEase(Ease.OutCubic).SetRelative()
            .OnComplete(() =>
            {
                // 移動可能にし、移動方向をゼロにする
                m_IsOperatable = true;
                m_MoveDirection = Vector2Int.zero;

                // StateをWaitに戻す
                m_BlocksState = BlocksState.Wait;
            });
    }

    /// <summary>移動処理</summary>
    /// <param name="x">ｘ移動量（マス数）</param>
    /// <param name="y">ｙ移動量（マス数）></param>
    private void Move(int x = 0, int y = 0, float moveTime = 0.1f) => Move(new Vector2Int(x, y), moveTime);

    Vector2Int GetBoardPosition(Vector3 position)
    {
        var pos = new Vector2Int((int)transform.position.x, (int)-transform.position.z);
        return pos - new Vector2Int(m_Blocks.GetWidth() / 2, m_Blocks.GetHeight() / 2);
    }

    void OutputDebugText()
    {
        string debugText = "";

        for (int y = 0; y < m_GameManager.m_BoardSize.y; ++y) {
            for (int x = 0; x < m_GameManager.m_BoardSize.x; ++x)
                debugText += m_GameManager.m_Board[x, y].state < 0 ? "-1" : " " + m_GameManager.m_Board[x, y].state;

            debugText += "\n";
        }

        Debug.Log(debugText);

        TextOperate.WriteFile("debugText.txt", debugText);
    }

    /// <summary> 壁検知 </summary>
    /// <returns>壁の有無　[0]: 上, [1]: 左, [2]: 下, [3]: 右</returns>
    private bool[] IsCollisionMoveLimit()
    {
        // 当たり判定用配列
        bool[] isCollision = new bool[4];

        // ブロックサイズ
        Vector2 blocksSize = new Vector2(m_Blocks.GetWidth(), m_Blocks.GetHeight());

        Vector2 limitLeftTop = Vector2.zero - (blocksSize / 2f);                    // 左上の移動限界
        Vector2 limitRightBottom = m_GameManager.m_BoardSize - (blocksSize / 2f); // 右下の移動限界

        isCollision[0] = transform.position.z >= limitLeftTop.y;        // 上
        isCollision[1] = transform.position.x <= -limitLeftTop.x;       // 左
        isCollision[2] = transform.position.z <= -limitRightBottom.y;   // 下
        isCollision[3] = transform.position.x >= limitRightBottom.x;    // 右

        return isCollision;
    }

    /// <summary> 設置処理 </summary>
    private void Set()
    {
        var pos = GetBoardPosition(transform.position);

        for (int y = 0; y < m_Blocks.GetHeight(); ++y)
            for (int x = 0; x < m_Blocks.GetWidth(); ++x)
                if (m_Blocks.shape[x, y])
                    m_GameManager.m_Board[pos.x + x, pos.y + y].state = playerIndex;

        // 親を変える
        GameObject[] children = gameObject.GetChildren();
        for (int i = 0; i < children.Length; ++i)
            children[i].transform.SetParent(m_AfterSetParent.transform);
    }
}
