using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Score Manager is a class which keeps track of the Score of the players and may hold other stats.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// ScoreManager singleton instance.
    /// </summary>
    public static ScoreManager Instance { get; private set; }

    /// <summary>
    /// Event which declares the end of the game and which returns an int representing the player who won.
    /// </summary>
    public event Action<int> OnGameOver;

    /// <summary>
    /// Event which declares the start or end of a round.
    /// Returns true when round starts.
    /// Returns false when round ends.
    /// </summary>
    public event Action<bool> OnRoundChange;

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
    public float Player2Score { get; private set; }
    /// <summary>
    /// Amount of rounds P2 has won.
    /// </summary>
    public int Player2RoundsWon = 0;
    /// <summary>
    /// How long the round lasts.
    /// </summary>
    public float RoundTime = 99f;

    /// <summary>
    /// The timer for the round.
    /// </summary>
    private float _roundTimer;

    /// <summary>
    /// Whether the round timer is counting down or not.
    /// </summary>
    private bool _roundTimerActive = true;

    /// <summary>
    /// Timer for each round.
    /// </summary>
    private float EndOfRoundTimer;

    /// <summary>
    /// Amound of time the game stays active between rounds.
    /// </summary>
    public float EndOfRoundTime = 5f;

    /// <summary>
    /// Whether the game is in between rounds.
    /// </summary>
    private bool _endOfRoundTimerActive = false;

    /// <summary>
    /// Bool which represents whether the game is over or not.
    /// </summary>
    private bool _isGameOver = false;

    /// <summary>
    /// Int representing the winner.
    /// </summary>
    private int _gameWinner = 0;

    private void Awake()
    {
        InitSingleton();
    }

    /// <summary>
    /// Method which verifies the instantiation of the ScoreManager singleton.
    /// </summary>
    private void InitSingleton()
    {
        // If the singleton exists and it's not this gameObject, then remove this gameObject.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Otherwise set it to be this.
        Instance = this;
    }

    /// <summary>
    /// Method which runs when the gameObject is enabled.
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to OnGameOver event.
        OnGameOver += GameOver;
    }

    /// <summary>
    /// Methid which runs when the gameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from OnGameOver event.
        OnGameOver -= GameOver;
    }

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
        if (_roundTimerActive)
        {
            TimerCountdown();
        }
        if (_endOfRoundTimerActive)
        {
            Intermission();
        }
    }


    /// <summary>
    /// Method which runs before the first Update frame for ScoreManager.
    /// </summary>
    private void Start()
    {
        StartHelper();
    }

    /// <summary>
    /// Helper method for the start function.
    /// </summary>
    private void StartHelper()
    {
        _roundTimer = RoundTime;
    }

    private void TimerCountdown()
    {
        _roundTimer -= Time.deltaTime;
        UIManager.Instance.CallTimerUpdate((int)_roundTimer);

        if (_roundTimer <= 0f)
        {
            EndOfRoundSequence();
        }
    }
    /// <summary>
    /// Function called to transition between rounds.
    /// </summary>
    private void EndOfRoundSequence()
    {
        //Set the bools to make sure it's in between rounds.
        _roundTimerActive = false;
        _endOfRoundTimerActive = true;

        Debug.Log("End of round vals before check ~ Player1wins: " + Player1RoundsWon);

        _roundTimer = RoundTime;
        //Set the PlayerNum to who has the most points in a round, and add a round won to the winner.
        int playerNum = 0;
        if (Player1Score > Player2Score)
        {
            Player1RoundsWon += 1;
            playerNum = 1;
        }
        else if (Player1Score < Player2Score)
        {
            Player2RoundsWon += 1;
            playerNum = 2;
        }
        else if (Player1Score == Player2Score)
        {
            Player1RoundsWon += 1;
            Player2RoundsWon += 1;
            playerNum = 0;
        }

        Debug.Log("End of round vals after check ~ Player1wins: " + Player1RoundsWon);


        // Round is over method.
        RoundOver(playerNum);

        //check if someone won 2 rounds
        if (
            (Player1RoundsWon > 1 && Player1RoundsWon > Player2RoundsWon)
            ||
            (Player2RoundsWon > 1 && Player2RoundsWon > Player1RoundsWon)
            ||
            (Player1RoundsWon == 2 && Player2RoundsWon == 2)
            )
        {
            Debug.Log("Someone won apparently");

            OnGameOver?.Invoke(playerNum);
        }
        
        
    }

    /// <summary>
    /// Method which runs after a round ends.
    /// </summary>
    private void Intermission()
    {
        EndOfRoundTimer -= Time.deltaTime;
        if (EndOfRoundTimer <= 0)
        {
            Debug.Log("Intermission ending, isgameover?: " + _isGameOver);

            // If the game ended.
            if (_isGameOver)
            {
                GameOverSceneChange(_gameWinner);
            }
            // Otherwise, start the next round.
            else
            {
                // Start the next round where the input is the current round number.
                NextRound(Player1RoundsWon + Player2RoundsWon + 1);
            }
        }
    }

    /// <summary>
    /// Method called when a round ends.
    /// </summary>
    /// <param name="roundWinner"> The winner of the previous round. </param>
    private void RoundOver(int roundWinner)
    {
        // UI declares the winner.
        UIManager.Instance.DeclareRoundWinner(roundWinner);

        // Inform others of the end of a round.
        OnRoundChange?.Invoke(false);
    }

    /// <summary>
    /// Method which starts the next round.
    /// </summary>
    private void NextRound(int roundNum)
    {
        // Clear the UI
        UIManager.Instance.ClearWinnerUI();

        // Reset the timer.
        EndOfRoundTimer = EndOfRoundTime;

        // Start round timer
        _roundTimerActive = true;

        // End intermission timer
        _endOfRoundTimerActive = false;

        // Inform others that the next round has begun.
        OnRoundChange?.Invoke(true);
    }

    /// <summary>
    /// Method which runs on GameOver.
    /// </summary>
    private void GameOver(int winnerNum)
    {
        // Game is over, set the bool indicating that.
        _isGameOver = true;

        // Set the winner number.
        _gameWinner = winnerNum;

        // Inform the UIManager of this.
        UIManager.Instance.DeclareGameWinner(winnerNum);
    }

    /// <summary>
    /// Method which changes the scene to the End Scene when the game ends and the intermission timer runs.
    /// </summary>
    private void GameOverSceneChange(int winnerNum)
    {
        // Create the EndSceneManager gameObject.
        GameObject endSceneManager = ArenaManager.Instance.GetEndSceneManager();

        // Set the Winner
        Winner winner;
        if (TryGetComponent(out winner))
        {
            winner.SetWinner(winnerNum);
        }

        // Change scene to the Ending Scene.
        SceneManager.LoadScene(2);
    }
}

