using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    /// <summary>
    /// The EffectsManager is a singleton, and thus has a single instance.
    /// </summary>
    public static EffectsManager Instance { get; private set; }

    /// <summary>
    /// Source of lightning that the EM will be able to use to strike positions.
    /// </summary>
    [SerializeField] private Lightning LightningSource;

    private bool TestEffectLock = false;

    private Dictionary<IEffectable, List<Effect>> _effectables = new Dictionary<IEffectable, List<Effect>>();  

    /// <summary>
    /// The Awake function checks if the current object is the singleton.
    /// </summary>
    private void Awake()
    {
        SingletonCheck();
    }

    /// <summary>
    /// Check whether this is the singleton.
    /// </summary>
    private void SingletonCheck()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Strikes the endPos with lightning. The Lightning component will be called and its line renderer will send a lightning-looking line to hit the endPos.
    /// </summary>
    /// <param name="endPos"> The destination of the line renderer. </param>
    public void Lightning(Vector3 endPos)
    {
        LightningSource.LightningStrike(endPos);
    }

    public async void EffectTest()
    {
        if (TestEffectLock)
        {
            return;
        }

        TestEffectLock = true;
        UIManager.Instance.TestUIUpdate("Effect Test");
        await Task.Delay(3000);
        UIManager.Instance.TestUIUpdate("");
        TestEffectLock = false;
    }
        
}
