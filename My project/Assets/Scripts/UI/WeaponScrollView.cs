using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器スクロールビュークラス
public class WeaponScrollView : MonoBehaviour
{
    [SerializeField] NotAllowedBuyDialog
                                m_CanNotBuyDialog       = null;             //購入不可ダイアログ
    [SerializeField] GameObject m_WeaponBuyDialog       = null;             //武器購入ダイアログ
    [SerializeField] GameObject m_WeaponPrefab          = null;             //武器プレハブ
    [SerializeField] Transform  m_Contents              = null;             //コンテンツ
    [SerializeField] WeaponInfo m_WeaponInfo            = null;             //武器情報クラス
    [SerializeField] float      m_ScrollSpeed           = 0.25f;            //移動速度
    [SerializeField] float      m_ScalingSpeed          = 0.25f;            //拡縮速度
    [SerializeField] float      m_WeaponSize_X          = 250.0f;           //武器横サイズ
    [SerializeField] float      m_CotentInterval        = 50.0f;            //コンテンツの整列間隔
    [SerializeField] float      m_MaxScale              = 1.1f;             //最大スケール
    [SerializeField] float      m_MinScale              = 0.8f;             //最小スケール
    [SerializeField] float      m_SpeedDamping          = 0.98f;            //速度減衰
    [SerializeField] float      m_MinScrollVelocity     = 0.5f;             //最小スクロール速度
    [SerializeField] float      m_MinFlickLength        = 10.0f;            //最小フリック距離
    [SerializeField] float      m_ClickMoveTime         = 1.0f;             //クリック時の移動時間
    [SerializeField] float      m_BaseScreenWidth       = 1080f;            //画面横幅基準値
    [SerializeField] int        m_MaxWeaponCount        = 8;                //武器最大量
    [Space(12)]
    public           WeaponData m_WeaponData            = null;             //武器データ
    Vector3                     m_PointerDownPosition   = Vector3.zero;     //ポインター初期位置
    Vector3                     m_PointerDragPosition   = Vector3.zero;     //現在ポインター位置
    Vector3                     m_ContentsFirstPosition = Vector3.zero;     //コンテンツ初期位置(ゲーム開始時)
    Vector3                     m_ContentsStartPosition = Vector3.zero;     //コンテンツ初期位置
    Vector3                     m_ContentsLastPosition  = Vector3.zero;     //コンテンツ最終位置
    float                       m_ContentsEndPositionX  = 0.0f;             //コンテンツ移動限界(X軸)
    float                       m_ScrollVelocity        = 0.0f;             //コンテンツスクロール速度
    float                       m_DragTime              = 0.0f;             //ドラッグ時間
    bool                        m_PointerDownFlag       = false;            //ポインター押下フラグ
    int                         m_CenterWeaponIndex     = 0;                //中心に来る武器の番号
    int                         m_BeforeWeaponIndex     = 0;                //1フレーム前に中心にいた武器の番号
    woskni.Timer                m_ClickMoveTimer;                           //クリック移動タイマー

    void Start()
    {
        //タイマー設定
        m_ClickMoveTimer.Setup(m_ClickMoveTime);

        //購入ダイアログは表示を消しておく
        m_WeaponBuyDialog.SetActive(false);

        //初期位置保存
        m_ContentsFirstPosition = m_Contents.localPosition;
        m_ContentsStartPosition = m_ContentsFirstPosition;
        m_ContentsLastPosition  = m_ContentsFirstPosition;

        //限界位置計算
        m_ContentsEndPositionX = m_ContentsFirstPosition.x -
                                    ((m_WeaponSize_X + m_CotentInterval) * (m_MaxWeaponCount - 1));

        //武器パネル生成
        List<WeaponData.Weapon> weaponList = m_WeaponData.GetWeaponList();
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            GameObject obj = Instantiate(m_WeaponPrefab, m_Contents);
            obj.name = "Weapon_" + (i + 1).ToString();
            obj.transform.localScale = new Vector3(m_MinScale, m_MinScale, 1.0f);
            obj.GetComponent<WeaponPanel>().Setup(i, weaponList[i].weaponImage, this);
        }

        //先頭の要素だけ大きくする
        m_Contents.GetChild(0).localScale = new Vector3(m_MaxScale, m_MaxScale, 1.0f);

        //武器情報クラスに初期値を入れる
        m_WeaponInfo.SetWeaponIndex(0);

        //最初の武器をあらかじめ選んでおく(装備フラグを上げる)
        TemporarySavingBounty.equipWeapon = m_WeaponData.GetWeaponAt(SaveSystem.m_SaveData.eqipWeaponID);

        //中心の武器の番号を取得
        m_CenterWeaponIndex = SaveSystem.m_SaveData.eqipWeaponID;
        Vector3 pos = m_Contents.localPosition;
        pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * SaveSystem.m_SaveData.eqipWeaponID;
        m_Contents.localPosition = pos;
        m_ContentsStartPosition = pos;
        m_ContentsLastPosition = pos;
    }

    void Update()
    {
        //子要素のスケーリング
        ChildScaling();

        //タイマーが稼働中ならイージングで移動
        if (!m_ClickMoveTimer.IsFinished())
        {
            m_ClickMoveTimer.Update();
            Vector3 pos = m_Contents.localPosition;
            float time  = m_ClickMoveTimer.time;
            float limit = m_ClickMoveTimer.limit;
            pos.x = woskni.Easing.OutSine(time, limit, m_ContentsStartPosition.x, m_ContentsLastPosition.x);
            m_Contents.localPosition = pos;
            return;
        }

        //押下中は何もしない
        if (m_PointerDownFlag)
        {
            //ポインター位置取得
            m_PointerDragPosition = woskni.InputManager.GetInputPosition(Application.platform);
            return;
        }

        //スクロール速度が一定以上ある
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity)
        {
            //速度を現在位置に加算(速度減衰もさせる)
            m_Contents.localPosition += new Vector3(m_ScrollVelocity * Time.deltaTime, 0f, 0f);

            //範囲を外れないように補正
            Vector3 pos = m_Contents.localPosition;
            pos.x = Mathf.Clamp(pos.x, m_ContentsEndPositionX, m_ContentsFirstPosition.x);
            m_Contents.localPosition = pos;
        }
        //スクロール速度がほとんどない
        else
        {
            //保存したX位置まで滑らかに動く
            Vector3 pos = m_Contents.localPosition;
            pos.x = Mathf.MoveTowards(pos.x, m_ContentsLastPosition.x, m_ScrollSpeed * Time.deltaTime);
            m_Contents.localPosition = pos;

            //初期位置保存
            m_ContentsStartPosition = m_Contents.localPosition;

            //速度削除
            m_ScrollVelocity = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        //タイマーが稼働中なら何もしない
        if (!m_ClickMoveTimer.IsFinished()) return;

        //押下中は何もしない
        if (m_PointerDownFlag)
        {
            //時間加算
            m_DragTime += Time.fixedDeltaTime;
            return;
        }

        //スクロール速度がある程度ある時は減衰させる
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity)
        {
            m_ScrollVelocity *= m_SpeedDamping;
            //速度チェック
            CheckVelocity();
        }
    }

    /// <summary>現在の速度チェック</summary>
    void CheckVelocity()
    {
        //速度がまだあったら終了
        if (Mathf.Abs(m_ScrollVelocity) > m_MinScrollVelocity) return;

        //速度がなくなったら武器の番号を取得する
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            //初期位置から等間隔の位置を出す
            float increment_x = (m_WeaponSize_X + m_CotentInterval) * i;
            Vector3 point = m_ContentsFirstPosition - new Vector3(increment_x, 0f, 0f);

            //「パネル位置」より大きい
            bool min = point.x - (m_WeaponSize_X + m_CotentInterval) / 2 <= m_Contents.localPosition.x;
            //「パネル位置 + サイズ」より小さい
            bool max = point.x + (m_WeaponSize_X + m_CotentInterval) / 2 >= m_Contents.localPosition.x;

            //位置が武器パネルの範囲内
            if (min && max)
            {
                //一番近い項目の位置を保存して終了
                Vector3 pos = m_Contents.localPosition;
                pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * i;
                m_ContentsLastPosition = pos;
                m_CenterWeaponIndex = i;
                return;
            }
        }
    }

    /// <summary>武器パネルがクリックされた</summary>
    /// <param name="weapon_id">武器ID</param>
    public void PanelClick(int weapon_id)
    {
        //タイマーが稼働中なら何もしない
        if (!m_ClickMoveTimer.IsFinished()) return;

        //クリックされた武器パネルの番号と今中心にいる武器パネルの番号が違う
        if (weapon_id != m_CenterWeaponIndex)
        {
            //クリックされた項目の位置を保存
            Vector3 pos = m_Contents.localPosition;
            pos.x = m_ContentsFirstPosition.x - (m_WeaponSize_X + m_CotentInterval) * weapon_id;
            m_ContentsLastPosition = pos;
            //速度を消す
            m_ScrollVelocity = 0.0f;
            //タイマーリセット
            m_ClickMoveTimer.Reset();
            //クリックされた武器の番号を保存
            m_CenterWeaponIndex = weapon_id;
        }
        //クリックされた武器パネルの番号と今中心にいる武器パネルの番号が同じ
        else
        {
            //すでに購入済みならここで終了
            if (SaveSystem.m_SaveData.hasWeapons[weapon_id])
            {
                //選んだ武器を装備する(装備フラグを上げる)
                TemporarySavingBounty.equipWeapon = m_WeaponData.GetWeaponAt(weapon_id);
                SaveSystem.m_SaveData.eqipWeaponID = weapon_id;
                SaveSystem.Save();

                return;
            }

#if true
            if (m_WeaponData.GetWeaponAt(weapon_id).presetIndex.Count <= 0)
            {
                m_CanNotBuyDialog.ShowDialog();
                return;
            }
#endif

            //武器購入ダイアログ表示
            m_WeaponBuyDialog.SetActive(true);
            m_WeaponBuyDialog.GetComponent<WeaponBuyDialog>().ShowDialog(weapon_id);
        }
    }

    /// <summary>子要素のスケーリング</summary>
    void ChildScaling()
    {
        int num = 0;
        for (int i = 0; i < m_MaxWeaponCount; ++i)
        {
            //初期位置から等間隔の位置を出す
            float increment_x = (m_WeaponSize_X + m_CotentInterval) * i;
            Vector3 point = m_ContentsFirstPosition - new Vector3(increment_x, 0f, 0f);

            //初期位置より大きい
            bool min = point.x - (m_WeaponSize_X + m_CotentInterval) / 2 <= m_Contents.localPosition.x;
            //初期位置+サイズより小さい
            bool max = point.x + (m_WeaponSize_X + m_CotentInterval) / 2 >= m_Contents.localPosition.x;

            //該当する子供を取得
            Transform child = m_Contents.GetChild(i);

            //範囲内
            if (min && max)
            {
                //滑らかに大きくする
                Vector3 scale = child.localScale;
                scale.x = scale.y = Mathf.MoveTowards(scale.x, m_MaxScale, m_ScalingSpeed * Time.deltaTime);
                child.localScale = scale;

                //番号の保存
                num = i;
            }
            //範囲外
            else
            {
                //滑らかに小さくする
                Vector3 scale = child.localScale;
                scale.x = scale.y = Mathf.MoveTowards(scale.x, m_MinScale, m_ScalingSpeed * Time.deltaTime);
                child.localScale = scale;
            }
        }

        //武器の番号が変わった瞬間
        if (m_BeforeWeaponIndex != num)
            //武器情報クラスに中心にいる武器の番号を渡す
            m_WeaponInfo.SetWeaponIndex(num);

        //1フレーム前の番号を保存
        m_BeforeWeaponIndex = num;
    }

    /// <summary>ポインターが押された</summary>
    public void PointerDown()
    {
        //タッチ位置保存
        m_PointerDownPosition = woskni.InputManager.GetInputPosition(Application.platform);
        m_PointerDragPosition = m_PointerDownPosition;

        //速度消去
        m_ScrollVelocity = 0.0f;

        //時間消去
        m_DragTime = 0.0f;

        //フラグを上げる
        m_PointerDownFlag = true;

        //初期位置保存
        m_ContentsStartPosition = m_Contents.localPosition;
    }

    /// <summary>ポインターが離れた</summary>
    public void PointerUp()
    {
        //速度算出
        Vector3 nowPointerPos = woskni.InputManager.GetInputPosition(Application.platform);
        float increment = (nowPointerPos - m_PointerDownPosition).x / (Screen.width / m_BaseScreenWidth);

        //フリック距離が一定以上ある
        if (Mathf.Abs(increment) > m_MinFlickLength)
            m_ScrollVelocity = increment / m_DragTime;

        //フラグを下げる
        m_PointerDownFlag = false;
    }

    /// <summary>ポインターをドラッグ</summary>
    public void PointerDrag()
    {
        //差分計算
        Vector3 increment = Vector3.zero;
        increment.x = (m_PointerDragPosition - m_PointerDownPosition).x / (Screen.width / m_BaseScreenWidth);

        //位置代入
        m_Contents.localPosition = m_ContentsStartPosition + increment;

        //範囲を外れないように補正
        Vector3 pos = m_Contents.localPosition;
        pos.x = Mathf.Clamp(pos.x, m_ContentsEndPositionX, m_ContentsFirstPosition.x);

        //補正値代入
        m_Contents.localPosition = pos;
    }

    /// <summary>スクロール位置リセット</summary>
    public void PositionReset()
    {
        m_Contents.localPosition = m_ContentsFirstPosition;
    }
}
