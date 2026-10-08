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
    /// Amount of rounds P1 has won.
    /// </summary>
    public int Player1RoundsWon = 0;
    /// <summary>
    /// Player 2's score.
    /// </summary>
    public float Player2Score {get; private set; }
    /// <summary>
    /// Amount of rounds P2 has won.
    /// </summary>
    public int Player2RoundsWon = 0;
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
        UIManager.Instance.CallP1Score(Player1Score,Player1RoundsWon);
    }

    /// <summary>
    /// Method which increments Player 2's score.
    /// </summary>
    /// <param name="Score"> The score as a float to add to the total. </param>
    private void PlayerTwoScore(float Score)
    {
        Player2Score += Score;
        // Debug.Log("Player 2 has" + Player2Score);
        UIManager.Instance.CallP2Score(Player2Score,Player2RoundsWon);
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
        if (RoundTimer <= 0f)
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
        //Set the PlayerNum to who has the most points in a round, and add a round won to the winner.
        int PlayerNum = 0;
        if (Player1Score > Player2Score)
        {
            Player1RoundsWon += 1;
            PlayerNum = 1;
        }
        else if (Player1Score < Player2Score)
        {
            Player2RoundsWon += 1;
            PlayerNum = 2;
        }
        else if (Player1Score == Player2Score)
        {
            Player1RoundsWon += 1;
            Player2RoundsWon += 1;
            PlayerNum = 0;
        }
        //check if someone won 2 rounds
        if ((Player1RoundsWon > 1 && Player1RoundsWon > Player2RoundsWon)||
            (Player2RoundsWon > 1 && Player2RoundsWon > Player1RoundsWon)||
            (Player1RoundsWon == 2 && Player2RoundsWon == 2))
        {
            Debug.Log("Game Over");
            UIManager.Instance.DeclareGameWinner(PlayerNum);
        }
        else
        {
            Debug.Log("Round over");
            UIManager.Instance.DeclareRoundWinner(PlayerNum);
        }
        
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

