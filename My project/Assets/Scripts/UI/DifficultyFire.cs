using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyFire : MonoBehaviour
{
    const float m_enemy_shake_intensity = 16f;

    [Header("値")]
    [SerializeField] Color          m_MinColor          = Color.white;  //最低色
    [SerializeField] Color          m_MaxColor          = Color.white;  //最高色
    [SerializeField] Transform      m_MinPos            = null;         //最低位置
    [SerializeField] Transform      m_MaxPos            = null;         //最高位置
    [SerializeField] Transform      m_EnemyImage        = null;         //敵画像

    [Header("コンポーネント")]
    [SerializeField] Image          m_BackGround        = null;         //背景画像
    [SerializeField] Slider         m_DifficultySlider  = null;         //難易度スライダー
    [SerializeField] RectTransform  m_MyTransform       = null;         //自身のトランスフォーム
    [HideInInspector]
    public bool m_IsShake               = false;        //振動フラグ
    Vector3     m_FirstEnemyPosition    = Vector3.zero; //敵の初期位置

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
