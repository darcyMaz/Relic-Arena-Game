using UnityEngine;

/// <summary>
/// Class which allows for the cross scene storage of the winner.
/// </summary>
public class Winner : MonoBehaviour
{
    /// <summary>
    /// Int representing the winner.
    /// </summary>
    [SerializeField] private int _winner = 0;

    /// <summary>
    /// Method which sets the winner.
    /// </summary>
    /// <param name="winner"> Winner to set as an int. </param>
    public void SetWinner(int winner)
    {
        _winner = winner;
    }

    /// <summary>
    /// Method which returns the winner.
    /// </summary>
    /// <returns> Winner as an int. </returns>
    public int GetWinner()
    {
        return _winner;
    }
}
