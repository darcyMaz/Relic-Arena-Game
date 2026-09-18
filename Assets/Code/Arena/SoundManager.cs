using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
    /// 
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="intensity"></param>
    public async void PlayMetalDetector(AudioClip audioClip)
    {
        // If this audio clip is not in the ongoingMetalDetector list, then it is not already playing.
        if (!_ongoingMetalDetectors.Contains(audioClip))
        {
            // Add it to the list so the audio clip will not overlap with itself.
            _ongoingMetalDetectors.Add(audioClip);

            // Get the duration of the audio clip.
            int audioClipDuration = 0;

            // PLAY the audio clip.
            // Debug.Log("Sound Manager tried to play Metal Detector noises but the Sound Management was not implemented: " + audioClip.name);

            await Task.Delay(audioClipDuration);

            _ongoingMetalDetectors.Remove(audioClip);
        }

    }
}
