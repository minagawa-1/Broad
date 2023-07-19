using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Result : MonoBehaviour
{
    [Header("画像・テキストコンポーネント")]
    [woskni.Name("フィールド背景"),       SerializeField] Image                 m_FieldImage;
    [woskni.Name("敵画像"),               SerializeField] Image                 m_EnemyImage;
    [woskni.Name("倒していない敵の色"),   SerializeField] Color                 m_IncompletionColor;

    [woskni.Name("背景画像"),             SerializeField] Image                 m_BackImage;
    [woskni.Name("バウンティ画像"),       SerializeField] Image                 m_BountyImage;
    [woskni.Name("はんこ画像"),           SerializeField] Image                 m_StampImage;
    [woskni.Name("バウンティ名"),         SerializeField] TextMeshProUGUI       m_BountyName;
    [woskni.Name("左側のテキスト"),       SerializeField] List<TextMeshProUGUI> m_LeftTextList;
    [woskni.Name("右側のテキスト"),       SerializeField] List<TextMeshProUGUI> m_RightTextList;
    [woskni.Name("下側の金額のテキスト"), SerializeField] TextMeshProUGUI       m_BottomYenText;
    [woskni.Name("時計"),                 SerializeField] Text                  m_ClockText;

    [Header("はんこ画像")]
    [woskni.Name("達成スプライト"),   SerializeField] Sprite m_CompletionSprite;
    [woskni.Name("未達成スプライト"), SerializeField] Sprite m_IncompletionSprite;

    [Header("タイマー用時間")]
    [woskni.Name("フェード時間"),     SerializeField] float m_FadeTime;
    [woskni.Name("はんこ時間"),       SerializeField] float m_StampTime;
    [woskni.Name("表示時間(紙)"),     SerializeField] float m_AppearBountyflyerTime;
    [woskni.Name("表示時間(文字)"),   SerializeField] float m_AppearBountytextTime;
    [woskni.Name("所持金増加時間"),   SerializeField] float m_YenGainTime;
    [woskni.Name("タイトル遷移時間"), SerializeField] float m_TransitionTime;

    [Header("最終金額表示")]
    [woskni.Name("金額背景"), SerializeField] Image           m_YenFrame;
    [woskni.Name("追加金額"), SerializeField] TextMeshProUGUI m_YenGainText;
    [woskni.Name("最終金額"), SerializeField] TextMeshProUGUI m_FinallyYenText;

    Vector3 m_BasisPosition;
    Vector3 m_FieldBasisPosition;
    Vector3 m_EnemyBasisPosition;

    woskni.Timer m_Timer;
    woskni.Timer m_TransitionTimer;

    // 武器ボーナスのフラグ
    bool m_BonusFlag = false;

    bool m_SceneChangeCompletedFlag = false;

    // 演算語の報酬金
    int m_FinallyReward = 0;

    // イージング開始時の金額
    int m_EasingStartYen;

    enum State
    {
          FadeOut       // フェード処理
        , ShowBounty    // バウンティ表示
        , ShowStamp     // スタンプ表示
        , ShowLeft      // 左側テキスト表示
        , ShowRight     // 右側テキスト表示
        , ShowBottom    // 下側テキスト表示
        , AddYen        // お金追加処理&表示
        , Finish        // 終了処理
    }

    State m_State;

    int m_CurrentAppearNum;

    void Start()
    {
        // バウンティクラスの達成フラグを見て、はんこ画像に反映させる
        int bounty_id = TemporarySavingBounty.bountyIndex;
        Bounty bounty = SaveSystem.m_SaveData.bounties[bounty_id];
        m_StampImage.sprite = bounty.completionFlag ? m_CompletionSprite : m_IncompletionSprite;

        // フィールド・敵の画像
        {
            // 画像設定
            m_FieldImage.sprite = TemporarySavingBounty.field.sprite;
            m_EnemyImage.sprite = TemporarySavingBounty.enemy.sprite;

            // アルファ値設定
            m_FieldImage.color = GetAlphaColor(m_FieldImage.color, 0f);
            m_EnemyImage.color = GetAlphaColor(m_EnemyImage.color, 0f);

            // 位置設定
            m_FieldBasisPosition = m_FieldImage.rectTransform.position;
            m_EnemyBasisPosition = m_EnemyImage.rectTransform.position;

            // 画像色決定(倒したことのない敵は黒くする)
            bool flag = SaveSystem.m_SaveData.completedEnemies[bounty.enemyIndex];
            Color color = flag ? Color.white : m_IncompletionColor;
            color.a = 0;
            m_EnemyImage.color = color;

            // 矩形サイズ調整
            Texture2D texture = m_EnemyImage.sprite.texture;
            m_EnemyImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }

        // BGMの音量を下げる
        woskni.SoundManager.ChangeVolume(TemporarySavingBounty.field.BGM, SaveSystem.m_SaveData.BGMvolume * 0.3f, m_FadeTime);

        // タイマーを設定
        m_Timer.Setup(m_FadeTime);

        m_BackImage.color   = GetAlphaColor(m_BackImage.color  , 0f);
        m_BountyImage.color = GetAlphaColor(m_BountyImage.color, 0f);
        m_BountyName.color  = GetAlphaColor(m_BountyName.color , 0f);
        m_StampImage.color  = GetAlphaColor(m_StampImage.color , 0f);

        m_BasisPosition = m_BountyImage.rectTransform.position;

        // バウンティ名設定
        m_BountyName.text = bounty.bountyName;

        for (int i = 0; i < m_LeftTextList.Count; ++i) {
            m_LeftTextList[i].color  = GetAlphaColor(m_LeftTextList[i].color, 0f);
            m_RightTextList[i].color = GetAlphaColor(m_RightTextList[i].color, 0f);
        }

        // ボーナスフラグを判定
        m_BonusFlag = TemporarySavingBounty.bonusWeapon.weaponName == TemporarySavingBounty.equipWeapon.weaponName;

        float difficulty = woskni.Math.Round(TemporarySavingBounty.difficulty * 2.00f + 1.00f, -2);
        difficulty -= 0.01f;

        // 右側のテキストの値情報
        TemporarySavingBounty.
               point = bounty.completionFlag ? bounty.point : 0;                     // 敗北だったら0ポイント
        m_RightTextList[0].text = TemporarySavingBounty.point.ToString() + " 円";    // 報酬金
        m_RightTextList[1].text = m_BonusFlag ? "達成！　x1.5" : "未達成";           // ボーナス武器
        m_RightTextList[2].text = "x" + difficulty.ToString("F2");       // 難易度ボーナス

        // 最終金額
        ////m_FinallyReward = (int)(TemporarySavingBounty.point * (m_BonusFlag ? 1.5f : 1f) * (difficulty * 2f + 1f));

        m_FinallyReward = woskni.Math.Round((int)(TemporarySavingBounty.point * (m_BonusFlag ? 1.5f : 1f) * difficulty), 2);

        // 精算額
        m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, 0f);
        m_BottomYenText.text  = m_FinallyReward.ToString() + " 円";

        // 追加金額
        // 敗北だったら文字色は白
        m_YenGainText.color = m_FinallyReward == 0 ? Color.white : m_YenGainText.color;
        m_YenGainText.color = GetAlphaColor(m_YenGainText.color, 0f);
        m_YenGainText.text  = "+" + m_FinallyReward.ToString();

        // 最終金額
        m_FinallyYenText.color = GetAlphaColor(m_FinallyYenText.color, 0f);
        m_FinallyYenText.text  = SaveSystem.m_SaveData.money.ToString();

        m_YenFrame.color = GetAlphaColor(m_YenFrame.color, 0f);

        m_State = State.FadeOut;

        m_CurrentAppearNum = 0;

        // イージング開始時の金額を設定
        m_EasingStartYen = SaveSystem.m_SaveData.money;

        m_ClockText.text = "";
    }

    void Update()
    {
        switch (m_State)
        {
            case State.FadeOut:    FadeOut();                           break;
            case State.ShowBounty: ShowBounty();                        break;
            case State.ShowStamp:  ShowStamp();                         break;
            case State.ShowLeft:   ShowLeftRight(ref m_LeftTextList, m_RightTextList);                                  break;
            case State.ShowRight:  ShowLeftRight(ref m_RightTextList, new List<TextMeshProUGUI>() { m_BottomYenText });    break;
            case State.ShowBottom: ShowBottom();                        break;
            case State.AddYen:     AddYen();                            break;
            case State.Finish:     Finish();                            break;
        }
    }

    /// <summary>アルファ値を変更した色を返す</summary>
    /// <param name="color">元の色</param>
    /// <param name="alpha">アルファ値 (0 to 1)</param>
    Color GetAlphaColor(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

    /// <summary>フェード処理</summary>
    /// <remarks>フェード処理を行い、背景を暗くする処理</remarks>
    void FadeOut()
    {
        m_Timer.Update();

        // フェード時間が終了するまで透明度を弄る処理を行う
        float alpha = woskni.Easing.Linear(m_Timer.time, m_Timer.limit, 0f, 0.5f);

        m_BackImage.color = GetAlphaColor(m_BackImage.color, alpha);

        if (m_Timer.IsFinished())
        {
            m_BackImage.color = GetAlphaColor(m_BackImage.color, 0.5f);

            m_State = State.ShowBounty;

            m_Timer.Setup(m_AppearBountyflyerTime);
        }
    }

    /// <summary>バウンティ表示</summary>
    /// <remarks>バウンティ画像を表示するための処理</remarks>
    void ShowBounty()
    {
        m_Timer.Update();

        // フェード時間が終了するまでイージングを行う
        {
            float time = m_Timer.time;
            float limit = m_Timer.limit;

            float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);

            const float dif = -120f;
            float easingDif = woskni.Easing.OutQuart(time, limit, dif, 0f);

            // 位置
            m_BountyImage.rectTransform.position = m_BasisPosition     .Difference(y: easingDif);
            m_FieldImage .rectTransform.position = m_FieldBasisPosition.Difference(y: easingDif);
            m_EnemyImage .rectTransform.position = m_EnemyBasisPosition.Difference(y: easingDif);

            // アルファ値
            m_BountyImage   .color  = GetAlphaColor(m_BountyImage   .color, alpha);
            m_BountyName    .color  = GetAlphaColor(m_BountyName    .color, alpha);

            m_FieldImage    .color  = GetAlphaColor(m_FieldImage    .color, alpha);
            m_EnemyImage    .color  = GetAlphaColor(m_EnemyImage    .color, alpha);

            m_FinallyYenText.color  = GetAlphaColor(m_FinallyYenText.color, alpha);
            m_YenFrame      .color  = GetAlphaColor(m_YenFrame      .color, alpha);
        }

        // タイマー終了
        if (m_Timer.IsFinished())
        {
            m_BountyImage.rectTransform.position = m_BasisPosition;
            m_FieldImage .rectTransform.position = m_FieldBasisPosition;
            m_EnemyImage .rectTransform.position = m_EnemyBasisPosition;

            m_BountyImage   .color = GetAlphaColor(m_BountyImage.color   , 1f);
            m_FieldImage    .color = GetAlphaColor(m_FieldImage.color    , 1f);
            m_EnemyImage    .color = GetAlphaColor(m_EnemyImage.color    , 1f);
            m_BountyName    .color = GetAlphaColor(m_BountyName.color    , 1f);
            m_FinallyYenText.color = GetAlphaColor(m_FinallyYenText.color, 1f);
            m_YenFrame      .color = GetAlphaColor(m_YenFrame.color      , 1f);

            m_State = State.ShowStamp;

            m_Timer.Setup(m_StampTime);
        }
    }
    /// <summary>スタンプ表示</summary>
    /// <remarks>達成・未達成のスタンプを押す処理</remarks>
    void ShowStamp()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear (time, limit, 0f, 1f);
        float scale = woskni.Easing.OutBack(time, limit, 1.5f, 1f, 2f);

        m_StampImage.color = GetAlphaColor(m_StampImage.color, alpha);
        m_StampImage.rectTransform.localScale = new Vector3(scale, scale, 1f);

        if(m_Timer.IsFinished())
        {
            m_StampImage.color = GetAlphaColor(m_StampImage.color, 1f);
            m_StampImage.rectTransform.localScale = Vector3.one;

            m_CurrentAppearNum = 0;

            m_BasisPosition = m_LeftTextList[m_CurrentAppearNum].rectTransform.position;

            m_State = State.ShowLeft;

            m_Timer.Setup(m_AppearBountytextTime);
        }
    }

    /// <summary>テキスト表示</summary>
    /// <param name="textList">処理するテキスト</param>
    /// <param name="nextTextList">次に処理するテキスト</param>
    /// <remarks>複数のテキストをリスト順に表示させる処理</remarks>
    void ShowLeftRight(ref List<TextMeshProUGUI> textList, List<TextMeshProUGUI> nextTextList)
    {
        m_Timer.Update();

        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutQuart(time, limit, -64f, 0f);

        textList[m_CurrentAppearNum].color = GetAlphaColor(textList[m_CurrentAppearNum].color, alpha);
        textList[m_CurrentAppearNum].rectTransform.position = m_BasisPosition + new Vector3(0f, dif, 0f);

        if(m_Timer.IsFinished())
        {
            m_Timer.Reset();

            textList[m_CurrentAppearNum].color = GetAlphaColor(textList[m_CurrentAppearNum].color, 1f);
            textList[m_CurrentAppearNum].rectTransform.position = m_BasisPosition;

            // 次テキストがリスト最後尾を超過したら
            if (++m_CurrentAppearNum >= textList.Count)
            {
                m_CurrentAppearNum = 0;

                Debug.Log(nextTextList[m_CurrentAppearNum]);

                m_BasisPosition = nextTextList[m_CurrentAppearNum].rectTransform.position;

                switch (m_State) {
                    case State.ShowLeft:  m_State = State.ShowRight;  break;
                    case State.ShowRight: m_State = State.ShowBottom; break;

                    default: Debug.LogError("Stateが例外です: " + m_State.ToString()); break;
                }
            }
            else
                m_BasisPosition = textList[m_CurrentAppearNum].rectTransform.position;
        }
    }

    /// <summary>テキスト表示</summary>
    /// <remarks>最終的な報酬金額を表示する</remarks>
    void ShowBottom()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.Linear(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutQuart(time, limit, -64f, 0f);

        m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, alpha);
        m_BottomYenText.rectTransform.position = m_BasisPosition + new Vector3(0f, dif);

        if (m_Timer.IsFinished())
        {
            m_Timer.Setup(m_YenGainTime);
            m_State = State.AddYen;

            m_BottomYenText.color = GetAlphaColor(m_BottomYenText.color, 1f);
            m_BottomYenText.rectTransform.position = m_BasisPosition;

            m_BasisPosition = m_YenGainText.rectTransform.position;
        }
    }

    /// <summary>所持金の増額処理</summary>
    void AddYen()
    {
        m_Timer.Update();
        float time = m_Timer.time;
        float limit = m_Timer.limit;

        float alpha = woskni.Easing.OutExp(time, limit, 0f, 1f);
        float dif = woskni.Easing.OutExp(time, limit, -16f, 0f);

        // いくら追加されたか表示する
        m_YenGainText.color = GetAlphaColor(m_YenGainText.color, alpha);
        m_YenGainText.rectTransform.position = m_BasisPosition + new Vector3(0f, dif);

        SaveSystem.m_SaveData.money = 
            (int)woskni.Easing.OutQuart(time, limit, m_EasingStartYen, Mathf.Min(9999999, m_EasingStartYen + m_FinallyReward));

        m_FinallyYenText.text = SaveSystem.m_SaveData.money.ToString();

        if (m_Timer.IsFinished())
        {
            // セーブする
            SaveSystem.m_SaveData.money = Mathf.Min(99999999, m_EasingStartYen + m_FinallyReward);

            m_YenGainText.color = GetAlphaColor(m_YenGainText.color, 1f);
            m_YenGainText.rectTransform.position = m_BasisPosition;

            m_FinallyYenText.text = SaveSystem.m_SaveData.money.ToString();

            m_Timer.Reset();
            m_TransitionTimer.Setup(m_TransitionTime);
            m_State = State.Finish;
        }
    }

    /// <summary>終了</summary>
    /// <remarks>シーン遷移等の処理</remarks>
    void Finish()
    {
        if(!m_TransitionTimer.IsFinished())
            m_TransitionTimer.Update();
        else
        {
            FinishChangeScene(true);
            return;
        }

        m_ClockText.text = (m_TransitionTimer.TimeLeft() + 0.5f).ToString("F0");

        // 14.2567秒 => 0.256
        float time01 = woskni.Easing.OutCubic(new woskni.Range(0f, 1f).GetAround(m_TransitionTimer.time), 1f, 0f, 1f);

        m_ClockText.color = m_ClockText.color.GetAlphaColor(1f - time01);

        //シーンチェンジが確定したらここで終了
        if (m_SceneChangeCompletedFlag) return;

        if (woskni.InputManager.IsButtonUp())
            FinishChangeScene(false);
    }

    void FinishChangeScene(bool afkFlag)
    {
        m_TransitionTimer.Fin();

        m_ClockText.text = "";

        SaveSystem.m_SaveData.AFKflag = afkFlag;
        SaveSystem.Save();

        woskni.SoundManager.ChangeVolume(TemporarySavingBounty.field.BGM, 0f, 1f);

        woskni.Scene.Change(woskni.Scenes.Title);

        m_SceneChangeCompletedFlag = true;
    }
}
