using UnityEngine;
using System.Collections;

public class Destructibleobject : MonoBehaviour
{
    [SerializeField] private AudioClip _DestructionSound1;
    [SerializeField] private AudioClip _DestructionSound2; 
    [SerializeField] private string _DestructionAnimation;
    [SerializeField] private float strength = 1.5f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] public int HitPoints = 3;
    private bool isShaking = false;

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
                TakeHit();
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
            // If so, register taking the hit
            TakeHit();
           
        }
    }
    //on a coroutine, before the next frame, the function will register the current position and then within a radius
    // shake according to a moddable strength and then after returning will wait a frame to return to its current position
    private IEnumerator ShakeNBake(){
       
        isShaking = true;
        Vector3 currentPosition = transform.position;
        float timeTaken = 0f;
        
        while (timeTaken < duration){

        Vector2 Shakin = Random.insideUnitCircle * strength;
        transform.position = currentPosition + new Vector3(Shakin.x, Shakin.y, 0f);
        timeTaken += Time.deltaTime;
        yield return null;
        }
        transform.position = currentPosition;
        isShaking = false;
    }

    private void TakeHit(){
         HitPoints -= 1;
            if (HitPoints == 0)
        DestroySelf();
        else if (!isShaking){
            StartCoroutine(ShakeNBake());
        }
    }
}

