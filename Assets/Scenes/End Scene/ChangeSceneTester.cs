using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneTester : MonoBehaviour
{
    public void ChangeSceneTest(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
