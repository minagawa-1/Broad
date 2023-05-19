using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public struct Timer : IEquatable<Timer>
    {
        /// <summary>�^�C�}�[�̑��x�{��</summary>
        public float timeScale { get; set; }

        /// <summary>���݂̌o�ߕb��</summary>
        public float time { get; set; }

        /// <summary>�I������(�b)</summary>
        public float limit { get; set; }

        /// <summary>�I�����Ԃ̏����ݒ�</summary>
        /// <param name="limit">�I������</param>
        public void Setup(float limit) { time = 0.0f; this.limit = limit; timeScale = 1f; }

        /// <summary>�o�ߎ��Ԃ̏�����</summary>
        public void Reset() { time = 0.0f; timeScale = 1f; }

        /// <summary>�^�C�}�[�X�V</summary>
        /// <param name="affectScale">Time.timeScale�̉e�����󂯂邩</param>
        public void Update(bool affectScale = true) => time += affectScale ? Time.deltaTime * timeScale : Time.unscaledDeltaTime * timeScale;

        /// <summary>�^�C�}�[���I��������</summary>
        public void Fin() => time = limit;

        /// <summary>�I�����m</summary>
        public bool IsFinished() => time >= limit;

        /// <summary>�f�o�b�O���O</summary>
        public string DebugLog(bool isOutputLog = true, string name = "")
        {
            if(name == "") name =  ToString();

            string logText = name + " : time: " + time.ToString("F2") + " / limit: " + limit.ToString("F2");

            if (isOutputLog) {
                Debug.Log(logText);
                return string.Empty;
            }

            return logText;
        }

        public float TimeLeft() => limit - time;


        bool IEquatable<Timer>.Equals(Timer other){ return Equals(this, other); }
    }
}