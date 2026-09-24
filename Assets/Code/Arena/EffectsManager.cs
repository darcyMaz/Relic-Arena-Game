using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    /// <summary>
    /// The EffectsManager is a singleton, and thus has a single instance.
    /// </summary>
    public static EffectsManager Instance { get; private set; }

    /// <summary>
    /// Source of lightning that the EM will be able to use to strike positions.
    /// </summary>
    [SerializeField] private Lightning LightningSource;

    /// <summary>
    /// A GameObject that can be cloned and accessed representing the display before launching a scarab attack.
    /// </summary>
    [SerializeField] private GameObject ScarabDisplay;

    /// <summary>
    /// A GameObject that can be cloned and accessed for the Fog Effect.
    /// </summary>
    [SerializeField] private GameObject Fog;

    [SerializeField] private GameObject LightningCounter;

    /// <summary>
    /// The Awake function checks if the current object is the singleton.
    /// </summary>
    private void Awake()
    {
        SingletonCheck();
    }

    /// <summary>
    /// Check whether this is the singleton.
    /// </summary>
    private void SingletonCheck()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Strikes the endPos with lightning. The Lightning component will be called and its line renderer will send a lightning-looking line to hit the endPos.
    /// </summary>
    /// <param name="endPos"> The destination of the line renderer. </param>
    public void Lightning(Vector3 endPos)
    {
        LightningSource.LightningStrike(endPos);
    }
    
    /// <summary>
    /// Returns a GameObject representing the display aid for the scarab active Effect.
    /// </summary>
    /// <param name="initialPosition"> The position this gameobject will start. </param>
    /// <returns> The scarab aid GameObject to display. </returns>
    public GameObject GetDisplayScarab(Vector3 initialPosition)
    {
        return Instantiate(ScarabDisplay, initialPosition, Quaternion.identity);
    }

    public GameObject GetFog(Transform parent)
    {
        return Instantiate(Fog, parent);
    }

    public GameObject GetLightningCounter(Vector3 initialPosition)
    {
        return Instantiate(LightningCounter, initialPosition, Quaternion.identity);
    }
    
}
