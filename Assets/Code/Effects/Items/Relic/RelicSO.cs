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
    /// The effect that this relic has (deprecated soon).
    /// </summary>
    [SerializeField] private Effect Effect;

    /// <summary>
    /// The details of the effect (deprecated soon).
    /// </summary>
    [SerializeField] private string EffectDetails;

    /// <summary>
    /// Passive Effects on this relic.
    /// </summary>
    [SerializeField] private List<Effect> PassiveEffects = new List<Effect>();

    /// <summary>
    /// Details of the passive effects on this relic.
    /// </summary>
    [SerializeField] private List<string> PassiveEffectsDetails = new List<string>();

    /// <summary>
    /// The active effect attached to this relic.
    /// </summary>
    [SerializeField] private Effect ActiveEffect;

    public float GetPrice()
    {
        return Price;
    }
    /*
    public Effect GetEffect()
    {
        return Effect;
    }
    */
    public string GetEffectDetails()
    {
        return EffectDetails;
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
    public Effect GetActiveEffect()
    {
        return ActiveEffect;
    }
}
