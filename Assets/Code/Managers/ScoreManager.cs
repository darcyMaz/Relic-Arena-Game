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
    /// How long the round lasts.
    /// </summary>
    public float RoundTimer = 99f;
    /// <summary>
    /// Whether the round timer is counting down or not.
    /// </summary>
    private bool RoundTimerActive = true;
    /// <summary>
    /// Amound of time the game stays active between rounds.
    /// </summary>
    public float EndOfRoundTimer = 5f;
    /// <summary>
    /// Whether the game is in between rounds.
    /// </summary>
    private bool EndOfRoundTimerActive = false;

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
    
    /// <summary>
    /// Function that launches the player win screen between rounds.
    /// </summary>
    /// <param name="PlayerNum">The number of the player that won the round.</param>

    private void Update()
    {
        if (RoundTimerActive)
        {
            TimerCountdown();
        }
        if (EndOfRoundTimerActive)
        {
            Intermission();
        }
    }

    private void TimerCountdown()
    {
        RoundTimer -= Time.deltaTime;
        UIManager.Instance.CallTimerUpdate((int)RoundTimer);
        if (RoundTimer < 0f)
        {
            EndOfRoundSequence();
        }
    }
    /// <summary>
    /// Function called to transition between rounds.
    /// </summary>
    private void EndOfRoundSequence()
    {
        RoundTimer = 0f;
        //Set the PlayerNum to who has the most points in a round.
        int PlayerNum = 0;
        if (Player1Score > Player2Score)
        {
            PlayerNum = 1;
        }
        else if (Player1Score < Player2Score)
        {
            PlayerNum = 2;
        }
        else if (Player1Score == Player2Score)
        {
            PlayerNum = 0;
        }
        UIManager.Instance.DeclareWinner(PlayerNum);
        //Set the bools to make sure it's in between rounds.
        RoundTimerActive = false;
        EndOfRoundTimerActive = true;
    }

    private void Intermission()
    {
        EndOfRoundTimer -= Time.deltaTime;
        if (EndOfRoundTimer > 0)
        {
            UIManager.Instance.ClearWinnerUI();
            //Round restart function (probably in Arena Manager)
        }
    }
}

