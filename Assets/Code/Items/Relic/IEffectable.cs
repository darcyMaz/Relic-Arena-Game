using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEffectable: MonoBehaviour
{
    // IEffectables (1) inform others that they have received or lost effects
    //              (2) have some functionality for receiving and losing effects and
    //              (3) implement each possible effect

    //public event Action <Effect> OnEffectReceived;
    // public event Action <Effect> OnEffectLost;
    
    /// <summary>
    /// The Effects Dictionary, Effect enums are mapped to class methods that take a string as input.
    /// </summary>
    protected Dictionary<Effect, Action<string, Effect>> _effectsDict = new Dictionary<Effect, Action<string, Effect>>();

    /// <summary>
    /// The Format Dictionary, Effect enums are mapped to strings representing the correct format for each Effect's details string.
    /// </summary>
    protected Dictionary<Effect, string> _formatDict = new Dictionary<Effect, string>();
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // Get ID from EffectsManager
    }

    /// <summary>
    /// Initializing function. To be called in the Awake function.
    /// </summary>
    private void Init()
    {
        InitEffectDict();
    }

    /// <summary>
    /// Initialize the Effect dictionary.
    /// The Effect Dictionary has as keys Effect enums, and as values a reference to a class method.
    /// </summary>
    private void InitEffectDict()
    {
        // Add all Effects to the dictionary, where each Effect has a corresponding function.
        _effectsDict.TryAdd(Effect.Test, this.EffectTest);
        _effectsDict.TryAdd(Effect.None, this.NoEffect);
        _effectsDict.TryAdd(Effect.DelayedLightning, this.DelayedLightning);


        // Then do a check at the end to see if the size of the dictionary matches up with the number of Effects.
        int EffectCount = Enum.GetNames(typeof(Effect)).Length;
        if (EffectCount > _effectsDict.Count)
        {
            Debug.Log("IEffectable does not implement every Effect in the Effect enum.");
        }
        else if (EffectCount < _effectsDict.Count)
        {
            Debug.LogError("IEffectable somehow has more implementions for Effects than there are Effects listed in the enum.");
        }
    }

    /// <summary>
    /// Initialize the format dictionary.
    /// The Format Dictionary has as keys Effect enums, and as values the expected format of the details string.
    /// </summary>
    private void InitFormatDict()
    {
        _formatDict.TryAdd(Effect.Test, "This is a test effect, there is no format.");
        _formatDict.TryAdd(Effect.None, "Empty String");
        _formatDict.TryAdd(Effect.DelayedLightning, "An integer: 3");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the collision object has a Relic component on it, then the IEffectable has been hit by a Relic.
        Relic relic;
        if (collision.gameObject.TryGetComponent(out relic))
        {
            // Apply the Effect.
            Action<string, Effect> effectAction;
            _effectsDict.TryGetValue(relic.GetEffect(), out effectAction);
            effectAction.Invoke(relic.GetEffectDetails(), relic.GetEffect());
        }
    }

    private void DelayedLightning(string delay, Effect currentEffect)
    {
        // Convert from string to float
        // Tell the effect manager to do this effect onto this IEffectable.
        try
        {
            float delayFloat = float.Parse(delay);
            // So we'll need to tell the effectsmanager to start this effect.
            // We'll also need to let the effectsmanager know that this effect has ended, potentially I mean.
            // Like, delayed lightning will strike after say, 3 seconds. But if it is cancelled out then that needs to be known.   
        }
        catch (FormatException fe)
        {
            LogFormatException(fe, currentEffect);
        }

        
    }
    private void EffectTest(string test, Effect currentEffect)
    {
        // EffectsManager.Instance.EffectTest();
    }
    private void NoEffect(string noEffect, Effect currentEffect)
    {
        Debug.Log("The no effect effect has been called. Here's the associated details string: " + noEffect);
    }

    /// <summary>
    /// This function logs the error for the case where the format of a Relic's details string is incorrect.
    /// </summary>
    /// <param name="formatException"> The FormatException created at run time, to be passed to the error log. </param>
    /// <param name="effect"> The Effect whose details wer not correctly formatted. </param>
    private void LogFormatException(FormatException formatException, Effect effect)
    {
        string format;

        // This try-catch block makes sure the _formatDict has a format for the Effect key. If it doesn't it mentions that too.
        try
        {
            _formatDict.TryGetValue(effect, out format);
        }
        catch
        {
            format = "(The IEffectable's Format Dictionary could not find this Effect's correct format)";
        }
        Debug.LogError("An IEffectable tried to apply the " + nameof(effect) + " Effect to itself. However, the details string originating in the RelicSO was of the wrong format. The correct format: " + format + "\n" + formatException);
    }

}
