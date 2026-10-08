using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    public GameObject settingsPanel;
    public GameObject mainButtons;

    void Start()
    {
        // Always start both sliders at full volume.
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        SetMusicVolume(1f);
        SetSFXVolume(1f);
    }

    public void SetMusicVolume(float volume)
    {
        // Convert slider value (0-1) into decibels.
        float dB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat("MusicVolume", dB);
    }

    public void SetSFXVolume(float volume)
    {
        // Convert slider value (0-1) into decibels.
        float dB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat("SFXVolume", dB);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}