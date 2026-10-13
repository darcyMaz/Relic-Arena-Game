using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class which controls the UI of the End Scene
/// </summary>
public class EndSceneUI : MonoBehaviour
{
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
}
