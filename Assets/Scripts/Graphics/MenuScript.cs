using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Slider Slider;
    public Dropdown Dropdown;
    public Text DropdownLabel;
    
    public void Play(int level)
    {
        Constants.Endless = false;
        switch (level)
        {
            case 0:
                Constants.LevelKey = Constants.L0Key;
                Constants.Endless = true;
                break;
            case 1:
                Constants.LevelKey = Constants.L1Key;
                Constants.Level = ShiftLevelToRange(LevelManager.Twinkle);
                break;
            case 2:
                Constants.LevelKey = Constants.L2Key;
                Constants.Level = ShiftLevelToRange(LevelManager.HeyMister);
                break;
            case 3:
                Constants.LevelKey = Constants.L3Key;
                Constants.Level = ShiftLevelToRange(LevelManager.Shindlers);
                break;
            case 4:
                Constants.LevelKey = Constants.L4Key;
                Constants.Level = ShiftLevelToRange(LevelManager.Challenge);
                break;
            case 5:
                Constants.LevelKey = Constants.L5Key;
                Constants.Level = LevelManager.GenerateLevel(16);
                break;
        }
        Initiate.Fade("Main", Color.black, 3);
    }

    public void OnVolumeChanged()
    {
        AudioListener.volume = Slider.value;
    }

    public void OnVoiceRangeChanged()
    {
        int voiceRange = Dropdown.value;
        SetVoiceRange(voiceRange);
    }

    public void OnApplySettings()
    {
        PlayerPrefs.SetFloat(Constants.VolumeKey, Slider.value);
        PlayerPrefs.SetInt(Constants.VoiceRangeKey, Dropdown.value);
    }

    int[] ShiftLevelToRange(int[] level)
    {
        if (Dropdown.value == 1) return level;

        int[] shifted = new int[level.Length];
        for (int i = 0; i < level.Length; i++)
        {
            shifted[i] = level[i] + (Dropdown.value < 1 ? -12 : 12);
        }
        return shifted;
    }

    void SetVoiceRange(int voiceRange)
    {
        switch (voiceRange)
        {
            case 0: // low
                Constants.MinNote = 36; // C2
                Constants.MaxNote = 60; // C4
                break;
            case 1: // medium
                Constants.MinNote = 48; // C3
                Constants.MaxNote = 72; // C5
                break;
            case 2: // high
                Constants.MinNote = 60; // C4
                Constants.MaxNote = 84; // C6
                break;
        }
    }

    void Awake()
    {
        bool oldVersion = true;
        if (PlayerPrefs.HasKey(Constants.VersionKey))
        {
            float savedVersion = PlayerPrefs.GetFloat(Constants.VersionKey);
            if (savedVersion >= Constants.Version)
            {
                oldVersion = false;
            }
        }

        if (oldVersion)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetFloat(Constants.VersionKey, Constants.Version);
        }

        float startingVolume = 1f;
        if (PlayerPrefs.HasKey(Constants.VolumeKey))
        {
            startingVolume = PlayerPrefs.GetFloat(Constants.VolumeKey);
        }

        Slider.value = startingVolume;
        AudioListener.volume = startingVolume;

        int voiceRange = 1;
        if (PlayerPrefs.HasKey(Constants.VoiceRangeKey))
        {
            voiceRange = PlayerPrefs.GetInt(Constants.VoiceRangeKey);
        }

        Dropdown.value = voiceRange;
        SetVoiceRange(voiceRange);

        switch (voiceRange)
        {
            case 0:
                DropdownLabel.text = "Low (C2-C4)";
                break;
            case 1:
                DropdownLabel.text = "Medium (C3-C5)";
                break;
            case 2:
                DropdownLabel.text = "High (C4-C6)";
                break;
        }
    }
}
