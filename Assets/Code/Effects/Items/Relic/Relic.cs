using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Relic
{
    [SerializeField] private RelicSO _relic;

    public Relic(RelicSO relicSO)
    {
        _relic = relicSO;
    }

    public string GetName()
    {
        CheckRelic();
        return _relic.GetName();
    }

    public Effect GetActiveEffect()
    {
        CheckRelic();
        return _relic.GetActiveEffect();
    }
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
