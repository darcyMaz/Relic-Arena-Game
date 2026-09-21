using System;
using UnityEngine;

public class BuriedRelic : MonoBehaviour
{
    /// <summary>
    /// Event which fire when a BuriedRelic is unearthed.
    /// </summary>
    public event Action<Vector2> OnBuriedRelicDugUp;

    /// <summary>
    /// Distance at which those with Metal Detectors can pick up BuriedRelics.
    /// I'd like to find a way to set static variable in the inspector.
    /// </summary>
    private static float _closeDistance = 0.25f;

    /// <summary>
    /// Relic data associated with this BuriedRelic.
    /// </summary>
    [SerializeField] private RelicSO _relicSO;

    /// <summary>
    /// The arena coordinates where this BuriedRelic is situated.
    /// </summary>
    private Vector2 BuriedRelicCoordinates = new Vector2();

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
            // If the gameObject is within the "very close range"
            if (Vector3.Distance(other.transform.position, transform.position) <= _closeDistance)
            {
                // Tell the SoundManager to play the Very Close Sound.
                SoundManager.Instance.PlayMetalDetector(metalDetector.GetVeryCloseSound());

                // Instantiate the relic object.
                Relic relic = new Relic(_relicSO);

                // Give the Relic to the metal detector, which the player listens to.
                metalDetector.InvokeRelicVeryClose(relic, this);
            }
            // If the gameObject is within the trigger but not the "very close range"
            else
            {
                // Tell the SoundManager to play the Near Sound.
                SoundManager.Instance.PlayMetalDetector(metalDetector.GetCloseSound());
            }
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
}
