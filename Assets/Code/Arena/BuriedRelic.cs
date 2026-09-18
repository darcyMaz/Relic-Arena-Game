using UnityEngine;

public class BuriedRelic : MonoBehaviour
{
    /// <summary>
    /// Distance at which those with Metal Detectors can pick up BuriedRelics.
    /// </summary>
    private static float _closeDistance;
    
    /// <summary>
    /// Ensures the initialization of this distance only happens once.
    /// </summary>
    private bool DistanceSetBool = false;

    private void Awake()
    {
        if (!DistanceSetBool) 
        {
            InitDistance();
            DistanceSetBool = true;
        }
    }

    /// <summary>
    /// Set the distance where the BuriedRelics beep vigorously and can be picked up.
    /// </summary>
    private void InitDistance()
    {
        _closeDistance = 2;
    }

    /// <summary>
    /// When something is in this trigger, check if it has a metal detector and play its sound.
    /// For the purpose of this prototype, this functions also check for the player's number.
    /// </summary>
    /// <param name="other"> Collider which is in this trigger. </param>
    private void OnTriggerStay(Collider other)
    {
        // if other has a metal detector
        //  if distance is near: soundmanager.NearRelic(sound)
        //  if distance is right under: soundManager.OverRelic(sound)

        // If this component has a Metal Detector.
        MetalDetector metalDetector;
        if (other.TryGetComponent(out metalDetector))
        {
            
        }
    }
}
