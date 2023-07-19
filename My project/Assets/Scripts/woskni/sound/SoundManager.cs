using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class SoundManager : MonoBehaviour
    {
        // サウンド情報
        public Sound m_Sound = null;

        public int m_MaxSource = 10;

        static Dictionary<string, Sound.Audio> m_SoundDictionary = new Dictionary<string, Sound.Audio>();

        // ボリューム範囲
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
            // 再生プレイヤー数だけ生成する
            soundPlayers = new SoundPlayer[Mathf.Max(0, m_MaxSource)];

            //AudioSourceを生成して配列に格納
            for (int i = 0; i < soundPlayers.Length; ++i) AddSoundSource(i);

            // シーンをまたいでも消えないようにする
            DontDestroyOnLoad(gameObject);

            SetDictionary();
        }

        void AddSoundSource(int num)
        {
            soundPlayers[num] = new SoundPlayer();

            // SoundPlayer用の子オブジェクトを作成
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

                // イージング
                {
                    var sp = soundPlayers[i];
                    float vol = woskni.Easing.Linear(sp.changeTimer.time, sp.changeTimer.limit, sp.beforeVolume, sp.afterVolume);
                    soundPlayers[i].source.volume = vol;
                }

                if (soundPlayers[i].changeTimer.IsFinished())
                {
                    soundPlayers[i].source.volume = soundPlayers[i].afterVolume;

                    // 音量が0なら停止
                    if (soundPlayers[i].source.volume <= 0f)
                        soundPlayers[i].source.Stop();

                    soundPlayers[i].changeFlag = false;
                }
            }
        }

        /// <summary>未再生のプレイヤーを返す</summary>
        /// <returns>未再生のプレイヤー (見つからなければnull)</returns>
        private static SoundPlayer GetUnusePlayer() => soundPlayers.FirstOrDefault(sp => !sp.source.isPlaying);

        /// <summary>サウンドプレイヤーを検索</summary>
        /// <param name="clip">検索するクリップ</param>
        /// <returns>見つかったサウンドプレイヤー or null</returns>
        public static SoundPlayer FindSoundPlayer(AudioClip clip)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
                if (soundPlayers[i].audio.clip == clip)
                    return soundPlayers[i];

            return null;
        }

        /// <summary>サウンドプレイヤーを検索</summary>
        /// <param name="clip">検索するクリップ</param>
        /// <returns>見つかったサウンドプレイヤー or null</returns>
        public static SoundPlayer FindSoundPlayer(string soundName)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
                if (soundPlayers[i].audio.name == soundName)
                    return soundPlayers[i];

            return null;
        }

        /// <summary>サウンド再生</summary>
        /// <param name="clip">再生するサウンド</param>
        /// <param name="volume">再生音量(0.0 to 1.0)</param>
        public static void Play(AudioClip clip, Sound.Category category, bool loop = true, float volume = Sound.default_volume, float pitch = 1f, float playTime = 0.0f)
        {
            SoundPlayer sp = GetUnusePlayer();

            // プレイヤーがすべて再生中なら警告を出してreturn
            if (sp == null)
            {
                Debug.LogWarning("プレイヤーに空きがありません");
                return;
            }

            sp.source.clip = clip;
            sp.source.volume = m_VolumeRange.GetIn(volume);
            sp.source.pitch = pitch;
            sp.source.loop = loop;
            sp.source.time = playTime;
            sp.source.Play();
        }

        /// <summary>サウンド再生</summary>
        /// <param name="soundName">サウンドリストに登録されたサウンド名</param>
        /// <param name="volume">再生音量(0.0 to 1.0) : 指定無しなら規定音量で再生</param>
        public static void Play(string soundName, bool loop = true, float volume = Sound.default_volume, float pitch = 1f, float playTime = 0.0f)
        {
            SoundPlayer sp = GetUnusePlayer();

            // プレイヤーがすべて再生中なら警告を出してreturn
            if (sp == null)
            {
                Debug.LogWarning("プレイヤーに空きがありません");
                return;
            }

            // 管理用Dictionary から、別名で探索
            // 見つかったらdataに格納されているので、それを主関数で行う
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
            {
                sp.audio = data;
                Play(data.clip, data.category, loop, volume, pitch, playTime);
            }
            else
                Debug.LogWarning($"{soundName}: 登録されていないサウンド名です");
        }

        /// <summary>音量変更</summary>
        /// <param name="clip">変更するサウンド</param>
        /// <param name="volume">変更後の音量(0.0 to 1.0)</param>
        /// <param name="changeTime">変更に要する時間</param>
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

        /// <summary>音量変更</summary>
        /// <param name="clip">変更するサウンド</param>
        /// <param name="volume">変更後の音量(0.0 to 1.0)</param>
        /// <param name="changeTime">変更に要する時間</param>
        public static void ChangeVolume(AudioClip clip, float changeVolume, float changeTime = 0f)
        {
            for (int i = 0; i < soundPlayers.Length; ++i)
            {
                // 再生サウンドが違えばcontinue
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

        /// <summary>BGM名・SE名からAudioを取得</summary>
        /// <param name="soundName">BGM名・SE名</param>
        public static Sound.Audio GetAudio(string soundName)
        {
            // 見つかった音源を返す
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
                return data;

            // 見つからなかったらnullを返す
            return null;
        }

        /// <summary>音量変更</summary>
        /// <param name="soundName">変更するサウンド</param>
        /// <param name="volume">変更後の音量(0.0 to 1.0)</param>
        /// <param name="changeTime">変更に要する時間</param>
        public static void ChangeVolume(string soundName, float volume, float changeTime = 0f)
        {
            // 管理用Dictionary から、別名で探索
            // 見つかったらdataに格納されているので、それを主関数で行う
            if (m_SoundDictionary.TryGetValue(soundName, out var data))
                ChangeVolume(data.clip, volume, changeTime);
            else
                Debug.LogWarning(soundName + ": 登録されていないサウンド名です");
        }

        private void SetDictionary()
        {
            // m_SoundDictionaryにセット
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