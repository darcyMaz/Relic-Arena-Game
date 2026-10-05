using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectsManager : MonoBehaviour
{
    /// <summary>
    /// The EffectsManager is a singleton, and thus has a single instance.
    /// </summary>
    public static EffectsManager Instance { get; private set; }

    /// <summary>
    /// Gameobject representing a source for lightning strikes. Holds a Lightning component.
    /// </summary>
    [SerializeField] private GameObject LightningSourcePrefab;

    /// <summary>
    /// A GameObject that can be cloned and accessed representing the display before launching a scarab attack.
    /// </summary>
    [SerializeField] private GameObject ScarabDisplay;

    /// <summary>
    /// A GameObject that can be cloned and accessed for the Fog Effect.
    /// </summary>
    [SerializeField] private GameObject Fog;

    /// <summary>
    /// A GameObject that indicates how soon a lightning strike is going to hit you.
    /// </summary>
    [SerializeField] private GameObject LightningCounter;

    /// <summary>
    /// A GameObject that shows the spot where lightning will strike.
    /// </summary>
    [SerializeField] private GameObject LightningMarker;

    // NOTE: The next four lightning vars should NOT be in EffectsManager, and should instead be passed through the Effect.

    /// <summary>
    /// A float representing the maximum variation in position that each component of the LightningMarker's position could have.
    /// </summary>
    [SerializeField] private float LightningMarkerMaxVariation = 0.6f;

    /// <summary>
    /// An integer representing the amount of time in milliseconds that the lighting forray effect's lightning is delayed before striking.
    /// </summary>
    [SerializeField] private int LightningMarkerDelay = 2000;

    /// <summary>
    /// A float representing the radius of the lightning marker's hit range.
    /// </summary>
    [SerializeField] private float LightningMarkerRadius = 2f;

    /// <summary>
    /// An int representing the delay between lightning markers spawning in for the LightningForray Effect.
    /// </summary>
    [SerializeField] private int NextLightningMarkerDelay = 1000;

    /// <summary>
    /// Event invoked whenever a lightning strikes the scene.
    /// </summary>
    public event Action<Vector3> OnLightningStrike;

    /// <summary>
    /// Event invoked whenever an active Effect is launched at another player.
    /// </summary>
    public event Action<Effect, string, int, int> OnActiveEffectLaunched;

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
    /// Method which calls the OnActiveEffectLaunched event.
    /// All EffectableBase objects will listen to that event.
    /// </summary>
    /// <param name="activeEffect"> The active Effect being launched. </param>
    /// <param name="playerSource"> The source of the event, the player number as an int. </param>
    /// <param name="playerTarget"> The target of the event, the player number as an int.</param>
    public void LaunchActiveEffect(Effect activeEffect, string effectDetails, int playerSource, int playerTarget)
    {
        // invoke a func which informs everyone about the effect being applied
        OnActiveEffectLaunched?.Invoke(activeEffect, effectDetails, playerSource, playerTarget);
    }

    /// <summary>
    /// Strikes the endPos with lightning. The Lightning component will be called and its line renderer will send a lightning-looking line to hit the endPos.
    /// </summary>
    /// <param name="endPos"> The destination of the line renderer. </param>
    public void Lightning(Vector3 endPos, int lifetime, float kinkDistance, float kinkExageration)
    {
        // The cloned lightning source.
        GameObject lightningClone = Instantiate(LightningSourcePrefab, transform);

        // Change its X position to match that of the end of the strike.
        lightningClone.transform.position = transform.position + new Vector3(endPos.x, 0, 0);

        // Get the lightning component from this clone.
        Lightning lightningComponent;
        if (lightningClone.TryGetComponent(out lightningComponent))
        {
            // Strike lightning!
            lightningComponent.LightningStrike(endPos, lifetime, kinkDistance, kinkExageration);
            OnLightningStrike?.Invoke(endPos);
        }
        else
        {
            Debug.Log("Lightning strike was not performed by the EffectsManager because the prefab did not contain the Lightning component.");
        }
    }
    
    /// <summary>
    /// Returns a GameObject representing the display aid for the scarab active Effect.
    /// </summary>
    /// <param name="initialPosition"> The position this gameobject will start. </param>
    /// <returns> The scarab aid GameObject to display. </returns>
    public GameObject GetDisplayScarab(Vector3 initialPosition)
    {
        if (ScarabDisplay == null) return null;
        return Instantiate(ScarabDisplay, initialPosition, Quaternion.identity);
    }

    /// <summary>
    /// Method which returns the Fog gameobject.
    /// </summary>
    /// <param name="parent"> The transform to attach the fog to. </param>
    /// <returns> The Fog gameObject. </returns>
    public GameObject GetFog(Transform parent)
    {
        return Instantiate(Fog, parent);
    }

    /// <summary>
    /// Method which returns the GameObject which indicates how soon lightning will strike the player.
    /// </summary>
    /// <param name="initialPosition"> The initial position of the gameObject. </param>
    /// <returns> The Lightning Counter GameObject </returns>
    public GameObject GetLightningCounter(Vector3 initialPosition)
    {
        if (LightningCounter == null) throw new NotImplementedException();
        return Instantiate(LightningCounter, initialPosition, Quaternion.identity);
    }

    /// <summary>
    /// Method which returns the GameObject that is a marker for where lightning will strike.
    /// </summary>
    /// <param name="position"> The position of the marker. </param>
    /// <returns> The Lightning Marker GameObject. </returns>
    public GameObject GetLightningMarker(Vector3 position)
    {
        return Instantiate(LightningMarker, position, Quaternion.identity);
    }
    
    /// <summary>
    /// Method which returns the maximum variation in the lightning marker's position from the EffectableBase being struck.
    /// </summary>
    /// <returns> The variation as a float in seconds. </returns>
    public float GetLightningMarkerMaxVariation()
    {
        return LightningMarkerMaxVariation;
    }

    /// <summary>
    /// Method which returns the delay in milliseconds of a lightning strike in the LightningForray Effect.
    /// </summary>
    /// <returns> An integer representing the delay in milliseconds. </returns>
    public int GetLightningMarkerDelay()
    {
        return LightningMarkerDelay;
    }

    /// <summary>
    /// Method which returns the radius of the hit area for the LightningForray Effect's lightning.
    /// </summary>
    /// <returns> A float representing the radius of the hitzone. </returns>
    public float GetLightningMarkerRadius()
    {
        return LightningMarkerRadius;
    }

    /// <summary>
    /// Method which returns the delay between lightning markers in the LightningForray Effect.
    /// </summary>
    /// <returns> An int representing the delay in milliseconds. </returns>
    public int GetNextLightningMarkerDelay()
    {
        return NextLightningMarkerDelay;
    }
}
