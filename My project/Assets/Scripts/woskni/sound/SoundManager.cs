using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class SoundManager : MonoBehaviour
    {
        // �T�E���h���
        public Sound m_Sound = null;

        public int m_MaxSource = 10;

        static Dictionary<string, Sound.Audio> m_SoundDictionary = new Dictionary<string, Sound.Audio>();

        // �{�����[���͈�
        static woskni.Range m_VolumeRange = new woskni.Range(0f, 1f);



        public class SoundPlayer
        {
            public Sound.Audio audio = null;

            public AudioSource source = null;

            public woskni.Timer changeTimer = new woskni.Timer();
            public bool changeFlag = false;

            public float beforeVolume = 0f;
            public float afterVolume  = 0f;
        }
        public static SoundPlayer[] soundPlayers;



        private void Awake()
        {
            // �Đ��v���C���[��������������
            soundPlayers = new SoundPlayer[Mathf.Max(0, m_MaxSource)];

            //AudioSource�𐶐����Ĕz��Ɋi�[
            for (int i = 0; i < soundPlayers.Length; ++i) AddSoundSource(i);

            // �V�[�����܂����ł������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);

            SetDictionary();
        }

        void AddSoundSource(int num)
        {
            soundPlayers[num] = new SoundPlayer();

            // SoundPlayer�p�̎q�I�u�W�F�N�g���쐬
            GameObject obj = new GameObject("SoundPlayer[" + transform.childCount + "]", typeof(AudioSource));
            obj.transform.parent = transform;

            soundPlayers[num].source             = obj.GetComponent<AudioSource>();
            soundPlayers[num].source.playOnAwake = false;
            soundPlayers[num].source.volume      = Sound.default_volume;
        }

        private void Update()
        {
            for (int i = 0; i < m_MaxSource; ++i)
            {
                if (!soundPlayers[i].source.isPlaying)
                    soundPlayers[i].source.clip = null;

                if (!soundPlayers[i].changeFlag) continue;

                soundPlayers[i].changeTimer.Update(false);

                // �C�[�W���O
                {
                    var sp = soundPlayers[i];
                    float vol = woskni.Easing.Linear(sp.changeTimer.time, sp.changeTimer.limit, sp.beforeVolume, sp.afterVolume);
                    soundPlayers[i].source.volume = vol;
                }

                if (soundPlayers[i].changeTimer.IsFinished())
                {
                    soundPlayers[i].source.volume = soundPlayers[i].afterVolume;

                    // ���ʂ�0�Ȃ��~
                    if (soundPlayers[i].source.volume <= 0f)
                        soundPlayers[i].source.Stop();

                    soundPlayers[i].changeFlag = false;
                }
            }
        }

        /// <summary>���Đ��̃v���C���[��Ԃ�</summary>
        /// <returns>���Đ��̃v���C���[ (������Ȃ����null)</returns>
        private static SoundPlayer GetUnusePlayer() => soundPlayers.FirstOrDefault(sp => !sp.source.isPlaying);

        /// <summary>�T�E���h�v���C���[������</summary>
        /// <param name="clip">��������N���b�v</param>
        /// <returns>���������T�E���h�v���C���[ or null</returns>
        public static SoundPlayer FindSoundPlayer(AudioClip clip)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
                if (soundPlayers[i].audio.clip == clip)
                    return soundPlayers[i];

            return null;
        }

        /// <summary>�T�E���h�v���C���[������</summary>
        /// <param name="clip">��������N���b�v</param>
        /// <returns>���������T�E���h�v���C���[ or null</returns>
        public static SoundPlayer FindSoundPlayer(string soundName)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
                if (soundPlayers[i].audio.name == soundName)
                    return soundPlayers[i];

            return null;
        }

        /// <summary>�T�E���h�Đ�</summary>
        /// <param name="clip">�Đ�����T�E���h</param>
        /// <param name="volume">�Đ�����(0.0 to 1.0)</param>
        public static void Play(AudioClip clip, Sound.Category category, bool loop = true, float volume = Sound.default_volume, float pitch = 1f, float playTime = 0.0f)
        {
            SoundPlayer sp = GetUnusePlayer();

            // �v���C���[�����ׂčĐ����Ȃ�x�����o����return
            if (sp == null)
            {
                Debug.LogWarning("�v���C���[�ɋ󂫂�����܂���");
                return;
            }

            sp.source.clip = clip;
            sp.source.volume = m_VolumeRange.GetIn(volume);
            sp.source.pitch = pitch;
            sp.source.loop = loop;
            sp.source.time = playTime;
            sp.source.Play();
        }

        /// <summary>�T�E���h�Đ�</summary>
        /// <param name="soundName">�T�E���h���X�g�ɓo�^���ꂽ�T�E���h��</param>
        /// <param name="volume">�Đ�����(0.0 to 1.0) : �w�薳���Ȃ�K�艹�ʂōĐ�</param>
        public static void Play(string soundName, bool loop = true, float volume = Sound.default_volume, float pitch = 1f, float playTime = 0.0f)
        {
            SoundPlayer sp = GetUnusePlayer();

            // �v���C���[�����ׂčĐ����Ȃ�x�����o����return
            if (sp == null)
            {
                Debug.LogWarning("�v���C���[�ɋ󂫂�����܂���");
                return;
            }

            // �Ǘ��pDictionary ����A�ʖ��ŒT��
            // ����������data�Ɋi�[����Ă���̂ŁA�������֐��ōs��
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
            {
                sp.audio = data;
                Play(data.clip, data.category, loop, volume, pitch, playTime);
            }
            else
                Debug.LogWarning($"{soundName}: �o�^����Ă��Ȃ��T�E���h���ł�");
        }

        /// <summary>���ʕύX</summary>
        /// <param name="clip">�ύX����T�E���h</param>
        /// <param name="volume">�ύX��̉���(0.0 to 1.0)</param>
        /// <param name="changeTime">�ύX�ɗv���鎞��</param>
        public static void ChangeVolume(int index, float changeVolume, float changeTime = 0f)
        {
           soundPlayers[index].changeFlag = true;

           soundPlayers[index].beforeVolume = soundPlayers[index].source.volume;
           soundPlayers[index].afterVolume = m_VolumeRange.GetIn(changeVolume);

           soundPlayers[index].changeTimer.Setup(changeTime);
        }

        public static List<Sound.Audio> GetAudios(Sound.Category category)
        {
            List<Sound.Audio> temp = new List<Sound.Audio>();

            foreach (SoundPlayer soundPlayer in soundPlayers)
            {
                if (soundPlayer.audio == null) continue;

                if (soundPlayer.audio.category == category)
                    temp.Add(soundPlayer.audio);
            }

            return temp;
        }

        /// <summary>���ʕύX</summary>
        /// <param name="clip">�ύX����T�E���h</param>
        /// <param name="volume">�ύX��̉���(0.0 to 1.0)</param>
        /// <param name="changeTime">�ύX�ɗv���鎞��</param>
        public static void ChangeVolume(AudioClip clip, float changeVolume, float changeTime = 0f)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
            {
                // �Đ��T�E���h���Ⴆ��continue
                if (soundPlayers[i].source.clip != clip) continue;

                soundPlayers[i].changeFlag = true;

                soundPlayers[i].beforeVolume = soundPlayers[i].source.volume;
                soundPlayers[i].afterVolume = m_VolumeRange.GetIn(changeVolume);

                soundPlayers[i].changeTimer.Setup(changeTime);
            }
        }

        public static void StopAll()
        {
            foreach(SoundPlayer player in soundPlayers)
                player.source.Stop();
        }

        /// <summary>BGM���ESE������Audio���擾</summary>
        /// <param name="soundName">BGM���ESE��</param>
        public static Sound.Audio GetAudio(string soundName)
        {
            // ��������������Ԃ�
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
                return data;

            // ������Ȃ�������null��Ԃ�
            return null;
        }

        /// <summary>���ʕύX</summary>
        /// <param name="soundName">�ύX����T�E���h</param>
        /// <param name="volume">�ύX��̉���(0.0 to 1.0)</param>
        /// <param name="changeTime">�ύX�ɗv���鎞��</param>
        public static void ChangeVolume(string soundName, float volume, float changeTime = 0f)
        {
            // �Ǘ��pDictionary ����A�ʖ��ŒT��
            // ����������data�Ɋi�[����Ă���̂ŁA�������֐��ōs��
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
                ChangeVolume(data.clip, volume, changeTime);
            else
                Debug.LogWarning(soundName + ": �o�^����Ă��Ȃ��T�E���h���ł�");
        }

        private void SetDictionary()
        {
            // m_SoundDictionary�ɃZ�b�g
            foreach (var audio in m_Sound.m_BGMList) AddDictionary(audio, Sound.Category.BGM);
            foreach (var audio in m_Sound.m_SEList) AddDictionary(audio, Sound.Category.SE);

            void AddDictionary(Sound.Audio audio, Sound.Category category)
            {
                audio.category = category;
                m_SoundDictionary.Add(audio.name, audio);
            }
        }
    }
}