using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //[SerializeField] private TextMeshProUGUI TestTextUI;
    [SerializeField] private TextMeshProUGUI P1Rounds;
    [SerializeField] private TextMeshProUGUI P1Score;

    [SerializeField] private TextMeshProUGUI P2Rounds;
    [SerializeField] private TextMeshProUGUI P2Score;

    [SerializeField] private TextMeshProUGUI RoundWinner;
    [SerializeField] private TextMeshProUGUI TimerDisplay;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        
        CallP1Score(0,0);
        CallP2Score(0,0);
    }

    public void CallP1Score(float Score, int Rounds)
    {
        P1Rounds.text = "Rounds: " + Rounds;
        P1Score.text = "Score: " + Score;
    }

    public void CallP2Score(float Score, int Rounds)
    {
        P2Rounds.text = "Rounds: " + Rounds;
        P2Score.text = "Score: " + Score;
    }
    public void CallTimerUpdate(int timer)
    {
        TimerDisplay.text = timer.ToString();
    }
    public void DeclareRoundWinner(int PlayerNum)
    {
        //RoundWinner.gameObject.SetActive(true);
        if (PlayerNum == 0)
        {
            RoundWinner.text = "Draw!";
        }
        else
        {
            RoundWinner.text = "Player " + PlayerNum + " wins!";
        }   
     }
    public void DeclareGameWinner(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            RoundWinner.text = "Two winners?! Ugh, just go home...";
        }
        else
        {
            RoundWinner.text = "Player " + PlayerNum + " is the best there is!";
        }
    }
    /// <summary>
    /// Clear the UI to start a new round.
    /// </summary>
    public void ClearWinnerUI()
    {
        //RoundWinner.gameObject.SetActive(false);
        RoundWinner.text = "";
    }
}
