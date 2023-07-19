using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    [CreateAssetMenu(menuName = "Scriptable/Create sound data")]
    public class Sound : ScriptableObject
    {
        public const float default_volume   = 0.75f;

        public enum Category
        {
              BGM
            , SE
        }

        [System.Serializable]
        public class Audio
        {
            public string name;

            [HideInInspector] public Category category;

            [woskni.Name("サウンド")] public AudioClip clip;
        }

        [Header("サウンドリスト")]
        public List<Audio> m_BGMList;
        public List<Audio> m_SEList;

        //public static float CalcVolume(float volume, Category category) => volume * (category == Category.BGM ? bgmVolume : seVolume);
    }
}