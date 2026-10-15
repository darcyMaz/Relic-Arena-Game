using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioClip WallRumble;
    /// <summary>
    /// AudioSource to play audio.
    /// </summary>
    [SerializeField] private AudioSource _audioSource;

    /// <summary>
    /// Singleton instance of Sound Manager.
    /// </summary>
    public static SoundManager Instance { get; private set; }

    private List<AudioClip> _ongoingMetalDetectors = new List<AudioClip>();


    /// <summary>
    /// Before the game start, initialize the singleton.
    /// </summary>
    private void Awake()
    {
        InitSingleton();
    }

    private void InitSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// <summary>
    /// Plays metal detector noises.
    /// </summary>
    /// <param name="audioClip"> The audio clip to play. </param>
    public async void PlayMetalDetector(AudioClip audioClip, float volume)
    {
        // Debug.Log("Play metal detector: " + audioClip);

        // If this audio clip is not in the ongoingMetalDetector list, then it is not already playing.
        if (!_ongoingMetalDetectors.Contains(audioClip))
        {
            // Add it to the list so the audio clip will not overlap with itself.
            _ongoingMetalDetectors.Add(audioClip);

            // Get the duration of the audio clip.
            int audioClipDuration = (int) (audioClip.length * 1000);

            // AudioClip class has floats in seconds for the duration: ex. 0.945
            // Async stuff (see line 63) uses int for durations in milliseconds: ex. 945
            // Not a problem if you convert properly.
            // 0.945 * 1000 = 945 -> float to int
            // .945 float to int -> 0 *1000 = 0 PROBLEM

            // PLAY the audio clip.
            _audioSource.PlayOneShot(audioClip, volume);

            // Await in milliseconds
            await Task.Delay(audioClipDuration);

            // Remove from list once it's done
            _ongoingMetalDetectors.Remove(audioClip);
        }
    }

    public void Play (AudioClip clip, float volume = 1f)
    {
        Debug.Log("Play called with: " + (clip != null ? clip.name : "NULL"), this);

        if (clip != null)
            _audioSource.PlayOneShot(clip,volume);
    }

}
