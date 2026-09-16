 using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Scriptable Objects/Relic")]
public class RelicSO : ItemSO
{
    [SerializeField] private float Price;
    [SerializeField] private Effect Effect;
    [SerializeField] private string EffectDetails;

    public float GetPrice()
    {
        return Price;
    }
    public Effect GetEffect()
    {
        return Effect;
    }
    public string GetEffectDetails()
    {
        return EffectDetails;
    }
}
