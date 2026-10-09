using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Controls the audio settings menu.
// The Music and SFX sliders change the volume of their
// corresponding groups in the Unity Audio Mixer.
public class SettingsMenu : MonoBehaviour
{
    // Audio Mixer containing the Music and SFX volume groups.
    public AudioMixer audioMixer;

    // Sliders used to control music and sound effect volume.
    public Slider musicSlider;
    public Slider sfxSlider;

    // UI panels used to switch between the Main Menu
    // and Settings screen.
    public GameObject settingsPanel;
    public GameObject mainButtons;

    void Start()
    {
        // Start both volume sliders at full volume.
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        // Apply the starting volume values to the Audio Mixer.
        SetMusicVolume(1f);
        SetSFXVolume(1f);
    }

    // Changes the volume of the Music Audio Mixer group.
    public void SetMusicVolume(float volume)
    {
        // The sliders use values from 0 to 1, but the
        // Audio Mixer uses decibels.
        // A value of 0 is converted to -80 dB, which acts as mute.
        float dB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        // Apply the converted value to the exposed
        // Music volume parameter in the Audio Mixer.
        audioMixer.SetFloat("MusicVolume", dB);
    }

    // Changes the volume of the SFX Audio Mixer group.
    public void SetSFXVolume(float volume)
    {
        // Convert the slider's 0-1 value into decibels.
        // A value of 0 is treated as muted.
        float dB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        // Apply the converted value to the exposed
        // SFX volume parameter in the Audio Mixer.
        audioMixer.SetFloat("SFXVolume", dB);
    }

    // Closes the Settings panel and returns
    // to the normal Main Menu buttons.
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}