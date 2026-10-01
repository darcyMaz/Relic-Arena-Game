using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Relic", menuName = "Scriptable Objects/Relic")]
public class RelicSO : ItemSO
{
    /// <summary>
    /// The price of this relic.
    /// </summary>
    [SerializeField] private float Price;

    /// <summary>
    /// The active effect attached to this relic.
    /// </summary>
    [SerializeField] private Effect ActiveEffect;

    /// <summary>
    /// The details of the active effect.
    /// </summary>
    [SerializeField] private string ActiveEffectDetails;

    /// <summary>
    /// The LaunchType of the active Effect.
    /// </summary>
    [SerializeField] private LaunchType LaunchType;

    /// <summary>
    /// Passive Effects on this relic.
    /// </summary>
    [SerializeField] private List<Effect> PassiveEffects = new List<Effect>();

    /// <summary>
    /// Details of the passive effects on this relic.
    /// </summary>
    [SerializeField] private List<string> PassiveEffectsDetails = new List<string>();

    /// <summary>
    /// Method which gets the price of this relic.
    /// </summary>
    /// <returns> The price of this relic as a float. </returns>
    public float GetPrice()
    {
        return Price;
    }

    /// <summary>
    /// Method which returns the active Effect for this relic.
    /// </summary>
    /// <returns> The active Effect. </returns>
    public Effect GetActiveEffect()
    {
        return ActiveEffect;
    }

    /// <summary>
    /// Method which returns the active Effect details.
    /// </summary>
    /// <returns> The active Effect details as a string. </returns>
    public string GetActiveEffectDetails()
    {
        return ActiveEffectDetails;
    }

    /// <summary>
    /// Method which returns the LaunchType for the active Effect.
    /// </summary>
    /// <returns> The LaunchType </returns>
    public LaunchType GetLaunchType()
    {
        return LaunchType;
    }

    /// <summary>
    /// Method which returns the list of passive effects associated with this relic.
    /// </summary>
    /// <returns> A shallow copy of the Passive Effects list. </returns>
    public List<Effect> GetPassiveEffects()
    {
        List<Effect> shallowCopy = new List<Effect>();
        foreach (var effect in PassiveEffects) { shallowCopy.Add(effect); }
        return shallowCopy;
    }
    /// <summary>
    /// Method which returns the list of details for each passive effect.
    /// </summary>
    /// <returns></returns>
    public List<string> GetPassiveEffectDetails()
    {
        List<string> shallowCopy = new List<string>();
        foreach (var effect in PassiveEffectsDetails) { shallowCopy.Add(effect); }
        return shallowCopy;
    }
}
