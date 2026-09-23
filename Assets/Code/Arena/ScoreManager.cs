using UnityEngine;

/// <summary>
/// The Score Manager is a class which keeps track of the Score of the players and may hold other stats.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public float Player1Score { get; private set; }
    public float Player2Score {get; private set; }

    public void PlayerOneScore(float Score){
            Player1Score += Score; 
        Debug.Log("Player 1 has" + Player1Score);
            UIManager.Instance.CallP1Score(Player1Score);

    }
    
public void PlayerTwoScore(float Score){
        Player2Score += Score;
        Debug.Log("Player 2 has" + Player2Score);
            UIManager.Instance.CallP2Score(Player2Score);

    }
}

