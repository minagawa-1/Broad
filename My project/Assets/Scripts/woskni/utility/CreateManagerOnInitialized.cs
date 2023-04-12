using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace woskni
{
    public class CreateManagerOnInitialized
    {
        // ゲームの起動時に実行
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CreateManager()
        {
            GameObject sceneManager = new GameObject("SceneManager");
            sceneManager.AddComponent<woskni.Scene>();

            GameObject soundManager = new GameObject("SoundManager");
            var sm = soundManager.AddComponent<woskni.SoundManager>();

            sm.m_Sound = Resources.Load<Sound>("SoundList");
            sm.m_MaxSource = 16;
        }
    }
}