using UnityEngine;

public class HomingUI : MonoBehaviour
{
    [SerializeField] Transform m_EnemyTransform  = null;
    [SerializeField] RectTransform m_UITransform = null;

    private void Start()
    {
        Vector2 size = m_EnemyTransform.GetComponent<SpriteRenderer>().sprite.rect.size;

        // scaleを加味する
        size *= m_EnemyTransform.localScale;

        // ｘとｙの大きい辺の正方形サイズにする
        size = new Vector2(Mathf.Max(size.x, size.y), Mathf.Max(size.x, size.y));

        m_UITransform.sizeDelta = size * 2.5f;
    }

    void Update() => m_UITransform.position = Camera.main.WorldToScreenPoint(m_EnemyTransform.position);
}
