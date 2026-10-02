using UnityEngine;
using System.Collections;

public class Destructibleobject : MonoBehaviour
{
    [SerializeField] private AudioClip _DestructionSound1;
    [SerializeField] private AudioClip _DestructionSound2; 
    [SerializeField] private string _DestructionAnimation;
    [SerializeField] private float strength = 0.3f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] public int HitPoints = 3;
    private bool isShaking = false;

    private Collider Collider;


    /// <summary>
    /// Method called on awake.
    /// </summary>
    private void Awake()
    {
        Collider = GetComponent<Collider>();
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
        //AudioClip clip = Random.value < 0.5f ? _DestructionSound1 : _DestructionSound2;
        //if (clip != null){}        
                TakeHit();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void LightningStrikeCheck(Vector3 lightningPosition)
    {
        // Is the position where lightning was struck within the collider?
        if ( Collider.bounds.Contains(lightningPosition) )
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
// when this function is called reduce 1 hp and destroy self if 0, or start coroutine of the shake. Play sound from sound manager
    private void TakeHit(){
                SoundManager.Instance.Play(SoundManager.Instance.WallRumble);

         HitPoints -= 1;
            if (HitPoints == 0)
        DestroySelf();
        else if (!isShaking){
            StartCoroutine(ShakeNBake());
        }
    }
}

