using UnityEngine;

public class Destructibleobject : MonoBehaviour
{
    [SerializeField] private AudioClip _DestructionSound1;
    [SerializeField] private AudioClip _DestructionSound2; 
    [SerializeField] private string _DestructionAnimation;

    private BoxCollider _boxCollider;


    /// <summary>
    /// Method called on awake.
    /// </summary>
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Method called when this gameObject is enabled.
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to the Lightning strike event.
        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.OnLightningStrike += LightningStrikeCheck;
        }
    }
    /// <summary>
    /// Method called when this gameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from the lightning strike event.
        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.OnLightningStrike -= LightningStrikeCheck;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            return;
        }
    
        DestroySelf();
    }

    public void DestroySelf()
    {
        AudioClip clip = Random.value < 0.5f ? _DestructionSound1 : _DestructionSound2;
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        Destroy(gameObject);
    }

    private void LightningStrikeCheck(Vector3 lightningPosition)
    {
        // Is the position where lightning was struck within the collider?
        if ( _boxCollider.bounds.Contains(lightningPosition) )
        {
            // If so, break this object.
            DestroySelf();
        }
    }
}

