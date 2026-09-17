using UnityEngine;

public class Relic : MonoBehaviour
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
    public Effect GetEffect()
    {
        CheckRelic();
        return _relic.GetEffect();
    }
    public float GetPrice()
    {
        CheckRelic();
        return _relic.GetPrice();
    }
    public string GetEffectDetails()
    {
        CheckRelic();
        return _relic.GetEffectDetails();
    }

    private void CheckRelic()
    {
        if (_relic == null) throw new UnassignedReferenceException("A Relic tried to read its RelicSO. It did not exist.");
    }
}
