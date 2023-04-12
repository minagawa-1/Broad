using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] Slider m_BGMSlider;
    [SerializeField] Slider m_SESlider;
    [SerializeField] Title  m_Title = null;

    // Start is called before the first frame update
    void Start()
    {
        m_BGMSlider.value = SaveSystem.m_SaveData.BGMvolume;
        m_SESlider .value = SaveSystem.m_SaveData.SEvolume;
    }

    public void ChangeBGMVolume()
    {
        SaveSystem.m_SaveData.BGMvolume = m_BGMSlider.value;

        SaveVolume();

        // BGM‚ðŽæ“¾
        var bgms = woskni.SoundManager.GetAudios(woskni.Sound.Category.BGM);

        if (bgms.Count == 0) return;

        for (int i = 0; i < bgms.Count; ++i)
            woskni.SoundManager.ChangeVolume(bgms[i].name, m_BGMSlider.value);
    }

    public void ChangeSEVolume()
    {
        SaveSystem.m_SaveData.SEvolume = m_SESlider.value;
        woskni.SoundManager.Play("eŒ‚", false, m_SESlider.value);

        SaveVolume();
    }

    public void ResetVolume()
    {
        m_BGMSlider.value = SaveData.m_initial_volume;
        m_SESlider.value  = SaveData.m_initial_volume;
        SaveVolume();
    }

    public void ScreenReturn()
    {
        m_Title.UndoWindowState();
    }

    private void SaveVolume()
    {
        if (woskni.InputManager.IsButtonUp())
            SaveSystem.Save();
    }
}
