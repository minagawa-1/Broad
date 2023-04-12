using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�G�ɒǏ]����UI�̃N���X
public class EnemyPosition : MonoBehaviour
{
    [SerializeField] RectTransform m_UIRectTransform = null;   //�Ǐ]������I�u�W�F�N�g
    [SerializeField] Transform     m_FollowObject     = null; //�G(�Ǐ]�Ώ�)

    void Update()
    {
        // �ʒu�̒Ǐ]
        Vector3 position = m_UIRectTransform.position;
        position.x = Camera.main.WorldToScreenPoint(m_FollowObject.transform.position).x;
        m_UIRectTransform.position = position;

        // �g�嗦�̒���(�����蔻��ɍ��킹��)
        Vector2 enemySize = m_FollowObject.GetComponent<SpriteRenderer>().bounds.size;
        float enemyRadius = Mathf.Max(enemySize.x, enemySize.y);

        m_UIRectTransform.sizeDelta
            = new Vector2(enemyRadius * 200f - m_UIRectTransform.sizeDelta.y, m_UIRectTransform.sizeDelta.y);
    }
}
