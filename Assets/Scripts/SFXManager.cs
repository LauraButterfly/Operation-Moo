using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip failSound;
    public AudioClip abductionSound;

    public AudioClip buttonClickSound;

    public void PlayFailSound()
    {
        audioSource.PlayOneShot(failSound);
    }

    public void PlayAbductionSound()
    {
        audioSource.PlayOneShot(abductionSound);
    }

    public void StopAbductionSound()
    {
        audioSource.Stop();
    }

    public void PlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}