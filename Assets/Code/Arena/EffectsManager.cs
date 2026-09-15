using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    private bool TestEffectLock = false;

    private Dictionary<IEffectable, List<Effect>> _effectables = new Dictionary<IEffectable, List<Effect>>();  


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


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
