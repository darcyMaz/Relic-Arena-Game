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
    /// Dictionary holding all passive effects currently on this IEffectable, mappes to the Relic which is powering that Effect.
    /// </summary>
    //protected List<Effect> _passiveEffects = new List<Effect>();
    protected Dictionary<Effect, Relic> _passiveEffects = new Dictionary<Effect, Relic>();


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
    /// The List of all active Effects.
    /// </summary>
    protected List<Effect> _activeEffects = new List<Effect>();

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

    protected virtual void Update()
    {
        UpdateHelper();
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
        _effectsDict.TryAdd(Effect.LightningForray, this.LightningForray);

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
        _formatDict.TryAdd(Effect.LightningForray, "Details for this not implemented, see EffectsManager to change values.");
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
        CancelAllEffects();
    }

    /// <summary>
    /// Method which provides initilizations on start.
    /// </summary>
    private void InitStart()
    {

    }

    private void UpdateHelper()
    {

    }

    /// <summary>
    /// This method cancels all effects.
    /// </summary>
    private void CancelAllEffects()
    {
        // For each Effect, there must be a source for cancellation tokens.
        foreach (Effect effect in Enum.GetValues(typeof(Effect)))
        {
            CancellationTokenSource cts;
            if (_effectsCancellationTokens.TryGetValue(effect, out cts))
            {
                cts.Cancel();
            }
        }
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

            ReceiveActiveEffect(relic.GetActiveEffect(), relic.GetActiveEffectDetails());
            Destroy(other.gameObject);
        }

        
    }

    /// <summary>
    /// This method is called by those seeking to apply active Effects to this IEffectable. 
    /// </summary>
    /// <param name="activeEffect"> Active effect being applied. </param>
    /// <param name="activeEffectDetails"> Details to the active effect being applied. </param>
    public void ReceiveActiveEffect(Effect activeEffect, string activeEffectDetails)
    {
        // If the incoming active effect is neither in the active effects list or the passive effects dictionary then activate this effect.
        if (!_activeEffects.Contains(activeEffect) && !_passiveEffects.TryGetValue(activeEffect, out _) && activeEffect != Effect.None)
        {
            // Add this active effect to the list.
            _activeEffects.Add(activeEffect);
            // Run the Effect Action where the relic is null.
            RunEffectAction(activeEffect, activeEffectDetails, null);
        }
        else
        {
            Debug.Log("An active Effect was receieved by an IEffectable but it was already active on the IEffectable so it was ignored.");
        }
    }
    protected abstract void LaunchActiveEffect(Vector2 direction);

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
        // Debug.Log("Invoke passive effecrt event() start");
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
        // Debug.Log("Start of check passive effects");
        
        // Go through each relic and add new effects to the _passiveEffects list.
        foreach (Relic relic in effectRelics)
        {
            // Go through each Effect in this particular relic and see if any of them are not already active.
            foreach (Effect effectKey in relic.GetPassiveEffects().Keys)
            {

                // If one of the effects in this relic is not in the _passiveEffects List or the _activeEffects list.
                if ( !_activeEffects.Contains(effectKey) && !_passiveEffects.TryGetValue(effectKey, out _) && effectKey != Effect.None)
                {

                    // The details of this effect.
                    string effectDetails;

                    // If this effect has effect details in the relic (as it should have).
                    if (relic.GetPassiveEffects().TryGetValue(effectKey, out effectDetails))
                    {

                        // Add the effect to the dictionary, making sure the relic associated with it is saved.
                        _passiveEffects.Add(effectKey, relic);

                        // Run the effect action.
                        RunEffectAction(effectKey, effectDetails, relic);

                    }
                    else
                    {
                        Debug.Log("There was an attempt by IEffectable to access Passive Effects but it failed. In particular, there was an attempt to access the Dictionary<Effect, string> that offers the details of each passive effect, but no string was returned. " );
                    }
                }
                else
                {
                    // Debug.Log("CheckPassiveEffects could have add the following effect but it was already in the currentPassiveEffects dict: " + effectKey + " its relic: " + relic.GetName() );
                }
            }
            

        }
        
    }

    /// <summary>
    /// Cancel the effects related to this relic.
    /// It is set to private because the order of method operations must stay inside IEffectable.
    /// </summary>
    /// <param name="relic"> The relic whose effects must be cancelled. </param>
    private void CancelEffectsOnRelic(Relic relic)
    {
        Dictionary<Effect,string>.KeyCollection effectsToCancel = relic.GetPassiveEffects().Keys;

        // For each Effect held by this Relic.
        foreach (Effect effect in effectsToCancel)
        {

            // If this Effect even is active (it should be logically).
            Relic associatedRelic;
            if (_passiveEffects.TryGetValue(effect, out associatedRelic))
            {
                // If the incoming relic and the relic associated with this effect are the same then this Effect will be removed.
                if (relic == associatedRelic)
                {
                    // Remove this Effect from the dictionary.
                    _passiveEffects.Remove(effect);

                    // Cancel the task at hand using the cancellation token source.
                    CancellationTokenSource token;
                    bool tokenFound = _effectsCancellationTokens.TryGetValue(effect, out token);

                    // If the token existed.
                    if (tokenFound)
                    {
                        token.Cancel();

                        // Here, make a new cancellationtokensource and replace the old one.
                        _effectsCancellationTokens.Remove(effect);
                        _effectsCancellationTokens.Add(effect, new CancellationTokenSource());
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
            int delayInt = int.Parse(delay);

            try
            {

                

                // Wait for the delay or upon cancellation, skip the lightning strike. 
                await Task.Delay(delayInt, token);

                // If the token was cancelled, then we still need to check and throw the exception, otherwise the rest of the operation will run.
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                // Strike lightning!
                if (this.gameObject != null) EffectsManager.Instance.Lightning(transform.position);
                ApplyLightning();
            }
            catch (OperationCanceledException) 
            {
                //Debug.Log("Lightning cancelled");
            }
        }
        catch (FormatException fe)
        {
            // _passiveEffects.Remove(currentEffect);
            LogFormatException(fe, currentEffect);
        }

        // If this is a passive Effect related to a Relic.
        if (thisRelic != null) CancelEffectsOnRelic(thisRelic);
    }
    protected abstract void ApplyLightning();

    /// <summary>
    /// Method which implements the Fog effect.
    /// </summary>
    /// <param name="details"> The string details for this Effect. </param>
    /// <param name="effect"> The Effect itself. </param>
    /// <param name="thisRelic"> The Relic this effect comes from. </param>
    /// <param name="token"> The cancellation token for this async function. </param>
    private async void Fog(string details, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Ensure the format of the effect details is correct for this effect.
        try
        {
            // Get the Fog GameObject clone from the EffectsManager.
            GameObject FogClone = EffectsManager.Instance.GetFog(this.transform);

            // Ensure that the operation is not cancelled.
            try
            {
                // Set the position of the fog to be just in front of the IEffectable.
                FogClone.transform.localPosition = new Vector3(0, 0, -0.5f);

                while (true)
                {
                    // If the FogClone does not exist, break.
                    if (FogClone == null) break;

                    // When token is cancelled.
                    if (token.IsCancellationRequested) 
                    {
                        throw new OperationCanceledException();
                    }

                    await Task.Delay(100);
                }
                
            }
            catch (OperationCanceledException)
            {
                // This does cause an error without the if statement when the game shuts down mid-game and this clone exists.
                if (FogClone != null) Destroy(FogClone.gameObject);
            }
        }
        catch (FormatException fe)
        {
            // Should the relic be consumed at this stage?
            LogFormatException(fe, currentEffect);
        }

        // Cancel all effects related to this relic when this one is complete.
        // Effects associated with a different relic will not be cancelled.
        // Active Effects will not run this line of code.
        if (thisRelic != null) CancelEffectsOnRelic(thisRelic);
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

                // There is no time limit on this effect.
                while (true)
                {
                    // When cancelled, throw this error.
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    // Yield the processes to others.
                    await Task.Delay(100);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancel the extra lives effect.
                CancelExtraLives();
                
            }
            
        }
        catch (FormatException fe)
        {
            // Should the relic be consumed at this stage?
            LogFormatException(fe, currentEffect);
        }

        //Debug.Log("end of ExtraLives");
        // Cancel all effects related to this relic when this one is complete.
        // Effects associated with a different relic will not be cancelled.
        if (thisRelic != null) CancelEffectsOnRelic(thisRelic);
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
    /// 
    /// </summary>
    /// <param name="details"></param>
    /// <param name="currentEffect"></param>
    /// <param name="thisRelic"></param>
    /// <param name="token"></param>
    private async void LightningForray(string details, Effect currentEffect, Relic thisRelic, CancellationToken token)
    {
        // Ensure the format is correct for this effect.
        try
        {
            // This effect gets all of its info from the EffectsManager.
            // This should probably be changed so that different relics can have variations for this.

            try
            {
                // Debug.Log("Before lightning forray loop, 3 seconds");
                // await Task.Delay(3000);


                while (true)
                {
                    // Create the lighting marker.
                    Vector3 markposition = RandomPositionVariation(EffectsManager.Instance.GetLightningMarkerMaxVariation());
                    GameObject mark = EffectsManager.Instance.GetLightningMarker(markposition + new Vector3(0, 0, 0.5f));

                    // Wait the assigned amount of time.
                    await Task.Delay(EffectsManager.Instance.GetLightningMarkerDelay(), token);
                    if (token.IsCancellationRequested)
                    {
                        Destroy(mark.gameObject);
                        throw new OperationCanceledException();
                    }

                    // Strike lightning at the mark.
                    EffectsManager.Instance.Lightning(mark.transform.position);

                    // If the player is within the mark's zone, then get hit and also cancel this effect.
                    if (Vector2.Distance(transform.position, mark.transform.position) < EffectsManager.Instance.GetLightningMarkerRadius())
                    {
                        // Hit!
                        ApplyLightningForray();
                        // Destroy the mark.
                        Destroy(mark.gameObject);
                        // Cancel the effect.
                        throw new OperationCanceledException();
                    }

                    // Destroy the lightning mark.
                    Destroy(mark.gameObject);

                    // Delay between the spawning in of marks.
                    await Task.Delay(EffectsManager.Instance.GetNextLightningMarkerDelay(), token);

                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                }
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

    protected abstract void ApplyLightningForray();


    private Vector3 RandomPositionVariation(float variationRange)
    {
        Vector3 newPosition = new Vector3(0,0,transform.position.z);
        newPosition.x = UnityEngine.Random.Range(transform.position.x - variationRange, transform.position.x + variationRange);
        newPosition.y = UnityEngine.Random.Range(transform.position.y - variationRange, transform.position.y + variationRange);

        return newPosition;
    }

    /// <summary>
    /// This method determines whether a Relic is an Effect relic.
    /// </summary>
    /// <param name="relic"></param>
    /// <returns></returns>
    protected bool IsEffectRelic(Relic relic)
    {
        // Get the passive effects.
        Dictionary<Effect, string>.KeyCollection passiveEffects = relic.GetPassiveEffects().Keys;

        // If this list is greater than 0, then double check to see if they're not all the None Effect.
        // Yes, I know this would be strange.
        if (passiveEffects.Count > 0)
        {
            // Use a counter. If it is greater than 0 at the end of the foreach loop, then it has an actual passive effect.
            int checkForNonNoneEffect = 0;
            foreach (Effect passiveEffect in passiveEffects)
            {
                if (passiveEffect == Effect.None) continue;
                checkForNonNoneEffect++;
            }
            if (checkForNonNoneEffect > 0)
            {
                return true;
            }
        }

        return false;
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
