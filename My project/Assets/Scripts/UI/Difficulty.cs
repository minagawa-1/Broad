using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//難易度クラス
public class Difficulty : MonoBehaviour
{
    [SerializeField] Color  m_HardestColor      = Color.red;    //一番難しい時の色
    [SerializeField] Color  m_EasiestColor      = Color.green;  //一番易しい時の色
    [SerializeField] Image  m_SliderFillArea    = null;         //塗りつぶしエリア
    [SerializeField] Slider m_Slider            = null;         //スライダー
    [SerializeField] float  m_BaseScreenHeight  = 1920f;        //画面高さ基準値
    Transform               m_SliderTransform   = null;         //スライダーのトランスフォーム
    Vector3                 m_PointerPos        = Vector3.zero; //ポインター位置
    float                   m_Hight             = 0.0f;         //スライダー高さ
    bool                    m_TouchFlag         = false;        //タッチフラグs

    /// <summary>難易度取得</summary>
    public float GetDifficulty() { return m_Slider.value; }

    /// <summary>スライダーの位置リセット</summary>
    public void ResetSlider() { m_Slider.value = SaveSystem.m_SaveData.difficulty; }

    void Start()
    {
        //情報取得
        m_Hight = m_Slider.GetComponent<RectTransform>().sizeDelta.y;
        m_SliderTransform = m_Slider.transform;

        //データ読み込み
        m_Slider.value = SaveSystem.m_SaveData.difficulty;
    }

    void Update()
    {
        //ゲージの割合でゲージの色を変える
        Color.RGBToHSV(m_EasiestColor, out float startH, out float s, out float v);
        Color.RGBToHSV(m_HardestColor, out float endH, out s, out v);
        float h = woskni.Easing.Linear(m_Slider.value, m_Slider.maxValue, startH, endH);
        m_SliderFillArea.color = Color.HSVToRGB(h, s, v);

        //タッチされてなければ何もしない
        if (!m_TouchFlag) return;

        //ポインター位置取得
        m_PointerPos = woskni.InputManager.GetInputPosition(Application.platform);

        //スライダー位置特定
        float sliderPosY = m_SliderTransform.position.y - (m_Hight * (Screen.height / m_BaseScreenHeight) / 2);
        //タッチ位置との差から割合を算出
        float ratio = (m_PointerPos.y - sliderPosY) / (m_Hight * (Screen.height / m_BaseScreenHeight));

        //割合代入
        m_Slider.value = ratio;
    }

    /// <summary>タッチ押下</summary>
    public void PointerDown()
    {
        m_TouchFlag = true;
    }

    /// <summary>タッチ解放</summary>
    public void PointerUp()
    {
        m_TouchFlag = false;
    }
}
