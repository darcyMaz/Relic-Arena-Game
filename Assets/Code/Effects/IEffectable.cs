using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

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
    /// Dictionary holding all passive effects currently on this IEffectable, mappes to their cancellation token.
    /// </summary>
    protected List<Effect> _currentPassiveEffects = new List<Effect>();

    /// <summary>
    /// The Effects Dictionary, Effect enums are mapped to class methods that take a string as input.
    /// </summary>
    protected Dictionary<Effect, Action<string, Effect, CancellationToken>> _effectsDict = new Dictionary<Effect, Action<string, Effect, CancellationToken>>();

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
        AwakeInit();
        Debug.Log("Awake in IEffectable");
    }

    /// <summary>
    /// Method that runs on Enable.
    /// </summary>
    protected virtual void OnEnable()
    {
        Debug.Log("OnEnable(): IEffectable");
        OnUpdateEffectRelics += CheckPassiveEffects;
    }
    /// <summary>
    /// Method that runs on Disable.
    /// </summary>
    protected virtual void OnDisable()
    {
        OnUpdateEffectRelics -= CheckPassiveEffects;
    }

    /// <summary>
    /// Method that runs at the Start of the game.
    /// </summary>
    protected virtual void Start()
    {

    }

    /// <summary>
    /// Initializing function. To be called in the Awake function.
    /// </summary>
    private void AwakeInit()
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
        _effectsDict.TryAdd(Effect.Test, this.EffectTest);
        _effectsDict.TryAdd(Effect.None, this.NoEffect);
        _effectsDict.TryAdd(Effect.DelayedLightning, this.DelayedLightning);
        _effectsDict.TryAdd(Effect.Knockout, this.Knockout);
        _effectsDict.TryAdd(Effect.ChangeSpeed, this.ChangeSpeed);

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
        _formatDict.TryAdd(Effect.DelayedLightning, "Integer representing milliseconds: 3000");
        _formatDict.TryAdd(Effect.Knockout, "float: 1.4");
        _formatDict.TryAdd(Effect.ChangeSpeed, "Non-negative float,non-negative float: 0.1,1.2");
    }

    /// <summary>
    /// Method which creates a cancellation token source for each Effect.
    /// </summary>
    private void InitCancellationDict()
    {
        Debug.Log("InitCancellationDict in IEffectable");

        // For each Effect, there must be a source for cancellation tokens.
        foreach (Effect effect in Enum.GetValues(typeof(Effect))) 
        {
            _effectsCancellationTokens.Add(effect, new CancellationTokenSource());
        }
    }

    /// <summary>
    /// The OnCollisionEnter method is where Effects start being applied to IEffectables.
    /// </summary>
    /// <param name="collision"> The body colliding with the IEffectable. </param>
    private void OnTriggerEnter(Collider collision)
    {
        // If the collision object has a Relic component on it, then the IEffectable has been hit by a Relic.
        ThrownRelic thrownRelic;
        if (collision.gameObject.TryGetComponent(out thrownRelic))
        {
            Relic relic = thrownRelic.GetRelic();
            if (relic == null)
            {
                Debug.LogError("A ThrownRelic hit an IEffectable but it did not have a Relic.");
                return;
            }

            RunEffectAction(relic.GetEffect(), relic.GetEffectDetails());
        }
    }

    private void RunEffectAction(Effect effect, string effectDetails)
    {
        // Try to get a cancellation token source for the effect at hand.
        CancellationTokenSource cancellationTokenSource;
        // Try to get the effect function for the effect at hand.
        Action<string, Effect, CancellationToken> effectAction;

        // If either of these things are not found, then do not invoke the repsective function.
        if (_effectsCancellationTokens.TryGetValue(effect, out cancellationTokenSource) && _effectsDict.TryGetValue(effect, out effectAction))
        {
            // Apply the effect.
            effectAction.Invoke(effectDetails, effect, cancellationTokenSource.Token);
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
        Debug.Log("---");
        Debug.Log("InvokePassiveEffectEvent() in IEffectable.");
        foreach (Relic rel in effectRelics) { Debug.Log(rel.GetName() + " " + rel.GetEffect()); }
        Debug.Log("---");

        /*

        for (int i=0; i< OnUpdateEffectRelics.GetInvocationList().Length; i++)
        {
            Debug.Log("invoc list for event: " + OnUpdateEffectRelics.GetInvocationList()[i]);
        }

        */

        OnUpdateEffectRelics?.Invoke(effectRelics);
    }
    protected abstract void BuildEffectRelicList();
    /// <summary>
    /// This function updates the current passive effects on the player.
    /// It runs async functions related to those passive effects, and cancels them when they are complete.
    /// </summary>
    /// <param name="effectRelics"> The updated list of EffectRelics. </param>
    private void CheckPassiveEffects(List<Relic> effectRelics)
    {
        Debug.Log("CheckPassiveEffects() in IEffectable");

        // Received as a parameter is a list of Relics, this part of the method prunes the list of any duplicate effects.
        // A list of Effects is created so that each relic does not need to compare itself to every other relic, just the effects that have already showed up.
        List<Relic> prunedEffectRelics = new List<Relic>();
        List<Effect> newPassiveEffects = new List<Effect>();

        foreach (Relic relic in effectRelics) 
        {
            Debug.Log(relic.GetEffect() + " " + relic.GetName());

            // If the currentPassiveEffects list does NOT contain the effect already, then add it to the list.
            if (!newPassiveEffects.Contains(relic.GetEffect()))
            {
                newPassiveEffects.Add(relic.GetEffect());
                prunedEffectRelics.Add(relic);
            }
        }

        // Go through each pruned relic and add new effects to the _currentPassiveEffects list.
        foreach (Relic relic in prunedEffectRelics)
        {
            // If the passive effects dictionary does not have that effect, run that effects async function and get the cancellation token.
            if (!_currentPassiveEffects.Contains(relic.GetEffect()))
            {
                // Run the async func and get its cancellation token source.
                // Debug.Log("A new passive effect would have been added. But the implementation is not complete: " + effect);

                _currentPassiveEffects.Add(relic.GetEffect());
                RunEffectAction(relic.GetEffect(), relic.GetEffectDetails());
            }
        }

        // This foreach loop will remove effects from the _effectsDict dictionary if they no longer appear on the new effectsList.
        // The removal cannot be dynamic, so we'll add them to the list which will then be used for removal.
        List<Effect> nonDynamicRemoval = new List<Effect>();
        foreach (Effect passiveEffect in _currentPassiveEffects)
        {
            // If an effect in the _currentPassiveEffects List is not in the updated list of effects, then the effect has ended.
            if (!newPassiveEffects.Contains(passiveEffect) && !nonDynamicRemoval.Contains(passiveEffect))
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
            _effectsDict.Remove(removeThis);
        }

        Debug.Log("---");
    }

    /// <summary>
    /// Apply the DelayedLightning Effect. This Effect will mainly be handled by the Effect Manager.
    /// </summary>
    /// <param name="delay"> The delay before the lightning strikes. </param>
    /// <param name="currentEffect"> The DelayedLightning Effect. </param>
    private async void DelayedLightning(string delay, Effect currentEffect, CancellationToken token)
    {
        // Convert from string to float
        // Tell the effect manager to do this effect onto this IEffectable.
        try
        {
            // Debug.Log("Delayed Lightning Effect not implemented in IEffectable. Requires EffectManager to exist.");
            int delayInt = int.Parse(delay);

            // Wait for the delay or cancel the delay and this func if the token hears about a cancel.
            await Task.Delay(delayInt, token);

            // Strike lightning!
            EffectsManager.Instance.Lightning(transform.position);
        }
        catch (FormatException fe)
        {
            LogFormatException(fe, currentEffect);
        }
    }
    protected abstract void ApplyLightning();

    private void EffectTest(string test, Effect currentEffect, CancellationToken token)
    {
        // EffectsManager.Instance.EffectTest();
    }
    private void NoEffect(string noEffect, Effect currentEffect, CancellationToken token)
    {
        Debug.Log("The no effect effect has been called. Here's the associated details string: " + noEffect);
    }

    /// <summary>
    /// Apply the Knockout Effect.
    /// </summary>
    /// <param name="knockoutTime"> The knockout time as a string to be reformatted. </param>
    /// <param name="currentEffect"> The Knockout Effect. </param>
    private void Knockout(string knockoutTime, Effect currentEffect, CancellationToken token)
    {
        try
        {
            float knockoutTimeFloat = float.Parse(knockoutTime);
            ApplyKnockout(knockoutTimeFloat);
        }
        catch (FormatException fe)
        {
            LogFormatException(fe, currentEffect);
        }
    }
    /// <summary>
    /// Apply the Knockout Effect. This Effect requires an abstract method because this gameObject must implement the Knockout effect for itself, as opposed to other gameObjects affecting this.
    /// </summary>
    /// <param name="knockoutTime"> Time for which this body is knocked out. </param>
    protected abstract void ApplyKnockout(float knockoutTime);

    private void ChangeSpeed(string speedAndDuration, Effect currentEffect, CancellationToken token)
    {
        try
        {
            // This could cover more edge cases than just format exception.
            string[] infoSplit = speedAndDuration.Split(',');
            float speed = float.Parse(infoSplit[0]);
            float duration = float.Parse(infoSplit[1]);
            ApplyChangeSpeed(speed, duration);
        }
        catch (FormatException fe)
        {
            LogFormatException(fe, currentEffect);
        }
    }

    protected abstract void ApplyChangeSpeed(float speed, float duration);

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
