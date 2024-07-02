using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsSaveAndLoad : MonoBehaviour
{
    //public static OptionsSave LoadedSave { get; private set; }

    OptionsSave Save { get => OptionsSave.Save; }
    [SerializeField] Slider _masterVolume;
    [SerializeField] Slider _musicVolume;
    [SerializeField] Slider _sfxVolume;
    [SerializeField] Toggle _showTooltips;
    [SerializeField] Toggle _groundClutter;

    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        LoadOptions();
    }

    void LoadOptions()
    {
        OptionsSave.LoadData();
        ApplySettings();
    }

    public void ShowOptionsData()
    {
        _masterVolume.value = Save.MasterVolume;
        _musicVolume.value = Save.MusicVolume;
        _sfxVolume.value = Save.SFXVolume;
        _showTooltips.isOn = Save.ShowTooltips;
        _groundClutter.isOn = Save.ShowGroundClutter;
    }

    public void SaveOptions()
    {
        Save.MasterVolume = _masterVolume.value;
        Save.MusicVolume = _musicVolume.value;
        Save.SFXVolume = _sfxVolume.value;
        Save.ShowTooltips = _showTooltips.isOn;
        Save.ShowGroundClutter = _groundClutter.isOn;

        OptionsSave.SaveData();
        ApplySettings();
    }

    public void ApplySettings()
    {
        ////Gameplay
        //TipsPopUp.ShowToolTips = Save.ShowTooltips;

        //Graphics
        SetQualityLevel(Save.GraphicsQuality);

        //Audio
        mixer.SetFloat("MasterVolume", Save.MasterVolume);
        mixer.SetFloat("MusicVolume", Save.MusicVolume);
        mixer.SetFloat("SFXVolume", Save.SFXVolume);
    }

    public void SetQualityLevel(int dropdownValue)
    {
        int qualityLevel = 0;
        switch (dropdownValue)
        {
            case 0:
                qualityLevel = 1;
                break;
            case 1:
                qualityLevel = 3;
                break;
            case 2:
                qualityLevel = 5;
                break;
            default:
                break;
        }

        QualitySettings.SetQualityLevel(qualityLevel);
    }
}

public class OptionsSave : SaveableData
{
    public static OptionsSave Save
    {
        get
        {
            if (save == null)
                LoadData();
            return save;
        }
        private set { save = value; }
    }

    static OptionsSave save;
    static string FileName { get => "options.json"; }

    // Graphics
    public int GraphicsQuality = 3;
    public bool ShowGroundClutter = true;

    // Audio
    public float MasterVolume = 0;
    public float MusicVolume = 0;
    public float SFXVolume = 0;

    // Gameplay
    public bool ShowTooltips = true;

    public static void SaveData()
    {
        Save.WriteToFile(FileName);
    }

    public static void LoadData()
    {
        OptionsSave loadedData = ReadFromFile<OptionsSave>(FileName);
        if (loadedData == null)
        {
            loadedData = new OptionsSave();
        }

        Save = loadedData;
        SaveData();
    }
}