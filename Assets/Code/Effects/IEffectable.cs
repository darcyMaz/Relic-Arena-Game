using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Abstract class where those implementing it become Effectable.
/// They may receive Effects, for which there are in-game consequences attached.
/// </summary>
public abstract class IEffectable: MonoBehaviour
{
    // IEffectables (1) inform others that they have received or lost effects
    //              (2) have some functionality for receiving and losing effects and
    //              (3) implement each possible effect

    public event Action <Effect> OnEffectReceived;
    public event Action <Effect> OnEffectLost;
    
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

    /// <summary>
    /// Initializing function. To be called in the Awake function.
    /// </summary>
    private void Init()
    {
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
    /// The OnCollisionEnter method is where Effects start being applied to IEffectables.
    /// </summary>
    /// <param name="collision"> The body colliding with the IEffectable. </param>
    private void OnCollisionEnter(Collision collision)
    {

        // If the collision object has a Relic component on it, then the IEffectable has been hit by a Relic.
        Relic relic;
        if (collision.gameObject.TryGetComponent(out relic))
        {
            // Apply the Effect.
            Action<string, Effect> effectAction;

            _effectsDict.TryGetValue(relic.GetEffect(), out effectAction);
            // Inform listeners of the Effect starting.
            OnEffectReceived?.Invoke(relic.GetEffect());
            effectAction.Invoke(relic.GetEffectDetails(), relic.GetEffect());

            Debug.Log(relic.GetEffect());
        }
    }

    /// <summary>
    /// Apply the DelayedLightning Effect. This Effect will mainly be handled by the Effect Manager.
    /// </summary>
    /// <param name="delay"> The delay before the lightning strikes. </param>
    /// <param name="currentEffect"> The DelayedLightning Effect. </param>
    private async void DelayedLightning(string delay, Effect currentEffect)
    {

        // Convert from string to float
        // Tell the effect manager to do this effect onto this IEffectable.
        try
        {
            //float delayFloat = float.Parse(delay);
            // Debug.Log("Delayed Lightning Effect not implemented in IEffectable. Requires EffectManager to exist.");
            int delayInt = int.Parse(delay);

            // Need to make a task that listens for an effect being cancelled.
            await Task.Delay(delayInt);

            // Strike lightning!
            EffectsManager.Instance.Lightning(transform.position);
        }
        catch (FormatException fe)
        {
            LogFormatException(fe, currentEffect);
        }
    }
    protected abstract void ApplyLightning();

    private void EffectTest(string test, Effect currentEffect)
    {
        // EffectsManager.Instance.EffectTest();
    }
    private void NoEffect(string noEffect, Effect currentEffect)
    {
        Debug.Log("The no effect effect has been called. Here's the associated details string: " + noEffect);
    }

    /// <summary>
    /// Apply the Knockout Effect.
    /// </summary>
    /// <param name="knockoutTime"> The knockout time as a string to be reformatted. </param>
    /// <param name="currentEffect"> The Knockout Effect. </param>
    private void Knockout(string knockoutTime, Effect currentEffect)
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

    private void ChangeSpeed(string speedAndDuration, Effect currentEffect)
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
