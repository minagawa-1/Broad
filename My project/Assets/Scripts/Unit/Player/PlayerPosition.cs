using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    [SerializeField] Player m_Player;

    Vector3 m_DiffLength = Vector3.zero;   // 位置とマウス座標の差分

    woskni.Timer m_DeadTimer;

    /// <summary>イージング開始時の拡大率</summary>
    Vector3 m_EasingStartScale;

    private void Start()
    {
        m_EasingStartScale = transform.localScale;

        m_DeadTimer.Setup(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Player.deadFlag) InputMove(Application.platform);
        else                    Dead();
    }

    void InputMove(RuntimePlatform platform)
    {
        Vector3 inputPosition = woskni.InputManager.GetInputPosition(platform);

        // 押された瞬間に差分を設定
        if (woskni.InputManager.IsButtonDown(Application.platform))
            m_DiffLength = transform.position - Camera.main.ScreenToWorldPoint(inputPosition);

        // 押している間は移動処理
        if (woskni.InputManager.IsButton(Application.platform))
            transform.position = GetInputPosition(inputPosition, GetComponent<SpriteRenderer>().sprite.rect);
    }

    Vector3 GetInputPosition(Vector3 inputPosition, Rect playerRect)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(inputPosition) + m_DiffLength;

        Vector2 screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        pos.x = Mathf.Max(-screen.x, Mathf.Min(pos.x, screen.x));
        pos.y = Mathf.Max(-screen.y, Mathf.Min(pos.y, screen.y));
        pos.z = 0f;

        return pos;
    }

    void Dead()
    {
        if (!m_DeadTimer.IsFinished())
        {
            m_DeadTimer.Update();

            Vector3 scale = woskni.Easing.InBack(m_DeadTimer.time, m_DeadTimer.limit, m_EasingStartScale, Vector3.zero, 8f);

            transform.localScale = scale;
        }
        else
        {
            if (transform.localScale != Vector3.zero) {
                transform.localScale = Vector3.zero;
                woskni.ShakeCamera.Shake(1f, 1f);
            }
        }
    }
}
