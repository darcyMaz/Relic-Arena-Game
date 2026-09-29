using UnityEngine;

/// <summary>
/// The Score Manager is a class which keeps track of the Score of the players and may hold other stats.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Player 1's score.
    /// </summary>
    public float Player1Score { get; private set; }
    /// <summary>
    /// Player 2's score.
    /// </summary>
    public float Player2Score {get; private set; }

    /// <summary>
    /// Method which increments the score of players in the game and returns a success indicator.
    /// </summary>
    /// <param name="score">  </param>
    /// <param name="playerNum"></param>
    /// <returns></returns>
    public bool PlayerScore(float score, int playerNum)
    {
        // Increment the scores based on the Player number.
        if (playerNum == 1)
        {
            PlayerOneScore(score);
            return true;
        }
        else if (playerNum == 2) 
        {
            PlayerTwoScore(score);
            return true;
        }

        // Return false if the playerNumber does not match 1 or 2.
        return false;
    }

    /// <summary>
    /// Method which increments Player 1's score.
    /// </summary>
    /// <param name="Score"> The score as a float to add to the total. </param>
    private void PlayerOneScore(float Score)
    {
        Player1Score += Score; 
        // Debug.Log("Player 1 has" + Player1Score);
        UIManager.Instance.CallP1Score(Player1Score);
    }

    /// <summary>
    /// Method which increments Player 2's score.
    /// </summary>
    /// <param name="Score"> The score as a float to add to the total. </param>
    private void PlayerTwoScore(float Score)
    {
        Player2Score += Score;
        // Debug.Log("Player 2 has" + Player2Score);
        UIManager.Instance.CallP2Score(Player2Score);
    }
}

