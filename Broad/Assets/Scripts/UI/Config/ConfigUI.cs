using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigUI : MonoBehaviour
{
    [Chapter("コンポーネント")]
    [SerializeField] InputField m_PlayerName    = null;
    [SerializeField] Slider     m_Autosave      = null;
    [SerializeField] Slider     m_DeadzoneLeft  = null;
    [SerializeField] Slider     m_DeadzoneRight = null;
    [SerializeField] Dropdown   m_MoveSpeed     = null;
    [SerializeField] Slider     m_ButtonGuide   = null;
    [SerializeField] Slider     m_MasterVolume  = null;
    [SerializeField] Slider     m_BGMVolume     = null;
    [SerializeField] Slider     m_SEVolume      = null;
    [SerializeField] Dropdown   m_Language      = null;
    [SerializeField] Slider     m_Brightness    = null;
    [SerializeField] Dropdown   m_FontSize      = null;
    [SerializeField] Dropdown   m_ScreenSize    = null;

    [Chapter("設定範囲")]
    [SerializeField] woskni.Range m_MoveSpeedRange  = new woskni.Range(0.5f , 1.5f );
    [SerializeField] woskni.Range m_FontSizeRange   = new woskni.Range(0.75f, 1.25f);


    Dictionary<int, SystemLanguage> m_Value2Language = new Dictionary<int, SystemLanguage>() {
        { 0, SystemLanguage.English           },
        { 1, SystemLanguage.Japanese          },
        { 2, SystemLanguage.ChineseSimplified }
    };

    Dictionary<SystemLanguage, int> m_Language2Value = new Dictionary<SystemLanguage, int>() {
        { SystemLanguage.English          , 0 },
        { SystemLanguage.Japanese         , 1 },
        { SystemLanguage.ChineseSimplified, 2 }
    };

    Dictionary<int, Vector2Int> m_Value2ScreenSize = new Dictionary<int, Vector2Int>() {
        { 0, new Vector2Int(1280, 720)  },
        { 1, new Vector2Int(1920, 1080) }
    };

    Dictionary<Vector2Int, int> m_ScreenSize2Value = new Dictionary<Vector2Int, int>() {
        { new Vector2Int(1280, 720) , 0 },
        { new Vector2Int(1920, 1080), 1 }
    };

    public void Start()
    {
        m_PlayerName.text     = Config.data.playerName;
        m_Autosave.value      = Config.data.autosave.ToInt();

        m_DeadzoneLeft.value  = Config.data.deadzoneLeft * 100f;
        m_DeadzoneRight.value = Config.data.deadzoneLeft * 100f;
        m_MoveSpeed.value     = m_MoveSpeed.Value2Number(Config.data.moveSpeed, m_MoveSpeedRange.min, m_MoveSpeedRange.max);
        m_ButtonGuide.value   = Config.data.buttonGuide.ToInt();
        
        m_MasterVolume.value  = Config.data.masterVolume * 100f;
        m_BGMVolume.value     = Config.data.bgmVolume    * 100f;
        m_SEVolume.value      = Config.data.seVolume     * 100f;

        m_Language.value      = m_Language2Value[Config.data.language];

        m_Brightness.value    = Config.data.brightness   * 100f;
        m_FontSize.value      = m_FontSize.Value2Number(Config.data.fontSizeScale, m_FontSizeRange.min, m_FontSizeRange.max);

        m_ScreenSize.value    = m_ScreenSize2Value[Config.data.screenSize];
    }

    /// <summary>処理した後、コンフィグのデータを保存する</summary>
    /// <param name="action">処理内容</param>
    public void SetData(System.Action action) {
        action.Invoke();
        if (Config.data.autosave) Config.Save();
    }

    /// <summary>プレイヤー名の設定</summary>
    /// <param name="field">名前入力欄</param>
    public void SetPlayerName()
    {
        SetData(() => Config.data.playerName = m_PlayerName.text);

        // 入力した名前をGameSettingのplayerNames[0]に保存
        GameSetting.instance.playerNames[0] = Config.data.playerName;
    }

    /// <summary>自動セーブの設定</summary>
    /// <param name="slider">スライダー式のトグル</param>
    public void SetAutosave()       => SetData(() => Config.data.autosave       = m_Autosave.value > 0.5f);

    /// <summary>デッドゾーン（左スティック）の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetDeadzoneLeft()   => SetData(() => Config.data.deadzoneLeft   = (float)m_DeadzoneLeft.value / 100f);

    /// <summary>デッドゾーン（右スティック）の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetDeadzoneRight()  => SetData(() => Config.data.deadzoneRight  = (float)m_DeadzoneRight.value / 100f);

    /// <summary>移動速度の設定</summary>
    /// <param name="dropdown">ドロップダウン</param>
    public void SetMoveSpeed()      => SetData(() => Config.data.moveSpeed 
                                                        = m_MoveSpeed.Number2Value(m_MoveSpeedRange.min, m_MoveSpeedRange.max));

    /// <summary>ボタンガイドの表示の設定</summary>
    /// <param name="slider">スライダー式のトグル</param>
    public void SetButtonGuide() {
        SetData(() => Config.data.buttonGuide = m_ButtonGuide.value > 0.5f);

        // 全てのボタンガイドの表示を操作する
        var guides = FindObjectsOfType<ButtonGuideActivate>(true);

        foreach (var guide in guides) guide.SetActive();
    }

    /// <summary>マスター音量の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetMasterVolume()   => SetData(() => Config.data.masterVolume   = (float)m_MasterVolume.value / 100f);

    /// <summary>BGM音量の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetBGMVolume()      => SetData(() => Config.data.bgmVolume      = (float)m_BGMVolume.value / 100f);

    /// <summary>SE音量の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetSEVolume()       => SetData(() => Config.data.seVolume       = (float)m_SEVolume.value / 100f);

    /// <summary>言語の設定</summary>
    /// <param name="dropdown">ドロップダウン</param>
    public void SetLanguage() {
        SetData(() => Config.data.language = m_Value2Language[m_Language.value]);
        Localization.Correct();
    }

    /// <summary>明度の設定</summary>
    /// <param name="slider">スライダー</param>
    public void SetBrightness() {
        SetData(() => Config.data.brightness = (float)m_Brightness.value / 100f);
        Transition.instance.SetMaxBrightness();
    }

    /// <summary>フォントサイズの設定</summary>
    /// <param name="dropdown">ドロップダウン</param>
    public void SetFontSize() {
        SetData(() => Config.data.fontSizeScale = m_FontSize.Number2Value(m_FontSizeRange.min, m_FontSizeRange.max));
        Localization.Correct();
    }

    /// <summary>画面サイズの設定</summary>
    /// <param name="dropdown">ドロップダウン</param>
    public void SetScreenSize() {
        SetData(() => Config.data.screenSize = m_Value2ScreenSize[m_ScreenSize.value]);

        bool fullScreen = m_ScreenSize.value < 1;
        Screen.SetResolution(Config.data.screenSize.x, Config.data.screenSize.y, fullScreen);
    }

    /// <summary>コンフィグのリセット</summary>
    public void ResetConfig() => Config.Reset();

    /// <summary>セーブデータの初期化</summary>
    public void ResetSaveData() => SaveSystem.Reset();
}
