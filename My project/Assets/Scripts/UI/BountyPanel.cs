using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//バウンティパネルクラス
public class BountyPanel : MonoBehaviour
{
    /// <summary>フィールド画像構造体</summary>
    [System.Serializable]
    public struct StageImage
    {
        public Image BackGround;  //背景(フィールド)
        public Image EnemyImage;  //敵イメージ
    }

    [Header("コンポーネント")]
    [SerializeField] TextMeshProUGUI    m_BountyNameText        = null;             //バウンティ名
    [SerializeField] StageImage         m_StageImage            = new StageImage(); //フィールド画像
    [SerializeField] TextMeshProUGUI    m_FieldNameText         = null;             //フィールド名
    [SerializeField] TextMeshProUGUI    m_BonusWeaponText       = null;             //ボーナス武器
    [SerializeField] TextMeshProUGUI    m_PointAmmountText      = null;             //ポイント量
    [SerializeField] TextMeshProUGUI    m_LifeAmmountText       = null;             //ライフ量
    [SerializeField] Color              m_IncompletionColor     = Color.white;      //倒していない敵の色
    [SerializeField] RectTransform      m_CompletionImage       = null;             //バウンティ達成画像
    [SerializeField] Image              m_MaskImage             = null;             //マスク画像
    [SerializeField, Range(0.01f, 1.0f)]
                     float              m_MaxMaskAlpha          = 0.4f;             //マスク画像の最大透明度
    [SerializeField, Range(0.01f, 0.1f)]
                     float              m_MaxAlphaDelta         = 0.05f;            //透明度の最大変化量
    [SerializeField, Range(1.0f , 1.5f)]
                     float              m_MaxScale              = 1.1f;             //最大スケール値
    [SerializeField, Range(0.01f, 0.1f)]
                     float              m_MaxScaleDelta         = 0.01f;            //スケール値の最大変化量
    [Space(12)]
    [SerializeField] BountyNameData     m_BountyNameData        = null;             //バウンティ名データ
    [SerializeField] FieldData          m_FieldData             = null;             //フィールドデータ
    [SerializeField] EnemyData          m_EnemyData             = null;             //エネミーデータ
    [SerializeField] WeaponData         m_WeaponData            = null;             //武器データ
    Bounty                              m_Bounty                = null;             //バウンティ情報
    FieldSelect                         m_FieldSelect           = null;             //フィールド選択クラス
    Image                               m_EnemyImage            = null;             //難易度選択時の敵画像
    Transform                           m_MyTransform           = null;             //自身のトランスフォーム

    [HideInInspector]
    public int                          m_BountyID              = 0;                //バウンティID
    [HideInInspector]
    public bool                         m_IsGriped              = false;            //掴まれているフラグ

    public void Initialize(int id)
    {
        //コンポーネントが空ならコンポーネント取得
        m_EnemyImage  ??= GameObject.Find("Difficulty/EnemyImage").GetComponent<Image>();
        m_FieldSelect ??= GameObject.Find("FieldSelect").GetComponent<FieldSelect>();
        m_MyTransform ??= transform;

        //ID設定
        m_BountyID = id;

        //セーブデータからバウンティ情報を取得、編集済みか見る
        m_Bounty = SaveSystem.m_SaveData.bounties[m_BountyID];

        // Bountyが空なら新規生成
        m_Bounty ??= new Bounty();

        //編集済みではなかったら新しく作る
        if (!m_Bounty.editedFlag)
        {
            m_Bounty.CreateBounty(m_FieldData, m_EnemyData, m_BountyNameData, m_WeaponData);
            SaveSystem.m_SaveData.bounties[m_BountyID] = m_Bounty;
            SaveSystem.Save();
        }

        //番号をもとにエネミーとフィールドの情報を取得する
        FieldData.Field field = m_FieldData.GetFieldAt(m_Bounty.fieldIndex);
        EnemyData.Enemy enemy = m_EnemyData.GetEnemyAt(m_Bounty.enemyIndex);

        // * バウンティパネルの情報設定 * //
        //画像設定
        m_StageImage.BackGround.sprite = field.sprite;
        m_StageImage.EnemyImage.sprite = enemy.sprite;

        //画像色決定(倒したことのない敵は黒くする)
        if (!SaveSystem.m_SaveData.completedEnemies[m_Bounty.enemyIndex])
            m_StageImage.EnemyImage.color = m_IncompletionColor;

        //矩形サイズ調整
        Texture2D texture = enemy.sprite.texture;
        m_StageImage.EnemyImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

        //文字列設定
        m_BountyNameText.text   = m_Bounty.bountyName;
        m_FieldNameText.text    = m_FieldData.GetFieldAt(m_Bounty.fieldIndex).name;
        m_BonusWeaponText.text  = m_WeaponData.GetWeaponAt(m_Bounty.weaponIndex).weaponName;
        m_PointAmmountText.text = m_Bounty.point.ToString();

        //文字サイズ調整
        float largeSize = m_LifeAmmountText.fontSize * 1.25f;
        m_LifeAmmountText.text = "<size=" + largeSize + ">" + m_Bounty.life.ToString() + "</size> 回力尽きたら失敗";

        //バウンティがすでに達成済みなら少し待ってから更新する
        if (m_Bounty.completionFlag && woskni.Scene.m_LastScene == woskni.Scenes.GameMain)
            Invoke(nameof(RenewalBounty), 0.25f);
    }

    private void Update()
    {
        //マスク画像のフェード処理(掴まれているフラグで濃くするか薄くするか決める）
        Color color = m_MaskImage.color;
        color.a = Mathf.MoveTowards(color.a, m_IsGriped ? m_MaxMaskAlpha : 0.0f, m_MaxAlphaDelta);
        m_MaskImage.color = color;

        //自身の拡縮処理(掴まれているフラグで大きくするか小さくするか決める）
        Vector3 scale = m_MyTransform.localScale;
        scale.x = scale.y = Mathf.MoveTowards(scale.x, m_IsGriped ? m_MaxScale : 1.0f, m_MaxScaleDelta);
        m_MyTransform.localScale = scale;
    }

    //バウンティ更新
    public void RenewalBounty()
    {
        //コルーチンスタート
        StartCoroutine(UpdateBounty());
    }

    IEnumerator UpdateBounty()
    {
        //バウンティ更新中フラグを上げる
        BountyScrollView bountyScrollView = GameObject.Find("BountyScrollView").GetComponent<BountyScrollView>();
        bountyScrollView.m_BountyUpdatingFlag = true;

        //キャンバスグループ取得
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        //タイマー生成
        woskni.Timer timer = new woskni.Timer();
        timer.Setup(0.4f);

        //バウンティ達成の画像のコンポーネント取得
        Image image = m_CompletionImage.GetComponent<Image>();

        while (true)
        {
            //タイマー更新
            timer.Update();

            //イージングで拡縮
            float size = woskni.Easing.OutBounce(timer.time, timer.limit, 1.0f, 0.55f);
            m_CompletionImage.localScale = new Vector2(size, size);

            //イージングでフェードさせる
            float alpha = woskni.Easing.OutSine(timer.time, timer.limit, 0.0f, 1.0f);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            //１フレーム待機
            yield return null;

            //タイマー終了でループ脱出
            if (timer.IsFinished())
            {
                //誤差修正
                m_CompletionImage.localScale = new Vector2(0.55f, 0.55f);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
                break;
            }
        }

        //タイマーリセット
        timer.Reset();

        //少し待機
        yield return new WaitForSeconds(0.1f);

        // 移動地点を設定
        Vector3 firstPos = transform.localPosition;
        Vector3 lastPos = firstPos + new Vector3(0, 300, 0);

        while (true)
        {
            //タイマー更新
            timer.Update();

            //イージングで動かす
            Vector3 pos = transform.localPosition;
            pos.y = woskni.Easing.OutQuintic(timer.time, timer.limit, firstPos.y, lastPos.y);
            transform.localPosition = pos;

            //イージングでフェードさせる
            canvasGroup.alpha = woskni.Easing.InQuart(timer.time, timer.limit, 1.0f, 0.0f);

            //１フレーム待機
            yield return null;

            //タイマー終了でループ脱出
            if (timer.IsFinished())
            {
                //誤差修正
                transform.localPosition = lastPos;
                canvasGroup.alpha = 0.0f;
                break;
            }
        }

        //タイマーリセット
        timer.Reset();

        //編集済みフラグ解除
        m_Bounty.editedFlag = false;

        //バウンティをリセット
        Initialize(m_BountyID);

        //少し待機
        yield return new WaitForSeconds(0.1f);

        //達成画像を見えなくする
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);

        // 移動地点を再設定
        lastPos = firstPos;
        firstPos = transform.localPosition;

        while (true)
        {
            //タイマー更新
            timer.Update();

            //イージングで動かす
            Vector3 pos = transform.localPosition;
            pos.y = woskni.Easing.OutQuintic(timer.time, timer.limit, firstPos.y, lastPos.y);
            transform.localPosition = pos;

            //イージングでフェードさせる
            canvasGroup.alpha = woskni.Easing.OutQuart(timer.time, timer.limit, 0.0f, 1.0f);

            //１フレーム待機
            yield return null;

            //タイマー終了でループ脱出
            if (timer.IsFinished())
            {
                //誤差修正
                transform.localPosition = lastPos;
                canvasGroup.alpha = 1.0f;
                break;
            }
        }

        //バウンティ更新中フラグを下げる
        bountyScrollView.m_BountyUpdatingFlag = false;

        //コルーチン終了
        yield break;
    }

    public void ButtonDown()
    {
        //情報設定
        TemporarySavingBounty.field        = m_FieldData.GetFieldAt(m_Bounty.fieldIndex);
        TemporarySavingBounty.enemy        = m_EnemyData.GetEnemyAt(m_Bounty.enemyIndex);
        TemporarySavingBounty.bonusWeapon  = m_WeaponData.GetWeaponAt(m_Bounty.weaponIndex);
        TemporarySavingBounty.bountyName   = m_Bounty.bountyName;
        TemporarySavingBounty.point        = m_Bounty.point;
        TemporarySavingBounty.life         = m_Bounty.life;
        TemporarySavingBounty.bountyIndex  = m_BountyID;

        //画像の設定
        m_EnemyImage.sprite = m_StageImage.EnemyImage.sprite;
        m_EnemyImage.color  = m_StageImage.EnemyImage.color;

        //画像サイズ変更
        Vector2 size = (TemporarySavingBounty.enemy.sprite.rect.size + Vector2.one * 100f) / 2f;
        m_EnemyImage.rectTransform.sizeDelta = size;

        //難易度選択画面へ
        m_FieldSelect.ShowDifficulty();
    }
}
