using UnityEngine;

public class HomingUI : MonoBehaviour
{
    [SerializeField] Transform m_EnemyTransform  = null;
    [SerializeField] RectTransform m_UITransform = null;

    private void Start()
    {
        Vector2 size = m_EnemyTransform.GetComponent<SpriteRenderer>().sprite.rect.size;

        // scale����������
        size *= m_EnemyTransform.localScale;

        // ���Ƃ��̑傫���ӂ̐����`�T�C�Y�ɂ���
        size = new Vector2(Mathf.Max(size.x, size.y), Mathf.Max(size.x, size.y));

        m_UITransform.sizeDelta = size * 2.5f;
    }

    void Update() => m_UITransform.position = Camera.main.WorldToScreenPoint(m_EnemyTransform.position);
}
