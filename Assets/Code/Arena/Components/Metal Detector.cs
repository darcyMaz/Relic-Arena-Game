using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private AudioClip _nearSound;
    [SerializeField] private AudioClip _veryCloseSound;

    public AudioClip GetNearSound()
    {
        return _nearSound;
    }
    public AudioClip GetVeryCloseSound()
    {
        return _veryCloseSound;
    }
}
