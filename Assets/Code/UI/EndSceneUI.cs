using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class which controls the UI of the End Scene
/// </summary>
[RequireComponent(typeof(Winner))]
public class EndSceneUI : MonoBehaviour
{
    /// <summary>
    /// The Winner component.
    /// </summary>
    private Winner _winner;
    
    /// <summary>
    /// Method which runs on the gameObject's enabling.
    /// </summary>
    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        InitWinner();
    }

    /// <summary>
    /// Method which gets the Winner component.
    /// </summary>
    private void InitWinner()
    {
        // Get the Winner component.
        _winner = GetComponent<Winner>();
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
    /// Method which destroys the winner GameObject.
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
