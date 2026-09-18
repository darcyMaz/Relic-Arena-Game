using UnityEngine;

public class ThrownRelic : MonoBehaviour
{
    /// <summary>
    /// Relic in this ThrownRelic.
    /// </summary>
    [SerializeField] private Relic _relic;

    /// <summary>
    /// Set the relic.
    /// </summary>
    /// <param name="relic"> Teh relic to set. </param>
    public void SetRelic(Relic relic)
    {
        _relic = relic;
    }

    /// <summary>
    /// Get the Relic.
    /// </summary>
    /// <returns> The Relic in this ThrownRelic </returns>
    public Relic GetRelic()
    {
        return _relic;
    }
}
