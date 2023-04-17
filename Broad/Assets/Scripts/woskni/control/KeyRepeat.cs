using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class KeyRepeat
    {
        public KeyCode key { get; }

        Timer m_RepeatTimer;

        enum State
        {
              None
            , Wait
            , Repeating
        }
        State m_State;

        /// <summary>�R���X�g���N�^</summary>
        /// <param name="key">����L�[</param>
        public KeyRepeat(KeyCode key) => this.key = key;

        /// <summary>�L�[���s�[�g</summary>
        /// <param name="wait"       > �L�[���s�[�g�J�n�܂ł̏��v���� </param>
        /// <param name="interval"   > ���s�[�g�Ԋu                   </param>
        /// <param name="affectScale"> Time.timeScale�̉e�����󂯂邩 </param>
        /// <returns>�����ꂽ�u�Ԃƈ����̈��Ԋu��true, ����ȊO�ł�false</returns>
        public bool Repeat(float wait, float interval = 0.1f, bool affectScale = true)
        {
            bool flag = false;

            if (Input.GetKey(key))
            {
                // �����ꂽ�u��
                if (Input.GetKeyDown(key))
                {
                    flag = true;
                    m_RepeatTimer.Setup(wait);

                    m_State = State.Wait;
                }
            }
            // ������Ă��Ȃ���΃��Z�b�g
            else
                m_State = State.None;

            switch (m_State)
            {
                case State.None:        flag = None     (                     ); break;
                case State.Wait:        flag = Wait     (interval, affectScale); break;
                case State.Repeating:   flag = Repeating(          affectScale); break;
            }

            return flag;
        }

        bool None()
        {
            if (m_RepeatTimer.time > 0f) m_RepeatTimer.Reset();

            return false;
        }

        bool Wait(float interval, bool affectScale)
        {
            // �����ꂽ�u��
            if (m_RepeatTimer.time == 0f)
            {
                // Update����true��Ԃ�
                m_RepeatTimer.Update(affectScale);
                return true;
            }
            // ������Ă���r��
            else
            {
                m_RepeatTimer.Update(affectScale);

                // ���v���Ԃɓ��B
                if (m_RepeatTimer.IsFinished())
                {
                    m_State = State.Repeating;
                    m_RepeatTimer.Setup(interval);

                    return true;
                }
            }

            return false;
        }

        bool Repeating(bool affectScale)
        {
            m_RepeatTimer.Update(affectScale);

            if (m_RepeatTimer.IsFinished())
            {
                m_RepeatTimer.Reset();

                return true;
            }

            return false;
        }
    }
}