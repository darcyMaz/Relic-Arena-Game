using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    //int tempIntClass = 0;


    /// <summary>
    /// Event that announces the current list of Passive Effects.
    /// </summary>
    public event Action<List<Relic>> OnUpdateEffectRelics;

    /// <summary>
    /// Event that announces when a Relic has been destroyed due to an Effect.
    /// </summary>
    public event Action<Relic> OnRelicConsumed;

    /// <summary>
    /// Dictionary holding all passive effects currently on this IEffectable, mappes to the Relic which is powering that Effect.
    /// </summary>
    //protected List<Effect> _currentPassiveEffects = new List<Effect>();
    protected Dictionary<Effect, Relic> _currentPassiveEffects = new Dictionary<Effect, Relic>();


    /// <summary>
    /// The Effects Dictionary, Effect enums are mapped to class methods that take a string as input.
    /// </summary>
    protected Dictionary<Effect, Action<string, Effect, Relic, CancellationToken>> _effectsDict = new Dictionary<Effect, Action<string, Effect, Relic, CancellationToken>>();

    /// <summary>
    /// The cancellation tokens tied to each effect.
    /// </summary>
    protected Dictionary<Effect, CancellationTokenSource> _effectsCancellationTokens = new Dictionary<Effect, CancellationTokenSource>();


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

    /// <summary>
    /// Method which takes an Effect and runs its respective action. 
    /// Meaning, this method allowed effects to be applied.
    /// </summary>
    /// <param name="effect"> The Effect to apply. </param>
    /// <param name="effectDetails"> The Effect's details. </param>
    /// <param name="relic"> The Relic related to the Effect. </param>
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
    /// It receives a list of Relics whose passive effects must be applied to the player ot already are.
    /// It runs async functions related to those passive effects if they're not applied already.
    /// </summary>
    /// <param name="effectRelics"> The updated list of EffectRelics. </param>
    private void CheckPassiveEffects(List<Relic> effectRelics)
    {
        // Build this list which gathers every unique effect in the effectRelics list.
        // This will be useful when checking which effects to remove.
        // List<Effect> uniqueEffects = new List<Effect>();
        //tempIntClass++;
        //int tempInt = tempIntClass;

        //Debug.Log("CheckPassiveEffects " + tempInt);
        // Go through each relic and add new effects to the _currentPassiveEffects list.
        foreach (Relic relic in effectRelics)
        {
            //Debug.Log("\t\t" + relic.GetName() + " " + tempInt);
            // Go through each Effect in this particular relic and see if any of them are not already active.
            foreach (Effect effectKey in relic.GetPassiveEffects().Keys)
            {
                /*
                // If this effect hasn't been added to uniqueEffects, add it.
                if (!uniqueEffects.Contains(effectKey) && effectKey != Effect.None)
                {
                    uniqueEffects.Add(effectKey);
                }
                */

                // If one of the effects in this relic is not in the _currentPassiveEffects List.
                if (!_currentPassiveEffects.TryGetValue(effectKey, out _) && effectKey != Effect.None)
                {
                    //Debug.Log("\t\tEffect is not in the currentPassiveEffects dict " + tempInt);

                    // The details of this effect.
                    string effectDetails;

                    // If this effect has effect details in the relic (as it should have).
                    if (relic.GetPassiveEffects().TryGetValue(effectKey, out effectDetails))
                    {
                        //Debug.Log("\t\tBefore adding it to currentpasseffect " + tempInt);

                        // Add the effect to the dictionary, making sure the relic associated with it is saved.
                        _currentPassiveEffects.Add(effectKey, relic);

                        //Debug.Log("\t\tAfter adding in to currentPassiveEffects dict / before running effect action" + tempInt);

                        // Run the effect action.
                        RunEffectAction(effectKey, effectDetails, relic);

                        //Debug.Log("\t\t After running effect action currentPassiveEffects dict " + tempInt);

                    }
                    else
                    {
                        Debug.Log("There was an attempt by IEffectable to access Passive Effects but it failed. In particular, there was an attempt to access the Dictionary<Effect, string> that offers the details of each passive effect, but no string was returned. " );
                    }
                }
                else
                {
                    Debug.Log("CheckPassiveEffects could have add the following effect but it was already in the currentPassiveEffects dict: " + effectKey + " its relic: " + relic.GetName() );
                }

                //Debug.Log("\t\t\t\t" + effectKey + " " + tempInt);
            }
            

        }

        Debug.Log("At end of checkpassiveeffects: see all effects in _currPassEffects ");
        foreach (Effect dictEffect in _currentPassiveEffects.Keys)
        {
            Debug.Log("\t\t" + dictEffect);
        }


        /*
        // This foreach loop will remove effects from the _currentPassiveEffects dictionary if they no longer appear on the new effectsList.
        // The removal cannot be dynamic, so we'll add them to the list which will then be used for removal.
        List<Effect> nonDynamicRemoval = new List<Effect>();
        foreach (Effect passiveEffect in _currentPassiveEffects.Keys)
        {
            if (!uniqueEffects.Contains(passiveEffect))
            {
                // Queue it for removal from the dictionary.
                nonDynamicRemoval.Add(passiveEffect);
            }
        }

        // Remove these effects from the _effectsDict dictionary.
        foreach (Effect removeThis in nonDynamicRemoval)
        {
            _currentPassiveEffects.Remove(removeThis);
        }
        */


    }

    /// <summary>
    /// Cancel the effects related to this relic.
    /// </summary>
    /// <param name="relic"> The relic whose effects must be cancelled. </param>
    protected void CancelEffectsOnRelic(Relic relic)
    {
        Debug.Log("Cancel effects on relics: ");

        Dictionary<Effect,string>.KeyCollection effectsToCancel = relic.GetPassiveEffects().Keys;

        // For each Effect held by this Relic.
        foreach (Effect effect in effectsToCancel)
        {
            // If this Effect even is active (it should be logically).
            Relic associatedRelic;
            if (_currentPassiveEffects.TryGetValue(effect, out associatedRelic))
            {
                // If the incoming relic and the relic associated with this effect are the same then this Effect will be removed.
                if (relic == associatedRelic)
                {
                    // Remove this Effect from the dictionary.
                    _currentPassiveEffects.Remove(effect);

                    // Cancel the task at hand using the cancellation token source.
                    CancellationTokenSource token;
                    bool tokenFound = _effectsCancellationTokens.TryGetValue(effect, out token);

                    // If the token existed.
                    if (tokenFound)
                    {
                        token.Cancel();
                        Debug.Log("The following has been cancelled: " + effect);

                        // Here, make a new cancellationtokensource and replace the old one.
                        _effectsCancellationTokens.Remove(effect);
                        _effectsCancellationTokens.Add(effect, new CancellationTokenSource());

                        //Debug.Log("CancelEffectsOnRelic: " + relic.GetName() + " " + effect);
                    }
                    // Otherwise inform the error log.
                    else
                    {
                        Debug.LogError("There was an attempt to cancel a passive effect, but the effects cancellation token did not exist in the effectsCancellationTokens dictonary.");
                    }
                }
            }
        }
        OnRelicConsumed?.Invoke(relic);
    }

    // TO-DO: Cut down the code of later effect action functions which all use this format.
    /*
    private async void EffectShell(Relic thisRelic)
    {
        string details = "";
        Effect currentEffect = Effect.None;
        CancellationToken token;

        // Ensure the format is correct for this effect.
        try
        {

        }
        catch (FormatException fe)
        {
            // Should the relic be consumed at this stage?
            LogFormatException(fe, currentEffect);
        }

        // Cancel all effects related to this relic when this one is complete.
        // Effects associated with a different relic will not be cancelled.
        CancelEffectsOnRelic(thisRelic);
    }
    */

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
                //Debug.Log("After lightning delay!");

                // Strike lightning!
                EffectsManager.Instance.Lightning(transform.position);
                ApplyLightning();
            }
            catch (OperationCanceledException) 
            {
                //Debug.Log("Lightning cancelled");
            }

            // Make sure this effect is removed from the list of current effects when finished.
            // _currentPassiveEffects.Remove(currentEffect);
            // OnRelicConsumed?.Invoke(thisRelic);
        }
        catch (FormatException fe)
        {
            // _currentPassiveEffects.Remove(currentEffect);
            LogFormatException(fe, currentEffect);
        }

        CancelEffectsOnRelic(thisRelic);
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
    private async void Fog(string details, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Ensure the format of the effect details is correct for this effect.
        try
        {
            // Ensure that the operation is not cancelled.
            try
            {
                // Get the Fog GameObject clone from the EffectsManager.
                GameObject FogClone = EffectsManager.Instance.GetFog(transform.position + new Vector3(0, 0, -0.5f));

                while (true)
                {
                    if (FogClone == null) break;

                    FogClone.transform.position = transform.position + new Vector3(0, 0, -0.5f);

                    if (token.IsCancellationRequested) 
                    {
                        Debug.Log("Fog cancelled");
                        break;
                    }
                    await Task.Delay(100);
                }
                
                // This does cause an error without the if statement when the game shuts down mid-game and this clone exists.
                if (FogClone != null) Destroy(FogClone.gameObject);
            }
            catch (OperationCanceledException)
            {

            }
        }
        catch (FormatException fe)
        {
            // Should the relic be consumed at this stage?
            LogFormatException(fe, currentEffect);
        }

        // Cancel all effects related to this relic when this one is complete.
        // Effects associated with a different relic will not be cancelled.
        CancelEffectsOnRelic(thisRelic);
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
                ApplyExtraLives(extraLives, thisRelic);
                // await Task.Delay(Timeout.Infinite, token);

                while (true)
                {
                    // Debug.Log("Yes, extra lives is still running.");
                    if (token.IsCancellationRequested)
                    {
                        Debug.Log("ExtraLives is cancelled");
                        break;
                    }
                    await Task.Delay(100);
                }
            }
            catch (OperationCanceledException)
            {
                CancelExtraLives();
            }
            
        }
        catch (FormatException fe)
        {
            // Should the relic be consumed at this stage?
            LogFormatException(fe, currentEffect);
        }

        Debug.Log("end of ExtraLives");
        // Cancel all effects related to this relic when this one is complete.
        // Effects associated with a different relic will not be cancelled.
        CancelEffectsOnRelic(thisRelic);
    }
    /// <summary>
    /// Method which applies custom aspects of the ExtraLife effect to each implementation.
    /// </summary>
    protected abstract void ApplyExtraLives(int extraLives, Relic extraLifeRelic);

    /// <summary>
    /// Method which helps cancel the ExtraLife effect.
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
