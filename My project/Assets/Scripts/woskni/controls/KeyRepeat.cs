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

        /// <summary>コンストラクタ</summary>
        /// <param name="key">判定キー</param>
        public KeyRepeat(KeyCode key) => this.key = key;

        /// <summary>キーリピート</summary>
        /// <param name="wait"       > キーリピート開始までの所要時間 </param>
        /// <param name="interval"   > リピート間隔                   </param>
        /// <param name="affectScale"> Time.timeScaleの影響を受けるか </param>
        /// <returns>押された瞬間と引数の一定間隔でtrue, それ以外ではfalse</returns>
        public bool Repeat(float wait, float interval = 0.1f, bool affectScale = true)
        {
            bool flag = false;

            if (Input.GetKey(key))
            {
                // 押された瞬間
                if (Input.GetKeyDown(key))
                {
                    flag = true;
                    m_RepeatTimer.Setup(wait);

                    m_State = State.Wait;
                }
            }
            // 押されていなければリセット
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
            // 押された瞬間
            if (m_RepeatTimer.time == 0f)
            {
                // Updateしてtrueを返す
                m_RepeatTimer.Update(affectScale);
                return true;
            }
            // 押されている途中
            else
            {
                m_RepeatTimer.Update(affectScale);

                // 所要時間に到達
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