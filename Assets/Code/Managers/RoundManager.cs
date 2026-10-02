using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    /// <summary>
    /// Event which activates or deactivates the game depending on the bool parameter.
    /// </summary>
    public event Action<bool, int> OnGameActivation;

    /// <summary>
    /// Method which starts a round.
    /// </summary>
    public void StartRound(int roundNumber)
    {
        OnGameActivation?.Invoke(true, roundNumber);
    }

    /// <summary>
    /// Method which ends a round.
    /// </summary>
    public void EndRound(int roundNumber)
    {
        OnGameActivation?.Invoke(false, roundNumber);
    }
}
