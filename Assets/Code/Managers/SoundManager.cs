using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class which manages the sound.
/// </summary>
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
    [SerializeField] private List<AudioSource> PlayerAudioSources = new List<AudioSource>();

    /// <summary>
    /// List of queues. One queue for each player. Has the potential audio clips to play. The closest is played on late update.
    /// </summary>
    private List<Queue<AudioClip>> _playerQueueList = new List<Queue<AudioClip>>();

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

    /// <summary>
    /// Initializes the singleton for the SoundManager.
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
        if (playerNumber > 0 && ( (playerNumber-1) < PlayerAudioSources.Count))
        {
            currentAudioSource = PlayerAudioSources[playerNumber-1];
            usesPlayerAS = true;
        }
        // Otherwise, use the misc audio source.
        else
        {
            currentAudioSource = _miscAudioSource;
        }

        if (playerNumber == 0)
        {

        }
        else
        {
            // If the current player's priority queue does not contain this audio clip.
            if (!_playerQueueList[playerNumber - 1].Contains(audioClip))
            {

            }
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

    /// <summary>
    /// Plays a sound in the misc audio source.
    /// </summary>
    /// <param name="clip"> The audio clip to play </param>
    /// <param name="volume"> The volume of the audio clip as a float from 0 to 1 </param>
    public void Play(AudioClip clip, float volume = 1f)
    {
        // Debug.Log("Play called with: " + (clip != null ? clip.name : "NULL"), this);

        if (clip != null)
        {
            _miscAudioSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// A priority queue for the audio clips based on volume.
    /// </summary>
    protected class PriorityQueue
    {
        // List of audio clips.
        private List<ClipVolPair> _audioClips;

        // Constructor initializes the queue.
        public PriorityQueue()
        {
            _audioClips = new List<ClipVolPair>();
        }

        // Adds the clip to the list based on the volume, prioritizing higher volumes.
        public void Add(AudioClip clip, float volume)
        {
            int index = 0;
            ClipVolPair pair = new ClipVolPair();

            for (; index<_audioClips.Count; index++)
            {

            }
        }

        // Clear the queue.
        public void Clear() 
        {
            _audioClips.Clear();
        }

        // Helper struct to track clip and volume at the same time.
        public struct ClipVolPair
        {
            // Constructor for the struct.
            public ClipVolPair(AudioClip clip, float volume)
            {
                Clip = clip;
                Volume = volume;
            }

            // Audio clip.
            public AudioClip Clip;
            // Volume as float from 0 to 1.
            public float Volume;
        }
    }
}
