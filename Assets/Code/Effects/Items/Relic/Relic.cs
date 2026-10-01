using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// Class representing the Relics in the gane. Each Relic object holds the information related to the relics that players find/dig up and the Effects asociated with them.
/// </summary>  
public class Relic
{
    /// <summary>
    /// RelicSO associated with this Relic.
    /// </summary>
    [SerializeField] private RelicSO _relic;

    /// <summary>
    /// Public constructor for the Relic class.
    /// </summary>
    /// <param name="relicSO"> The RelicSO data to be associated with this Relic. </param>
    public Relic(RelicSO relicSO)
    {
        _relic = relicSO;
    }

    /// <summary>
    /// Method which returns the name of the Relic as a string.
    /// </summary>
    /// <returns> The name of the Relic as a string. </returns>
    public string GetName()
    {
        CheckRelic();
        return _relic.GetName();
    }

    /// <summary>
    /// Method which returns the active Effect of this Relic.
    /// </summary>
    /// <returns> The active Effect. </returns>
    public Effect GetActiveEffect()
    {
        CheckRelic();
        return _relic.GetActiveEffect();
    }

    /// <summary>
    /// Method which returns the LaunchType of the active Effect.
    /// </summary>
    /// <returns> The LaunchType of the active Effect. </returns>
    public LaunchType GetLaunchType()
    {
        CheckRelic();
        return _relic.GetLaunchType();
    }

    /// <summary>
    /// Method which returns a dictionary mapping the passive Effects to their details.
    /// </summary>
    /// <returns> The dictionary mapping Effect and details as a string. </returns>
    public Dictionary<Effect,string> GetPassiveEffects()
    {
        CheckPassiveEffectDetails();

        // Build a dictionary for passive effects mapped to their details.
        Dictionary<Effect, string> passiveEffects = new Dictionary<Effect,string>();

        // For each effect and for each detail, map them together.
        for (int effectIndex = 0; effectIndex < _relic.GetPassiveEffects().Count; effectIndex++)
        {
            passiveEffects.Add(_relic.GetPassiveEffects()[effectIndex], _relic.GetPassiveEffectDetails()[effectIndex]);
        }

        return passiveEffects;
    }

    public float GetPrice()
    {
        CheckRelic();
        return _relic.GetPrice();
    }

    public Sprite GetSprite()
    {
        return _relic.GetSprite();
    }

    public string GetActiveEffectDetails()
    {
        CheckRelic();
        return _relic.GetActiveEffectDetails();
    }

    private void CheckRelic()
    {
        if (_relic == null) throw new UnassignedReferenceException("A Relic tried to read its RelicSO. It did not exist.");
    }
    private void CheckPassiveEffectDetails()
    {
        CheckRelic();
        if (_relic.GetPassiveEffectDetails().Count != _relic.GetPassiveEffects().Count)
        {
            throw new DataException("A Relic's lists for passive effects and their details are not the same size.");
        }
    }
}
