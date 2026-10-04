using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI TestTextUI;
    [SerializeField] private TextMeshProUGUI P1Score;
    [SerializeField] private TextMeshProUGUI P2Score;
    [SerializeField] private TextMeshProUGUI RoundWinner;
    [SerializeField] private TextMeshProUGUI TimerDisplay;
    //[SerializeField] private Text _scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TestUIUpdate(string text)
    {
        TestTextUI.text = text;
    }
    void Start(){
        
        CallP1Score(0);
        CallP2Score(0);
    }

    public void CallP1Score(float Score){
        P1Score.text = "Player One Score: " + Score;
    }

    public void CallP2Score(float Score){

        P2Score.text = "Player Two Score: " + Score;
    }
    public void CallTimerUpdate(int timer)
    {
        TimerDisplay.text = timer.ToString();
    }
    public void DeclareWinner(int PlayerNum)
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
    /// <summary>
    /// Clear the UI to start a new round.
    /// </summary>
    public void ClearWinnerUI()
    {
        //RoundWinner.gameObject.SetActive(false);
        RoundWinner.text = "";
    }
}
