using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{
    /// <summary>
    /// Reference to the camera that this sprite rotates towards.
    /// </summary>
    [SerializeField] private Camera _camera;

    /// <summary>
    /// At the first frame, inform the dev that the camera is not assigned.
    /// </summary>
    private void Start()
    {
        if (!CameraCheck())
        {
            Debug.Log("A SpriteFaceCamera component does not have a Camera assigned to it. It will not rotate to face the camera: " + name);
        }
    }

    /// <summary>
    /// Every frame, call RotateSprite()
    /// </summary>
    private void Update()
    {
        if (CameraCheck()) RotateSprite();
    }

    /// <summary>
    /// Set this gameObject's transform.forward such that it faces the camera.
    /// To be precise, build the directional vector from the this object to the camera, then normalize it, reverse it, and set this transform's forward to it.
    /// </summary>
    private void RotateSprite()
    {
        // The directional vector from this to the camera. Normalize it as the magnitude is not relevant.
        Vector3 thisToCamDirection = (_camera.transform.position - transform.position).normalized;
        // Set the transform.forward to be opposite the directional vector. Otherwise, the sprite is the wrong way.
        transform.forward = -thisToCamDirection;
    }

    // Check if the camera exists.
    private bool CameraCheck()
    {
        return _camera != null;
    }
}
