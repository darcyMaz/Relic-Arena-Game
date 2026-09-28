using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BuriedRelic : MonoBehaviour
{
    /// <summary>
    /// Event which fire when a BuriedRelic is unearthed.
    /// </summary>
    public event Action<Vector2> OnBuriedRelicDugUp;

    /// <summary>
    /// Distance at which those with Metal Detectors can pick up BuriedRelics as a percentage of the radius.
    /// </summary>
    [SerializeField] private float _closeDistancePercentage = 0.5f;

    /// <summary>
    /// The calculated close distance.
    /// </summary>
    private float _actualCloseDistance = 1;

    /// <summary>
    /// Relic data associated with this BuriedRelic.
    /// </summary>
    [SerializeField] private RelicSO _relicSO;

    /// <summary>
    /// The arena coordinates where this BuriedRelic is situated.
    /// </summary>
    private Vector2 BuriedRelicCoordinates = new Vector2();

    /// <summary>
    /// The sphere collider component.
    /// </summary>
    private SphereCollider _sphereCollider;

    /// <summary>
    /// Locks the playing of sound from this BuriedRelic.
    /// </summary>
    private bool SoundLock = false;

    /// <summary>
    /// Queue for AudioClips to play based on the presense of Metal Detectors.
    /// </summary>
    private List<AudioClip> _audioClipQueue = new List<AudioClip>();

    private int LockCheck = 0;

    /// <summary>
    /// Method which plays on awake.
    /// </summary>
    private void Awake()
    {
        // Get the sphere collider.
        _sphereCollider = GetComponent<SphereCollider>();

        // Calculate the close distance.
        _actualCloseDistance = _sphereCollider.radius * _closeDistancePercentage;
    }

    private void Update()
    {
        CheckPlayAudio();
    }

    private async void CheckPlayAudio()
    {
        // if the audio queue is not empty and bool is not locked

        if (!SoundLock && _audioClipQueue.Count > 0)
        {
            // Lock the playing of audio.
            SoundLock = true;

            // If more than one async method got through the if statement somehow, then use the numbered lock check to remove all but the first.
            // On further thought, this shouldn't be necessary because this function call will only happen once per frame.
            if (++LockCheck > 1)
            {
                Debug.Log("BuriedRelic.CheckPlayAudio(): A method call got through the LockCheck when it was supposed to be locked.");
                return;
            }

            // Get the AudioClip at the front of the queue.
            AudioClip toPlay = _audioClipQueue[0];
            _audioClipQueue.RemoveAt(0);

            // Play the AudioClup
            SoundManager.Instance.PlayMetalDetector(toPlay);

            // Wait for the AudioClip to complete and add a small delay to buffer the noises.
            await Task.Delay( SecondsToMilliseconds(toPlay.length) + 100);

            // Unlock the playing of audio.
            SoundLock = false;
            LockCheck = 0;

        }
    }

    /// <summary>
    /// Set the relicSO data.
    /// </summary>
    /// <param name="relicSO"> The RelicSO to set. </param>
    public void SetRelicData(RelicSO relicSO)
    {
        _relicSO = relicSO;
    }

    /// <summary>
    /// When something is in this trigger, check if it has a metal detector and play its sound.
    /// For the purpose of this prototype, this functions also check for the player's number.
    /// </summary>
    /// <param name="other"> Collider which is in this trigger. </param>
    private void OnTriggerStay(Collider other)
    {
        // If this component has a Metal Detector.
        MetalDetector metalDetector;
        if (other.TryGetComponent(out metalDetector))
        {
            Debug.Log("---");
            Debug.Log("Distance between BuriedRelic and MetalDetector: " + Vector3.Distance(other.transform.position, transform.position));
            Debug.Log("trigger (player) position: " + other.transform.position + " buried relic: " + transform.position);
            Debug.Log("Radius of sphere: " + _sphereCollider.radius + " Close Distance: " + _actualCloseDistance + " close distance percentage: " + _closeDistancePercentage);

            // If the gameObject is within the "very close range"
            if (Vector3.Distance(other.transform.position, transform.position) <= _actualCloseDistance) // XXXX This is weird
            {
                // Enqueue the current AudioClip is it hasn't been Enqueued already.
                if (!_audioClipQueue.Contains(metalDetector.GetVeryCloseSound()))
                {
                    _audioClipQueue.Add(metalDetector.GetVeryCloseSound());
                }
                // Remove the other sound, if it is in the list right now.
                _audioClipQueue.Remove(metalDetector.GetCloseSound());
                
                // Instantiate the relic object.
                Relic relic = new Relic(_relicSO);

                // Give the Relic to the metal detector, which the player listens to.
                metalDetector.InvokeRelicVeryClose(relic, this);
            }
            // If the gameObject is within the trigger but not the "very close range"
            else
            {
                // Enqueue the current AudioClip is it hasn't been Enqueued already.
                if (!_audioClipQueue.Contains(metalDetector.GetCloseSound()))
                {
                    _audioClipQueue.Add(metalDetector.GetVeryCloseSound());
                }
                // Remove the other sound, if it is in the list right now.
                _audioClipQueue.Remove(metalDetector.GetVeryCloseSound());
            }
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        MetalDetector metalDetector;
        if (other.TryGetComponent(out metalDetector))
        {
            // Remove both sounds, if they are in the list.
            _audioClipQueue.Remove(metalDetector.GetVeryCloseSound());
            _audioClipQueue.Remove(metalDetector.GetCloseSound());
        }
    }
    

    public Vector2 GetArenaCoords()
    {
        return BuriedRelicCoordinates;
    }
    public void SetArenaCoords(Vector2 coords)
    {
        BuriedRelicCoordinates = coords;
    }

    private void OnDestroy()
    {
        OnBuriedRelicDugUp?.Invoke(GetArenaCoords());
    }

    /// <summary>
    /// Method which converts seconds in float to milliseconds in int.
    /// </summary>
    /// <param name="seconds"> Seconds as a float. </param>
    /// <returns> Milliseconds as an int. </returns>
    private int SecondsToMilliseconds(float seconds)
    {
        return (int) (seconds * 1000);
    }
}
