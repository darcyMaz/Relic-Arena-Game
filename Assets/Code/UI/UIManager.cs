using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI TestTextUI;
    [SerializeField] private TextMeshProUGUI P1Score;
    [SerializeField] private TextMeshProUGUI P2Score;
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
}
