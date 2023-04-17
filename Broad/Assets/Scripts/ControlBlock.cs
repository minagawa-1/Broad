using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControlBlock : MonoBehaviour
{
    bool[,] sampleBlocks = new bool[,] {
            {false, true},
            {false, true},
            {true , true}
        };

    [Header("コンポーネント")]
    [SerializeField] GameManager m_GameManager = null;

    [Header("移動間隔・移動時間")]
    [SerializeField] float m_MoveInterval = 0.1f;
    [SerializeField] float m_MoveTime = 0.05f;

    // 操作可能か
    bool m_IsOperatable = true;

    woskni.Timer m_KeyRepeatTimer;

    // ブロック情報
    Blocks m_Blocks;

    // Start is called before the first frame update
    void Start()
    {
        m_Blocks = new Blocks(sampleBlocks);

        m_KeyRepeatTimer.Setup(m_MoveInterval);

        // 初期位置の設定(ボードのサイズが奇数か偶数かで0.5fずらすか決める)
        transform.position = transform.position.Difference(x: m_GameManager.m_BoardSize.x % 2 == 0 ? 0.5f : 0f
                                                         , z: m_GameManager.m_BoardSize.y % 2 == 0 ? 0.5f : 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // 上下左右の移動処理（Ease.OutCubic・相対移動)
        if (KeyRepeat(KeyCode.UpArrow    , KeyCode.W)) Move( 0,  1, m_MoveTime);
        if (KeyRepeat(KeyCode.LeftArrow  , KeyCode.A)) Move(-1,  0, m_MoveTime);
        if (KeyRepeat(KeyCode.DownArrow  , KeyCode.S)) Move( 0, -1, m_MoveTime);
        if (KeyRepeat(KeyCode.RightArrow , KeyCode.D)) Move( 1,  0, m_MoveTime);
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
        // 移動
        bool[] isCollision = IsCollisionMoveLimit();

        // 上下左右の壁検知
        if (isCollision[0] && move.y > 0) return;
        if (isCollision[1] && move.x < 0) return;
        if (isCollision[2] && move.y < 0) return;
        if (isCollision[3] && move.x > 0) return;

        // 操作不可なら返す
        if (!m_IsOperatable) return;

        // 操作不能にする
        m_IsOperatable = false;

        // Vector3に変換
        Vector3 moveVector3 = new Vector3(m_GameManager.m_SquareSize.x * move.x, 0f, m_GameManager.m_SquareSize.y * move.y);

        transform.DOLocalMove(moveVector3, moveTime).SetEase(Ease.OutCubic).SetRelative().OnComplete(() => m_IsOperatable = true);
    }

    /// <summary>移動処理</summary>
    /// <param name="x">ｘ移動量（マス数）</param>
    /// <param name="y">ｙ移動量（マス数）></param>
    private void Move(int x = 0, int y = 0, float moveTime = 0.1f) => Move(new Vector2Int(x, y), moveTime);

    /// <summary> 壁検知 </summary>
    /// <returns>壁の有無　[0]: 上, [1]: 左, [2]: 下, [3]: 右</returns>
    private bool[] IsCollisionMoveLimit()
    {
        bool[] isCollision = new bool[4];

        Vector2 blocksSize = new Vector2((float)m_Blocks.GetWidth(), (float)m_Blocks.GetHeight());
        Vector2 maxDinstance = ((Vector2)m_GameManager.m_BoardSize / 2f) - blocksSize / 2f;

        Debug.Log(maxDinstance);

        isCollision[0] = transform.position.x >= maxDinstance.x;    // 上
        isCollision[1] = transform.position.y < -maxDinstance.y;    // 左
        isCollision[2] = transform.position.y < -maxDinstance.x;    // 下
        isCollision[3] = transform.position.y >= maxDinstance.y;    // 右

        return isCollision;
    }
}
