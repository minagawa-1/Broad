using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class ShakeCamera : MonoBehaviour
    {
        public enum ShakeType
        {
            /// <summary>�����_���ȕ���</summary>
            Default,

            /// <summary>�p�[�����m�C�Y�ŎZ�o��������</summary>
            //PerlinNoise
        }

        static public bool isShaking;

        static public Vector3 m_ShakeDistance;

        static woskni.Timer m_ShakeTimer;

        static Vector3 m_BasisPosition;

        static float m_Intensity;
        static ShakeType m_ShakeType;
        static bool m_AffectScale;

        private void Update()
        {
            if (!isShaking) return;

            m_ShakeTimer.Update(m_AffectScale);

            if (!m_ShakeTimer.IsFinished())
            {
                Vector3 shakePosition = m_BasisPosition;

                switch (m_ShakeType)
                {
                    case ShakeType.Default: shakePosition = ShakeDefault(m_Intensity); break;
                        //case ShakeType.PerlinNoise: shakePosition = ShakePerlinNoise (intensity); break;
                }

                Camera.main.transform.position = shakePosition;
            }
            else
            {
                isShaking = false;
                Camera.main.transform.position = m_BasisPosition;
            }
        }

        /// <summary>�J�����U��</summary>
        /// <param name="intensity">    �U���̋���(���a)                </param>
        /// <param name="time">         �U������                        </param>
        /// <param name="shakeType">    �U���̎��                      </param>
        /// <param name="affectScale">  Time.timeScale�̉e�����󂯂邩  </param>
        public static void Shake(float intensity, float time = 1f, ShakeType shakeType = ShakeType.Default, bool affectScale = true)
        {
            if (m_ShakeTimer.IsFinished()) m_BasisPosition = Camera.main.transform.position;

            isShaking = true;
            m_ShakeTimer.Setup(time);

            m_Intensity     = intensity;
            m_ShakeType     = shakeType;
            m_AffectScale   = affectScale;
        }

        /// <summary>�U�������ʒu��Ԃ�(�����_������)</summary>
        /// <param name="intensity">�U���̋���(���a)</param>
        static Vector3 ShakeDefault(float intensity)
        {
            intensity *= woskni.Easing.Linear(m_ShakeTimer.time, m_ShakeTimer.limit, 1f, 0f);

            m_ShakeDistance = Random.insideUnitSphere * intensity;

            return m_BasisPosition + m_ShakeDistance;
        }

        /// <summary>�U�������ʒu��Ԃ�(�p�[�����m�C�Y�ŎZ�o��������)</summary>
        /// <param name="intensity">�U���̋���(���a)</param>
        static Vector3 ShakePerlinNoise(float intensity, Vector2 offset)
        {
            float noiseX = 2 * (Mathf.PerlinNoise(offset.x, offset.y) - 0.5f);



            return m_BasisPosition;
        }
    }
}