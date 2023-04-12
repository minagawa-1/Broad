using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeCycle : MonoBehaviour
{
    /// <summary>�����Ǝ����ɂ�������ʂ̏��</summary>
    [System.Serializable]
    public class TimeInfo
    {
        [Space(16)]
        public string timeName = "";     // ���̎���

        public woskni.Range transRange = new woskni.Range();

        [Header("�C�[�W���O�I������Bloom")]
        [woskni.Name("����")]
        public float intensity = 6f;

        [woskni.Name("�F")]
        public Color color = Color.white;
    }

    [SerializeField, UnityEngine.Min(0f)] float m_BloomRate;

    [Space(32)]
    [SerializeField] List<TimeInfo> m_TimeList;

    [Header("���z�I�u�W�F�N�g")]
    [SerializeField] Light m_Sun          = null;

    [SerializeField, woskni.Name("���z�̉�]�A�j���[�V����")]
    AnimationCurve m_SunRotAnimation = new AnimationCurve();

    [SerializeField, woskni.Name("�w�i�̃}�e���A��")] Material m_Material;

    // PostProcessing��Bloom���
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

    /// <summary>���E���������^�̎����ɕϊ�����</summary>
    /// <returns>���Ƃ��� 17��45�� ������� 17.75f ���Ԃ�</returns>
    float GetFloatHour(System.DateTime time) => time.Hour + (time.Minute / 60f);

    /// <summary>���ݎ����ɑΉ�����m_TimeList�̔ԍ���Ԃ�</summary>
    /// <returns>m_TimeList�̔ԍ��B�@��������Y�����Ȃ����-1��Ԃ�</returns>
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

    /// <summary>�w�i�̌����ڂ��J�ڒ���</summary>
    /// <param name="time">���ݎ���(FloatHour�l)</param>
    bool IsFadingTime(float time)
    {
        for (int i = 0; i < m_TimeList.Count; ++i)
            if (m_TimeList[i].transRange.IsIn(time))
                return true;

        return false;
    }

    /// <summary>�^�C�����X�g�Ǝ�������Bloom�̋������v�Z</summary>
    /// <returns>Bloom�̋���</returns>
    float CalcIntensity()
    {
        int num = GetTimeListNum();
        int last = GetTimeListNum(-1);

        if (num == -1) return m_TimeList[last].intensity;

        float time = m_CurrentTime - m_TimeList[num].transRange.min;
        float limit = m_TimeList[num].transRange.max - m_TimeList[num].transRange.min;

        return woskni.Easing.Linear(time, limit, m_TimeList[last].intensity, m_TimeList[num].intensity);
    }

    /// <summary>�^�C�����X�g�Ǝ�������Bloom�̐F���v�Z</summary>
    /// <returns>Bloom�̐F</returns>
    Color CalcColor()
    {
        int num = GetTimeListNum();
        int last = GetTimeListNum(-1);

        if (num == -1) return m_TimeList[last].color;

        float time = m_CurrentTime - m_TimeList[num].transRange.min;
        float limit = m_TimeList[num].transRange.max - m_TimeList[num].transRange.min;

        return woskni.Easing.Linear(time, limit, m_TimeList[last].color, m_TimeList[num].color);
    }

    /// <summary>�^�C�����X�g�̃C���f�b�N�X���擾</summary>
    /// <param name="dif">���݂̃C���f�b�N�X�Ƃ̍�</param>
    /// <returns>�^�C�����X�g�̓Y��</returns>
    int GetTimeListNum(int dif = 0) => new woskni.RangeInt(0, m_TimeList.Count).GetAround(CalcCurrentTimeNum() + dif);

    /// <summary>PostProcessing.Bloom �̐ݒ�</summary>
    /// <param name="intensity">����</param>
    /// <param name="color">�F</param>
    void SetBloom(float intensity, Color color)
    {
        // Bloom��ݒ肵�āA��������OK�ɂ���
        // (�d�l��A����true�ɐݒ肵�Ȃ��Ǝ��t���[����false�ɂȂ�)
        m_Bloom.enabled.Override(true);

        m_Bloom.intensity.Override(intensity * m_BloomRate);
        m_Bloom.color.Override(color);

        // Bloom�̕ύX��PostProcessing�S�̂ɔ��f������
        PostProcessManager.instance.QuickVolume(gameObject.layer, 1f, m_Bloom);
    }

    /// <summary>���z�̈ʒu�ݒ�</summary>
    /// <param name="y">y���W</param>
    void SetSunRotationY(float y)
        => m_Sun.transform.rotation = Quaternion.Euler(m_Sun.transform.rotation.eulerAngles.x, y, m_Sun.transform.rotation.eulerAngles.z);

    /// <summary>�������̍Čv�Z</summary>
    /// <param name="intensity">����</param>
    /// <param name="color">�F</param>
    void CalcLight(float intensity, Color color)
    {
        // �����̐ݒ�
        //m_Sun.intensity = intensity;

        // �F�̐ݒ�
        m_SkyColor = color;
        m_Sun.color = woskni.Easing.Linear(0.5f, 1f, color, Color.white);
        m_Material.color = woskni.Easing.Linear(0.5f, 1f, color, Color.white);   // ���F�Ƃ̕���
    }
}
