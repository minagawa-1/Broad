using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DFR : MonoBehaviour
{
    ConfigData lastData;

    [SerializeField] UnityEngine.InputSystem.LowLevel.GamepadButton m_GamepadButton;

    private void Start()
    {
        lastData = Config.data.Copy();
    }

    private void Update()
    {
        ChangedLog();
    }

    void ChangedLog()
    {
        if (Config.data == lastData) return;

        ChangedLog(Config.data.playerName   , lastData.playerName   , "playerName"   );
        ChangedLog(Config.data.autosave     , lastData.autosave     , "autosave"     );

        ChangedLog(Config.data.deadzoneLeft , lastData.deadzoneLeft, "deadzoneLeft"  );
        ChangedLog(Config.data.deadzoneRight, lastData.deadzoneRight, "deadzoneRight");
        ChangedLog(Config.data.moveSpeed    , lastData.moveSpeed    , "moveSpeed"    );
        ChangedLog(Config.data.buttonGuide  , lastData.buttonGuide  , "buttonGuide"  );

        ChangedLog(Config.data.masterVolume , lastData.masterVolume , "masterVolume" );
        ChangedLog(Config.data.bgmVolume    , lastData.bgmVolume    , "bgmVolume"    );
        ChangedLog(Config.data.seVolume     , lastData.seVolume     , "seVolume"     );

        ChangedLog(Config.data.language     , lastData.language     , "language"     );

        ChangedLog(Config.data.brightness   , lastData.brightness   , "brightness"   );
        ChangedLog(Config.data.fontSizeScale, lastData.fontSizeScale, "fontSizeScale");
        ChangedLog(Config.data.screenSize   , lastData.screenSize   , "screenSize"   );

        lastData = Config.data.Copy();
    }

    void ChangedLog(object currentData, object lastData, string dispName)
    {
        if (!currentData.Equals(lastData)) Debug.Log($"{dispName}: {currentData}");
    }
}
