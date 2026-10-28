using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        SetupResolutions();

        LoadSettings();
    }

    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        SaveSettings(); 
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings(); 
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveSettings(); 
    }


    public void SaveSettings()
    {
        PlayerPrefs.SetInt("SavedResolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("SavedQuality", qualityDropdown.value);
        PlayerPrefs.SetInt("SavedFullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.Save(); 
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SavedResolution"))
        {
            int resIndex = PlayerPrefs.GetInt("SavedResolution");
            resolutionDropdown.value = resIndex;
            SetResolution(resIndex);

            int qualIndex = PlayerPrefs.GetInt("SavedQuality");
            qualityDropdown.value = qualIndex;
            SetQuality(qualIndex);

            bool isFull = PlayerPrefs.GetInt("SavedFullscreen") == 1;
            fullscreenToggle.isOn = isFull;
            SetFullscreen(isFull);
        }
    }
}