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

    /// <summary>
    /// Initialize the singleton, checking if it needs to be removed.
    /// </summary>
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
    public void PlayMetalDetector(AudioClip audioClip)
    {
        // Get the duration of the audio clip.
        // int audioClipDuration = (int) audioClip.length * 1000;

        // PLAY the audio clip.
        _audioSource.PlayOneShot(audioClip);
    }
}
