using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyFire : MonoBehaviour
{
    const float m_enemy_shake_intensity = 16f;

    [Header("�l")]
    [SerializeField] Color          m_MinColor          = Color.white;  //�Œ�F
    [SerializeField] Color          m_MaxColor          = Color.white;  //�ō��F
    [SerializeField] Transform      m_MinPos            = null;         //�Œ�ʒu
    [SerializeField] Transform      m_MaxPos            = null;         //�ō��ʒu
    [SerializeField] Transform      m_EnemyImage        = null;         //�G�摜

    [Header("�R���|�[�l���g")]
    [SerializeField] Image          m_BackGround        = null;         //�w�i�摜
    [SerializeField] Slider         m_DifficultySlider  = null;         //��Փx�X���C�_�[
    [SerializeField] RectTransform  m_MyTransform       = null;         //���g�̃g�����X�t�H�[��
    [HideInInspector]
    public bool m_IsShake               = false;        //�U���t���O
    Vector3     m_FirstEnemyPosition    = Vector3.zero; //�G�̏����ʒu

    private void Start()
    {
        m_FirstEnemyPosition = m_EnemyImage.localPosition;
    }

    void Update()
    {
        m_MyTransform.position = woskni.Easing.InCubic(m_DifficultySlider.value, m_DifficultySlider.maxValue,
                                                      m_MinPos.position, m_MaxPos.position);

        m_BackGround.color = woskni.Easing.InCubic(m_DifficultySlider.value, m_DifficultySlider.maxValue,
                                                  m_MinColor, m_MaxColor);

        float intensity = woskni.Easing.InQuart(m_DifficultySlider.value, 1f, 0f, m_enemy_shake_intensity);
        Vector3 shake = Random.insideUnitSphere * intensity;
        m_EnemyImage.localPosition = m_FirstEnemyPosition + shake;

    }
}
