using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class which controls the UI of the End Scene
/// </summary>
public class EndSceneUI : MonoBehaviour
{
    /// <summary>
    /// Text on the canvas displaying a winning message.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _winnerText;

    /// <summary>
    /// Method called before the first call of Update on this script.
    /// </summary>
    private void Start()
    {
        SetWinnerText();
    }

    /// <summary>
    /// Method whichs sets the text which displays the winner.
    /// </summary>
    private void SetWinnerText()
    {
        _winnerText.text += " " + EndSceneManager.Instance.GetWinner().ToString();
    }

    /// <summary>
    /// Changes the scene to the given scene index.
    /// </summary>
    /// <param name="sceneIndex"> An int representing the scene index. </param>
    public void ChangeScenePress(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Changes the scene to the given scene name.
    /// </summary>
    /// <param name="sceneName"> A string representing the scene name. </param>
    public void ChangeScenePress(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Method which quits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Method which destroys the end scene manager.
    /// </summary>
    public void DestroyESM()
    {
        EndSceneManager.Instance.DestroySelf();
    }
}
