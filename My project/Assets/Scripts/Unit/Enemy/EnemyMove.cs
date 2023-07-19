using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("èoåªà íu")]
    [SerializeField] Vector3 m_AppearPosition;

    [Header("èäóvéûä‘")]
    [SerializeField] float        m_AppearTime;

    [SerializeField] Enemy m_Enemy;

    enum State
    {
          Appear
        , Wait
        , Move
        , Dead
    }
    State m_EnemyState;

    woskni.Timer m_Timer;

    Vector3 m_EasingStartPosition;
    Vector3 m_EasingEndPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_EnemyState = State.Appear;

        m_Timer.Setup(m_AppearTime);

        m_EasingStartPosition = transform.position;
        m_EasingEndPosition = m_AppearPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_EnemyState != State.Dead && m_Enemy.deadFlag)
        {
            m_EnemyState = State.Dead;
            m_Timer.Setup(0.5f);
        }

        switch (m_EnemyState)
        {
            case State.Appear: Appear(); break;
            case State.Wait:   Wait();   break;
            case State.Move:   Move();   break;
            case State.Dead:   Dead();   break;
        }
    }

    void Appear()
    {
        m_Timer.Update();

        float time = m_Timer.time;
        float limit = m_Timer.limit;

        transform.position = woskni.Easing.OutCirc(time, limit, m_EasingStartPosition, m_EasingEndPosition);

        if (m_Timer.IsFinished())
        {
            transform.position = m_EasingEndPosition;

            m_EnemyState = State.Wait;

            m_Timer.Setup(TemporarySavingBounty.enemy.moveIntervalRange.Random());
        }
    }

    void Wait()
    {
        m_Timer.Update();

        //çUåÇ
        m_Enemy.Fire();

        if (m_Timer.IsFinished())
        {
            m_EasingStartPosition = transform.position;

            float maxMoveLength = TemporarySavingBounty.enemy.moveLength;

            // à⁄ìÆêÊÇ™íÜêSÇ©ÇÁÇ«ÇÍÇŸÇ«ó£ÇÍÇƒÇ¢ÇÈÇ©
            float x = Random.Range(-maxMoveLength, maxMoveLength);
            float y = Random.Range(-maxMoveLength, maxMoveLength);
            Vector3 posDif = new Vector3(x, y);

            m_EasingEndPosition = m_AppearPosition + posDif;

            m_Timer.Setup(TemporarySavingBounty.enemy.moveTime);

            m_EnemyState = State.Move;
        }

    }

    void Move()
    {
        m_Timer.Update();

        if (!m_Timer.IsFinished()) transform.position = woskni.Easing.OutCirc(m_Timer.time, m_Timer.limit, m_EasingStartPosition, m_EasingEndPosition);

        //çUåÇ
        m_Enemy.Fire();

        if (m_Timer.IsFinished())
        {
            transform.position = m_EasingEndPosition;

            m_EnemyState = State.Wait;

            m_Timer.Setup(TemporarySavingBounty.enemy.moveIntervalRange.Random());
        }
    }

    void Dead()
    {
        // éÄñSèàóù
        if (m_Enemy.deadFlag)
        {
            if (!m_Timer.IsFinished())
            {
                m_Timer.Update();

                float scale = woskni.Easing.InBack(m_Timer.time, m_Timer.limit, 1f, 0f, 3f);

                transform.localScale = new Vector3(scale, scale, scale);
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
}
