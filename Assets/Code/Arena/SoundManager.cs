using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// AudioSource to play audio.
    /// </summary>
    [SerializeField] private AudioSource _audioSource;

    /// <summary>
    /// Singleton instance of Sound Manager.
    /// </summary>
    public static SoundManager Instance { get; private set; }

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

    private List<AudioClip> _ongoingMetalDetectors = new List<AudioClip>();

    /// <summary>
    /// Plays metal detector noises.
    /// </summary>
    /// <param name="audioClip"> The audio clip to play. </param>
    public async void PlayMetalDetector(AudioClip audioClip)
    {
        // If this audio clip is not in the ongoingMetalDetector list, then it is not already playing.
        if (!_ongoingMetalDetectors.Contains(audioClip))
        {
            // Add it to the list so the audio clip will not overlap with itself.
            _ongoingMetalDetectors.Add(audioClip);

            // Get the duration of the audio clip.
            int audioClipDuration = (int) audioClip.length * 1000;

            // PLAY the audio clip.
            _audioSource.PlayOneShot(audioClip);

            await Task.Delay(audioClipDuration);

            _ongoingMetalDetectors.Remove(audioClip);
        }
    }

    /**
     * Todo:
     * Make the metal detector noises a queue.
     * So, PlayMetalDetector changes to QueueMetalDetector
     * If that clip is in the queue, do nothing
     * Else, Add it to the queue
     * Every frame:
     *      Check if something is currently playing
     *      Check if something is in the queue
     *      Play or don't play
     * 
     */
}
