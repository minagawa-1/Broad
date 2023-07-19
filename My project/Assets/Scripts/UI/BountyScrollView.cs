using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//バウンティスクロールビュークラス
public class BountyScrollView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_BountyIndexText       = null;         //バウンティ番号文字列
    [SerializeField] GameObject      m_BountyPrefab          = null;         //バウンティのプレハブ
    [SerializeField] Transform       m_Contents              = null;         //コンテンツ
    [SerializeField] float           m_DragLength            = 1.25f;        //ドラッグ距離
    [SerializeField] float           m_ScrollTime            = 0.25f;        //移動時間
    [SerializeField] float           m_FlickTime             = 0.75f;        //フリック時間
    [SerializeField] float           m_ScrollAngle           = 10.0f;        //スクロール角度
    [SerializeField] float           m_MaxScale              = 1.1f;         //スケール最大値
    [SerializeField] float           m_MinScale              = 0.8f;         //スケール最小値
    RectTransform[]                  m_BountyArray           = null;         //バウンティ配列
    Vector3                          m_PointerDownPosition   = Vector3.zero; //ポインター初期位置
    Vector3                          m_PointerUpPosition     = Vector3.zero; //ポインター最終位置
    float                            m_ContentsStartAngle    = 0f;           //コンテンツ初期角度
    float                            m_ContentsLastAngle     = 0f;           //コンテンツ最終角度
    bool                             m_ScrollFlag            = false;        //スクロール中フラグ
    bool                             m_PointerDown           = false;        //ポインター押下フラグ
    int                              m_ContentsIndex         = 0;            //コンテンツの番号
    int                              m_BountyIndex           = 0;            //バウンティの番号
    int                              m_BeforeBountyIndex     = 0;            //1フレーム前のバウンティの番号
    woskni.Timer                     m_ScrollTimer;                          //スクロールタイマー
    woskni.Timer                     m_FlickTimer;                           //フリックタイマー

    [HideInInspector]
    public bool                      m_BountyUpdatingFlag    = false;        //バウンティ更新中フラグ

    void Start()
    {
        //子供の大きさを変える
        foreach (Transform child in m_Contents)
        {
            Vector3 scale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            child.localScale = scale;
        }

        //タイマー設定
        m_ScrollTimer.Setup(m_ScrollTime);
        m_FlickTimer.Setup(m_FlickTime);

        //プレハブ複製
        int bountyCount = SaveData.m_bounty_count;
        m_BountyArray = new RectTransform[bountyCount];
        for (int i = 0; i < bountyCount; ++i)
        {
            m_BountyArray[i] = Instantiate(m_BountyPrefab).GetComponent<RectTransform>();
            m_BountyArray[i].gameObject.SetActive(false);
            m_BountyArray[i].name = "Bounty_" + (i + 1).ToString();
            m_BountyArray[i].SetParent(transform);
            m_BountyArray[i].localPosition = Vector3.zero;
            m_BountyArray[i].localScale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            BountyPanel bountyPanel = m_BountyArray[i].GetComponent<BountyPanel>();
            bountyPanel.Initialize(i);
        }

        //最初の位置と親決め
        FirstSetBountyPosition_and_Parent(TemporarySavingBounty.bountyIndex);
    }

    void Update()
    {
        //ポインター押下中ならタイマーを進める
        if (m_PointerDown)
            m_FlickTimer.Update();

        //スクロール中ならスクロール処理と拡縮処理をする
        if (m_ScrollFlag)
        {
            Scaling();
            Scroll();
        }

        //バウンティ番号を文字列に変換する(0から始まるので+1)
        int bountyCount = SaveData.m_bounty_count;
        m_BountyIndexText.text = (m_BountyIndex + 1).ToString() + "/" + bountyCount;
    }

    //スクロール処理
    void Scroll()
    {
        m_ScrollTimer.Update();

        //イージングで角度更新
        float limit = m_ScrollTimer.limit;
        float time  = m_ScrollTimer.time;
        Vector3 rotate = m_Contents.localEulerAngles;
        rotate.z = woskni.Easing.OutSine(time, limit, m_ContentsStartAngle, m_ContentsLastAngle);
        m_Contents.localEulerAngles = rotate;

        //タイマー終了
        if (m_ScrollTimer.IsFinished())
        {
            //状態リセット
            m_ScrollTimer.Reset();
            m_ScrollFlag = false;
            m_Contents.localEulerAngles = new Vector3(0, 0, m_ContentsLastAngle);
        }
    }

    //スケーリング処理
    void Scaling()
    {
        //変数取得
        float limit = m_ScrollTimer.limit;
        float time  = m_ScrollTimer.time;

        //イージングで拡縮
        Vector3 scale = Vector3.one;
        scale.x = scale.y = woskni.Easing.OutSine(time, limit, m_MaxScale, m_MinScale);
        m_BountyArray[m_BeforeBountyIndex].parent.localScale = scale;
        scale.x = scale.y = woskni.Easing.OutSine(time, limit, m_MinScale, m_MaxScale);
        m_BountyArray[m_BountyIndex].parent.localScale = scale;
    }

    //ポインターが押された
    public void PointerDown()
    {
        //タッチ位置保存
        m_PointerDownPosition = woskni.InputManager.GetInputPosition(Application.platform);

        //タイマーリセット
        m_FlickTimer.Reset();

        m_PointerDown = true;

        //バウンティパネルのサイズを取得
        Vector2 size = m_BountyArray[m_BountyIndex].sizeDelta;
        //マウスカーソルとの位置関係を計算
        Vector3 pos = m_PointerDownPosition - m_BountyArray[m_BountyIndex].position;
        //パネルの内側か判定する
        bool isInside = true;
        isInside &= Mathf.Abs(pos.x) < size.x * m_MaxScale / 2;
        isInside &= Mathf.Abs(pos.y) < size.y * m_MaxScale / 2;

        m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().m_IsGriped = isInside;
    }

    //ポインターが離れた
    public void PointerUp()
    {
        m_PointerDown = false;

        m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().m_IsGriped = m_PointerDown;

        //スクロール中は何もしない
        if (m_ScrollFlag) return;

        //一定時間以上タッチしていたら終了
        if (m_FlickTimer.IsFinished()) return;

        //バウンティ更新中なら終了
        if (m_BountyUpdatingFlag) return;

        //タッチ位置保存
        m_PointerUpPosition = woskni.InputManager.GetInputPosition(Application.platform);

        //差分を計算
        Vector3 increment = m_PointerUpPosition - m_PointerDownPosition;

        //横方向の差分が必要距離に満たなかったら終了
        if (Mathf.Abs(increment.x) < m_DragLength)
        {
            //バウンティパネルのサイズを取得
            Vector2 size = m_BountyArray[m_BountyIndex].sizeDelta;
            //マウスカーソルとの位置関係を計算
            Vector3 pos = m_PointerUpPosition - m_BountyArray[m_BountyIndex].position;
            //パネルの内側か判定する
            bool isInside = true;
            isInside &= Mathf.Abs(pos.x) < size.x * m_MaxScale / 2;
            isInside &= Mathf.Abs(pos.y) < size.y * m_MaxScale / 2;

            //パネルをクリックしたと判定する
            if (isInside)
                m_BountyArray[m_BountyIndex].GetComponent<BountyPanel>().ButtonDown();

            return;
        }

        //初期角度代入
        m_ContentsStartAngle = m_Contents.localEulerAngles.z;

        //番号の保存
        m_BeforeBountyIndex = m_BountyIndex;

        //番号の加算
        m_BountyIndex   += increment.x < 0 ? +1 : -1;
        m_ContentsIndex += increment.x < 0 ? +1 : -1;

        //番号の補正(バウンティ番号)
        int bountyCount = SaveData.m_bounty_count;
        if (m_BountyIndex >= bountyCount)   m_BountyIndex = 0;
        else if (m_BountyIndex < 0)         m_BountyIndex = bountyCount - 1;

        //番号の補正(コンテンツ番号)
        int child_count = m_Contents.childCount;
        if (m_ContentsIndex >= child_count) m_ContentsIndex = 0;
        else if (m_ContentsIndex < 0)       m_ContentsIndex = child_count - 1;

        //最終角度計算
        m_ContentsLastAngle = m_ContentsStartAngle + m_ScrollAngle * (increment.x < 0 ? +1 : -1);

        //フラグを上げる
        m_ScrollFlag = true;

        //位置と親決め
        SetBountyPosition_and_Parent(increment.x < 0);
    }

    //最初の位置と親決め
    void FirstSetBountyPosition_and_Parent(int firstIndex)
    {
        //バウンティとコンテンツの要素番号を計算する
        int firstContentsIndex  = m_Contents.childCount - 2;

        //画面に写る3つとその隣の2つを決める
        for (int i = 0; i < 5; i++)
        {
            //要素番号を算出
            int index = firstIndex - 2 < 0 ? SaveData.m_bounty_count + (firstIndex - 2) : firstIndex - 2;
            int b_index = (index + i) % SaveData.m_bounty_count;
            int c_index = (firstContentsIndex + i) % m_Contents.childCount;

            //親とローカル位置・角度・拡大率を設定する
            m_BountyArray[b_index].SetParent(m_Contents.GetChild(c_index));
            m_BountyArray[b_index].localPosition = Vector3.zero;
            m_BountyArray[b_index].localEulerAngles = Vector3.zero;
            m_BountyArray[b_index].parent.localScale =
                            i == 2 ? new Vector3(m_MaxScale, m_MaxScale, 1.0f) :
                                     new Vector3(m_MinScale, m_MinScale, 1.0f);

            //状態をアクティブにする
            m_BountyArray[b_index].gameObject.SetActive(true);
        }

        //番号を設定
        m_BountyIndex = firstIndex;
    }

    //位置と親決め
    void SetBountyPosition_and_Parent(bool leftMove)
    {
        int bountyCount = SaveData.m_bounty_count;
        int sub;
        int firstBountyIndex;
        int lastBountyIndex;
        int firstContentsIndex;
        int lastContentsIndex;

        //左向き
        if (leftMove)
        {
            //取り外すバウンティの場所を特定
            sub = m_BountyIndex - 3;
            firstBountyIndex = sub >= 0 ? sub : bountyCount + sub;

            //親子関係を解除して位置リセット
            m_BountyArray[firstBountyIndex].SetParent(transform);
            m_BountyArray[firstBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[firstBountyIndex].gameObject.SetActive(false);

            //取り付けるバウンティの場所を特定
            sub = m_ContentsIndex + 2;
            lastContentsIndex = sub % m_Contents.childCount;
            sub = m_BountyIndex + 2;
            lastBountyIndex = sub % bountyCount;

            //親子関係を作って位置設定
            m_BountyArray[lastBountyIndex].SetParent(m_Contents.GetChild(lastContentsIndex));
            m_BountyArray[lastBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[lastBountyIndex].localEulerAngles = Vector3.zero;
            m_BountyArray[lastBountyIndex].gameObject.SetActive(true);
        }
        //右向き
        else
        {
            //取り外すバウンティの場所を特定
            sub = m_BountyIndex + 3;
            lastBountyIndex = sub % bountyCount;

            //親子関係を解除して位置リセット
            m_BountyArray[lastBountyIndex].SetParent(transform);
            m_BountyArray[lastBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[lastBountyIndex].gameObject.SetActive(false);

            //取り付けるバウンティの場所を特定
            sub = m_ContentsIndex - 2;
            firstContentsIndex = sub >= 0 ? sub : m_Contents.childCount + sub;
            sub = m_BountyIndex - 2;
            firstBountyIndex = sub >= 0 ? sub : bountyCount + sub;

            //親子関係を作って位置設定
            m_BountyArray[firstBountyIndex].SetParent(m_Contents.GetChild(firstContentsIndex));
            m_BountyArray[firstBountyIndex].localPosition = Vector3.zero;
            m_BountyArray[firstBountyIndex].localEulerAngles = Vector3.zero;
            m_BountyArray[firstBountyIndex].gameObject.SetActive(true);
        }
    }

    /// <summary>バウンティリセット</summary>
    public void BountiesReset()
    {
        //バウンティ情報を全てリセット
        for (int i = 0; i < SaveSystem.m_SaveData.bounties.Length; ++i)
        {
            SaveSystem.m_SaveData.bounties[i] = new Bounty();
            m_BountyArray[i].GetComponent<BountyPanel>().Initialize(i);
        }
    }
}
