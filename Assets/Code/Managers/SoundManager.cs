using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// Wall rumble
    /// </summary>
    [SerializeField] public AudioClip WallRumble;
    
    /// <summary>
    /// AudioSource to play audio.
    /// </summary>
    [SerializeField] private AudioSource _miscAudioSource;

    /// <summary>
    /// List of all of the audio sources attached to players.
    /// </summary>
    [SerializeField] private List<AudioSource> _playerAudioSources = new List<AudioSource>();

    /// <summary>
    /// Singleton instance of Sound Manager.
    /// </summary>
    public static SoundManager Instance { get; private set; }

    /// <summary>
    /// List of ongoing metal detector noises.
    /// </summary>
    private List<AudioClip> _ongoingMetalDetectors = new List<AudioClip>();


    /// <summary>
    /// Before the game start, initialize the singleton.
    /// </summary>
    private void Awake()
    {
        InitSingleton();
    }

    private void Update()
    {
        //foreach (AudioClip clip in _ongoingMetalDetectors)
        //{
        //    DecrementAudioClipVolume(clip);
        //}
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
    /// If they come from a player it uses the audio sources for those players, otherwise uses the misc source.
    /// </summary>
    /// <param name="audioClip"> The audio clip to play. </param>
    /// <param name="volume"> The volume of the audio source as a float between 0 and 1. </param>
    /// <param name="playerNumber"> The player number as an int. Set to zero if this is not a player. </param>
    public async void PlayMetalDetector(AudioClip audioClip, float volume, int playerNumber)
    {
        // Get the audio source for this play.
        AudioSource currentAudioSource;
        bool usesPlayerAS = false;
        // If the player number is a real player number and the list has an audio source.
        if (playerNumber > 0 && ( (playerNumber-1) < _playerAudioSources.Count))
        {
            currentAudioSource = _playerAudioSources[playerNumber-1];
            usesPlayerAS = true;
        }
        // Otherwise, use the misc audio source.
        else
        {
            currentAudioSource = _miscAudioSource;
        }


        // If this audio clip is not in the ongoingMetalDetector list, then it is not already playing.
        if (!_ongoingMetalDetectors.Contains(audioClip))
        {
            // Reset the audio to 1.
            currentAudioSource.volume = 1f;

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
            currentAudioSource.PlayOneShot(audioClip, volume);

            // Await in milliseconds
            await Task.Delay(audioClipDuration);

            // Remove from list once it's done
            _ongoingMetalDetectors.Remove(audioClip);
        }
        // If the audio clip is already in the list AND this is audio for a player:
        else if (usesPlayerAS)
        {
            // Set the volume of the current audio source based on the distance.
            currentAudioSource.volume = volume;
        }
    }

    public void Play (AudioClip clip, float volume = 1f)
    {
        Debug.Log("Play called with: " + (clip != null ? clip.name : "NULL"), this);

        if (clip != null)
            _miscAudioSource.PlayOneShot(clip,volume);
    }

    private void DecrementAudioClipVolume(AudioClip clip)
    {
        _miscAudioSource.volume -= 0.01f;
    }

}
