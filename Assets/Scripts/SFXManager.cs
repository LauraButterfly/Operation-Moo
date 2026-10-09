using UnityEngine;

// Stores and plays the game's sound effects.
// Other scripts can call these public methods whenever
// a specific gameplay or UI sound needs to be played.
public class SFXManager : MonoBehaviour
{
    // AudioSource used to play all assigned sound effects.
    public AudioSource audioSource;

    // Sound played when the player fails an abduction attempt.
    public AudioClip failSound;

    // Sound used for the tractor beam / successful abduction.
    public AudioClip abductionSound;

    // Sound played when a UI button is clicked.
    public AudioClip buttonClickSound;

    // Warning sound played during the final seconds of the timer.
    public AudioClip timerWarningSound;

    // Plays the sound for a failed abduction.
    public void PlayFailSound()
    {
        audioSource.PlayOneShot(failSound);
    }

    // Plays the abduction sound.
    public void PlayAbductionSound()
    {
        audioSource.PlayOneShot(abductionSound);
    }

    // Stops the sound currently playing through this AudioSource.
    // This is used when the abduction sound needs to end early.
    public void StopAbductionSound()
    {
        audioSource.Stop();
    }

    // Plays the UI button click sound.
    public void PlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }

    // Plays the warning sound used during
    // the final seconds of the game timer.
    public void PlayTimerWarningSound()
    {
        audioSource.PlayOneShot(timerWarningSound);
    }
}