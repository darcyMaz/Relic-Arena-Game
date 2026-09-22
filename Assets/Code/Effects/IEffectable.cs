using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Note on compressing this class:
//      The formatDict could be changed to lambda funcs
//      A function called EffectFunc() could have most of what's in the Effect funcs already made and then make smaller funcs for each effect

/// <summary>
/// Abstract class where those implementing it become Effectable.
/// They may receive Effects, for which there are in-game consequences attached.
/// </summary>
public abstract class IEffectable: MonoBehaviour
{
    /// <summary>
    /// Event that announces the current list of Passive Effects.
    /// </summary>
    public event Action<List<Relic>> OnUpdateEffectRelics;

    /// <summary>
    /// Event that announces when a Relic has been destroyed due to an Effect.
    /// </summary>
    public event Action<Relic> OnRelicConsumed;

    /// <summary>
    /// Dictionary holding all passive effects currently on this IEffectable, mappes to their cancellation token.
    /// </summary>
    protected List<Effect> _currentPassiveEffects = new List<Effect>();

    /// <summary>
    /// The Effects Dictionary, Effect enums are mapped to class methods that take a string as input.
    /// </summary>
    protected Dictionary<Effect, Action<string, Effect, Relic, CancellationToken>> _effectsDict = new Dictionary<Effect, Action<string, Effect, Relic, CancellationToken>>();

    /// <summary>
    /// The cancellation tokens tied to each effect.
    /// </summary>
    private Dictionary<Effect, CancellationTokenSource> _effectsCancellationTokens = new Dictionary<Effect, CancellationTokenSource>();


    /// <summary>
    /// The Format Dictionary, Effect enums are mapped to strings representing the correct format for each Effect's details string.
    /// </summary>
    protected Dictionary<Effect, string> _formatDict = new Dictionary<Effect, string>();

    /// <summary>
    /// Method that runs on Awake.
    /// </summary>
    protected virtual void Awake()
    {
        InitAwake();
    }

    /// <summary>
    /// Method that runs on Enable.
    /// </summary>
    protected virtual void OnEnable()
    {
        InitOnEnable();
    }
    /// <summary>
    /// Method that runs on Disable.
    /// </summary>
    protected virtual void OnDisable()
    {
        InitOnDisable();
    }

    /// <summary>
    /// Method that runs at the Start of the game.
    /// </summary>
    protected virtual void Start()
    {
        InitStart();
    }

    /// <summary>
    /// Initializing function. To be called in the Awake function.
    /// </summary>
    private void InitAwake()
    {
        InitCancellationDict();
        InitEffectDict();
        InitFormatDict();
    }

    /// <summary>
    /// Initialize the Effect dictionary.
    /// The Effect Dictionary has as keys Effect enums, and as values a reference to a class method.
    /// </summary>
    private void InitEffectDict()
    {
        // Add all Effects to the dictionary, where each Effect has a corresponding function.
        _effectsDict.TryAdd(Effect.None, this.NoEffect);
        _effectsDict.TryAdd(Effect.DelayedLightning, this.DelayedLightning);
        _effectsDict.TryAdd(Effect.Fog, this.Fog);
        _effectsDict.TryAdd(Effect.ExtraLives, this.ExtraLives);

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
    /// This could be changed to take in lamba functions instead of string explainers.
    /// </summary>
    private void InitFormatDict()
    {
        _formatDict.TryAdd(Effect.None, "Empty String");
        _formatDict.TryAdd(Effect.DelayedLightning, "Integer representing milliseconds: 3000");
        _formatDict.TryAdd(Effect.Fog, "Integer representing milliseconds: 3000");
        _formatDict.TryAdd(Effect.ExtraLives, "Integer representing the number of extra lives: 3");
    }

    /// <summary>
    /// Method which creates a cancellation token source for each Effect.
    /// </summary>
    private void InitCancellationDict()
    {
        // For each Effect, there must be a source for cancellation tokens.
        foreach (Effect effect in Enum.GetValues(typeof(Effect))) 
        {
            _effectsCancellationTokens.Add(effect, new CancellationTokenSource());
        }
    }

    /// <summary>
    /// Method which provides initializatons for the OnEnable function.
    /// </summary>
    private void InitOnEnable()
    {
        OnUpdateEffectRelics += CheckPassiveEffects;
    }

    /// <summary>
    /// Method which provides initializations for the OnDisable function.
    /// </summary>
    private void InitOnDisable()
    {
        OnUpdateEffectRelics -= CheckPassiveEffects;
    }

    /// <summary>
    /// Method which provides initilizations on start.
    /// </summary>
    private void InitStart()
    {

    }

    /// <summary>
    /// The OnCollisionEnter method is where Effects start being applied to IEffectables.
    /// This may be changed away from an OnTriggerEnter to better suit the needs of the project.
    /// </summary>
    /// <param name="collision"> The body colliding with the IEffectable. </param>
    private void OnTriggerEnter(Collider other)
    {
        // If the collision object has a Relic component on it, then the IEffectable has been hit by a Relic.
        ThrownRelic thrownRelic;
        if (other.gameObject.TryGetComponent(out thrownRelic))
        {
            Relic relic = thrownRelic.GetRelic();
            if (relic == null)
            {
                Debug.LogError("A ThrownRelic hit an IEffectable but it did not have a Relic.");
                return;
            }

            RunEffectAction(relic.GetActiveEffect(), relic.GetActiveEffectDetails(), relic);
        }
    }


    public void ReceiveActiveEffect()
    {

    }
    protected abstract void LaunchActiveEffect();

    private void RunEffectAction(Effect effect, string effectDetails, Relic relic)
    {
        // Try to get a cancellation token source for the effect at hand.
        CancellationTokenSource cancellationTokenSource;
        // Try to get the effect function for the effect at hand.
        Action<string, Effect, Relic, CancellationToken> effectAction;

        // If either of these things are not found, then do not invoke the repsective function.
        if (_effectsCancellationTokens.TryGetValue(effect, out cancellationTokenSource) && _effectsDict.TryGetValue(effect, out effectAction))
        {
            // Apply the effect.
            effectAction.Invoke(effectDetails, effect, relic, cancellationTokenSource.Token);
        }
        else
        {
            Debug.LogError("An IEffectable tried to Invoke an Effect but something went wrong. The Effect: " + effect);
        }
    }

    /// <summary>
    /// Method which invokes the OnUpdateEffectRelics.
    /// This method invokes the event from implementing classes only.
    /// </summary>
    protected void InvokePassiveEffectEvent(List<Relic> effectRelics)
    {
        OnUpdateEffectRelics?.Invoke(effectRelics);
    }

    /// <summary>
    /// This method builds the effect relic list and then should call the function InvokePassiveEffectEvent();
    /// </summary>
    protected abstract void BuildEffectRelicList();

    /// <summary>
    /// This function updates the current passive effects on the player.
    /// It receives a list of Relics whose passive effects are read and applied.
    /// It runs async functions related to those passive effects.
    /// </summary>
    /// <param name="effectRelics"> The updated list of EffectRelics. </param>
    private void CheckPassiveEffects(List<Relic> effectRelics)
    {

        // Build this list which gathers every unique effect in the effectRelics list.
        // This will be useful when checking which effects to remove.
        List<Effect> uniqueEffects = new List<Effect>();

        // Go through each relic and add new effects to the _currentPassiveEffects list.
        foreach (Relic relic in effectRelics)
        {

            // Go through each Effect in this particular relic and see if any of them are not already active.
            foreach (Effect effectKey in relic.GetPassiveEffects().Keys)
            {
                // If this effect hasn't been added to uniqueEffects, add it.
                if (!uniqueEffects.Contains(effectKey) && effectKey != Effect.None)
                {
                    uniqueEffects.Add(effectKey);
                }

                // If one of the effects in this relic is not in the _currentPassiveEffects List.
                if (!_currentPassiveEffects.Contains(effectKey) && effectKey != Effect.None)
                {
                    // The details of this effect.
                    string effectDetails;

                    // If this effect has effect details in the relic.
                    if (relic.GetPassiveEffects().TryGetValue(effectKey, out effectDetails))
                    {
                        // Add the effect.
                        _currentPassiveEffects.Add(effectKey);
                        // Run the effect action.
                        RunEffectAction(effectKey, effectDetails, relic);
                    }
                    else
                    {
                        Debug.Log("There was an attempt by IEffectable to access Passive Effects but it failed. In particular, there was an attempt to access the Dictionary<Effect, string> that offers the details of each passive effect, but no string was returned.");
                    }
                }
            } 
        }


        // This foreach loop will remove effects from the _effectsDict dictionary if they no longer appear on the new effectsList.
        // The removal cannot be dynamic, so we'll add them to the list which will then be used for removal.
        List<Effect> nonDynamicRemoval = new List<Effect>();
        foreach (Effect passiveEffect in _currentPassiveEffects)
        {
            if (!uniqueEffects.Contains(passiveEffect))
            {
                // Queue it for removal from the dictionary.
                nonDynamicRemoval.Add(passiveEffect);

                // Cancel the task at hand using the cancellation token source.
                CancellationTokenSource token;
                bool tokenFound = _effectsCancellationTokens.TryGetValue(passiveEffect, out token);

                // If the token existed.
                if (tokenFound)
                {
                    token.Cancel();
                }
                // Otherwise inform the error log.
                else
                {
                    Debug.LogError("There was an attempt to cancel a passive effect, but the effects cancellation token did not exist in the _currentPassiveEffects dictonary.");
                }
            }
        }

        // Remove these effects from the _effectsDict dictionary.
        foreach (Effect removeThis in nonDynamicRemoval)
        {
            _currentPassiveEffects.Remove(removeThis);
        }


    }

    /// <summary>
    /// Apply the DelayedLightning Effect. This Effect will mainly be handled by the Effect Manager.
    /// </summary>
    /// <param name="delay"> The delay before the lightning strikes. </param>
    /// <param name="currentEffect"> The DelayedLightning Effect. </param>
    private async void DelayedLightning(string delay, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Convert from string to float
        // Tell the effect manager to do this effect onto this IEffectable.
        try
        {
            // Debug.Log("Delayed Lightning Effect not implemented in IEffectable. Requires EffectManager to exist.");
            int delayInt = int.Parse(delay);

            try
            {
                // Wait for the delay or upon cancellation, skip the lightning strike. 
                await Task.Delay(delayInt, token);

                // Strike lightning!
                EffectsManager.Instance.Lightning(transform.position);
                ApplyLightning();
            }
            catch (OperationCanceledException) 
            {
                
            }

            // Make sure this effect is removed from the list of current effects when finished.
            _currentPassiveEffects.Remove(currentEffect);
            OnRelicConsumed?.Invoke(thisRelic);
        }
        catch (FormatException fe)
        {
            _currentPassiveEffects.Remove(currentEffect);
            LogFormatException(fe, currentEffect);
        }
    }
    protected abstract void ApplyLightning();

    /// <summary>
    /// Method which implements the Fog effect.
    /// </summary>
    /// <param name="details"> The string details for this Effect. </param>
    /// <param name="effect"> The Effect itself. </param>
    /// <param name="thisRelic"> The Relic this effect comes from. </param>
    /// <param name="token"> The cancellation token for this async function. </param>
    /// <exception cref="NotImplementedException"></exception>
    private async void Fog(string details, Effect effect, Relic thisRelic, CancellationToken token)
    {
        // ask the effectmanager for a clone of the fog prefab
        // place it at transform.position + z pos
        // then wait details amount of seconds with cancellation in mind
        // then destroy relic
    }

    /// <summary>
    /// Method which applies custom aspects of the Fog effect to each implementation.
    /// </summary>
    protected abstract void ApplyFog();

    /// <summary>
    /// Method which implements the ExtraLife effect.
    /// </summary>
    /// <param name="details"> The string details for this Effect. </param>
    /// <param name="effect"> The Effect itself. </param>
    /// <param name="thisRelic"> The Relic this effect comes from. </param>
    /// <param name="token"> The cancellation token for this async function. </param>
    private async void ExtraLives(string details, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Ensure the format is correct for this effect.
        try
        {
            int extraLives = int.Parse(details);

            try
            {
                ApplyExtraLives(extraLives);
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException)
            {
                CancelExtraLives();
            }

            _currentPassiveEffects.Remove(currentEffect);
            OnRelicConsumed?.Invoke(thisRelic);
        }
        catch (FormatException fe)
        {
            _currentPassiveEffects.Remove(currentEffect);
            LogFormatException(fe, currentEffect);
        }
    }
    /// <summary>
    /// Method which applies custom aspects of the ExtraLife effect to each implementation.
    /// </summary>
    protected abstract void ApplyExtraLives(int extraLives);

    /// <summary>
    /// 
    /// </summary>
    protected abstract void CancelExtraLives();

    /// <summary>
    /// A function which runs when the None Effect runs. It does nothing.
    /// </summary>
    /// <param name="details"> The string details for this Effect. </param>
    /// <param name="effect"> The Effect itself. </param>
    /// <param name="thisRelic"> The Relic this effect comes from. </param>
    /// <param name="token"> The cancellation token for this async function. </param>
    private async void NoEffect(string noEffect, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Debug.Log("The no effect effect has been called. Here's the associated details string: " + noEffect);
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
