using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeCycle : MonoBehaviour
{
    /// <summary>時刻と時刻における効果の情報</summary>
    [System.Serializable]
    public class TimeInfo
    {
        [Space(16)]
        public string timeName = "";     // 何の時間

        public woskni.Range transRange = new woskni.Range();

        [Header("イージング終了時のBloom")]
        [woskni.Name("強さ")]
        public float intensity = 6f;

        [woskni.Name("色")]
        public Color color = Color.white;
    }

    [SerializeField, UnityEngine.Min(0f)] float m_BloomRate;

    [Space(32)]
    [SerializeField] List<TimeInfo> m_TimeList;

    [Header("太陽オブジェクト")]
    [SerializeField] Light m_Sun          = null;

    [SerializeField, woskni.Name("太陽の回転アニメーション")]
    AnimationCurve m_SunRotAnimation = new AnimationCurve();

    [SerializeField, woskni.Name("背景のマテリアル")] Material m_Material;

    // PostProcessingのBloom情報
    Bloom m_Bloom;

    public float m_CurrentTime { get; private set; }

    [SerializeField, Space(32), woskni.ReadOnly] Color m_SkyColor;

    // Start is called before the first frame update
    void Start()
    {
        m_Bloom = ScriptableObject.CreateInstance<Bloom>();

        m_CurrentTime = GetFloatHour(System.DateTime.Now);

        SetBloom(CalcIntensity(), CalcColor());
    }

    // Update is called once per frame
    void Update()
    {
#if false
        m_CurrentTime = GetFloatHour(System.DateTime.Now);
#else
        m_CurrentTime += Time.deltaTime * 2f;
        //m_CurrentTime = 17.5f;
        m_CurrentTime = new woskni.Range(0f, 24f).GetAround(m_CurrentTime);
#endif

        if (IsFadingTime(m_CurrentTime)) Fade(); else Wait();

        CalcLight(1f, m_Bloom.color);

        SetSunRotationY(m_SunRotAnimation.Evaluate(m_CurrentTime / 24f) * -360f + 180f);
    }

    void Wait()
    {

    }

    void Fade()
    {
        SetBloom(CalcIntensity(), CalcColor());
    }

    /// <summary>時・分を小数型の時刻に変換する</summary>
    /// <returns>たとえば 17時45分 を入れると 17.75f が返る</returns>
    float GetFloatHour(System.DateTime time) => time.Hour + (time.Minute / 60f);

    /// <summary>現在時刻に対応したm_TimeListの番号を返す</summary>
    /// <returns>m_TimeListの番号。　いずれも該当しなければ-1を返す</returns>
    int CalcCurrentTimeNum()
    {
        for (int i = 0; i < m_TimeList.Count; ++i)
            if (m_TimeList[i].transRange.IsIn(m_CurrentTime))
                return i;

        for (int i = 0; i < m_TimeList.Count; ++i)
            if (m_CurrentTime < m_TimeList[i].transRange.min)
                return new woskni.RangeInt(0, m_TimeList.Count).GetAround(i - 1);

        return m_TimeList.Count - 1;
    }

    /// <summary>背景の見た目が遷移中か</summary>
    /// <param name="time">現在時間(FloatHour値)</param>
    bool IsFadingTime(float time)
    {
        for (int i = 0; i < m_TimeList.Count; ++i)
            if (m_TimeList[i].transRange.IsIn(time))
                return true;

        return false;
    }

    /// <summary>タイムリストと時刻からBloomの強さを計算</summary>
    /// <returns>Bloomの強さ</returns>
    float CalcIntensity()
    {
        int num = GetTimeListNum();
        int last = GetTimeListNum(-1);

        if (num == -1) return m_TimeList[last].intensity;

        float time = m_CurrentTime - m_TimeList[num].transRange.min;
        float limit = m_TimeList[num].transRange.max - m_TimeList[num].transRange.min;

        return woskni.Easing.Linear(time, limit, m_TimeList[last].intensity, m_TimeList[num].intensity);
    }

    /// <summary>タイムリストと時刻からBloomの色を計算</summary>
    /// <returns>Bloomの色</returns>
    Color CalcColor()
    {
        int num = GetTimeListNum();
        int last = GetTimeListNum(-1);

        if (num == -1) return m_TimeList[last].color;

        float time = m_CurrentTime - m_TimeList[num].transRange.min;
        float limit = m_TimeList[num].transRange.max - m_TimeList[num].transRange.min;

        return woskni.Easing.Linear(time, limit, m_TimeList[last].color, m_TimeList[num].color);
    }

    /// <summary>タイムリストのインデックスを取得</summary>
    /// <param name="dif">現在のインデックスとの差</param>
    /// <returns>タイムリストの添字</returns>
    int GetTimeListNum(int dif = 0) => new woskni.RangeInt(0, m_TimeList.Count).GetAround(CalcCurrentTimeNum() + dif);

    /// <summary>PostProcessing.Bloom の設定</summary>
    /// <param name="intensity">強さ</param>
    /// <param name="color">色</param>
    void SetBloom(float intensity, Color color)
    {
        // Bloomを設定して、書き換えOKにする
        // (仕様上、毎回trueに設定しないと次フレームでfalseになる)
        m_Bloom.enabled.Override(true);

        m_Bloom.intensity.Override(intensity * m_BloomRate);
        m_Bloom.color.Override(color);

        // Bloomの変更をPostProcessing全体に反映させる
        PostProcessManager.instance.QuickVolume(gameObject.layer, 1f, m_Bloom);
    }

    /// <summary>太陽の位置設定</summary>
    /// <param name="y">y座標</param>
    void SetSunRotationY(float y)
        => m_Sun.transform.rotation = Quaternion.Euler(m_Sun.transform.rotation.eulerAngles.x, y, m_Sun.transform.rotation.eulerAngles.z);

    /// <summary>日光情報の再計算</summary>
    /// <param name="intensity">強さ</param>
    /// <param name="color">色</param>
    void CalcLight(float intensity, Color color)
    {
        // 強さの設定
        //m_Sun.intensity = intensity;

        // 色の設定
        m_SkyColor = color;
        m_Sun.color = woskni.Easing.Linear(0.5f, 1f, color, Color.white);
        m_Material.color = woskni.Easing.Linear(0.5f, 1f, color, Color.white);   // 白色との平均
    }
}
