using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MetalDetector : MonoBehaviour
{
    /// <summary>
    /// Event called when BuriedRelic is very close.
    /// </summary>
    public event Action<Relic, BuriedRelic> OnRelicVeryClose;

    /// <summary>
    /// Close AudioClip.
    /// </summary>
    [SerializeField] private AudioClip _closeSound;

    /// <summary>
    /// Very close AudioClip.
    /// </summary>
    [SerializeField] private AudioClip _veryCloseSound;

    /// <summary>
    /// Get the sound for when the metal detector is close to a buried relic.
    /// </summary>
    /// <returns> The audio clip for the close sound. </returns>
    public AudioClip GetCloseSound()
    {
        return _closeSound;
    }

    /// <summary>
    /// Get the sound for when the metal detector is very close to a buried relic.
    /// </summary>
    /// <returns> The audio clip for the very close sound. </returns>
    public AudioClip GetVeryCloseSound()
    {
        return _veryCloseSound;
    }

    /// <summary>
    /// Invoke the event which announces when a relic is very close.
    /// </summary>
    /// <param name="relic"> Relic to store. </param>
    /// <param name="buriedRelic"> Buried relic that Metal Detector is close to. </param>
    public void InvokeRelicVeryClose(Relic relic, BuriedRelic buriedRelic)
    {
        OnRelicVeryClose?.Invoke(relic, buriedRelic);
    }
}
