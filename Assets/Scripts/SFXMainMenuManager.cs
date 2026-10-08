using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip buttonClickSound;

    public void PlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}