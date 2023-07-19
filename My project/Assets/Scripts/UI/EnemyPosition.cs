using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//敵に追従するUIのクラス
public class EnemyPosition : MonoBehaviour
{
    [SerializeField] RectTransform m_UIRectTransform = null;   //追従させるオブジェクト
    [SerializeField] Transform     m_FollowObject     = null; //敵(追従対象)

    void Update()
    {
        // 位置の追従
        Vector3 position = m_UIRectTransform.position;
        position.x = Camera.main.WorldToScreenPoint(m_FollowObject.transform.position).x;
        m_UIRectTransform.position = position;

        // 拡大率の調整(当たり判定に合わせる)
        Vector2 enemySize = m_FollowObject.GetComponent<SpriteRenderer>().bounds.size;
        float enemyRadius = Mathf.Max(enemySize.x, enemySize.y);

        m_UIRectTransform.sizeDelta
            = new Vector2(enemyRadius * 200f - m_UIRectTransform.sizeDelta.y, m_UIRectTransform.sizeDelta.y);
    }
}
