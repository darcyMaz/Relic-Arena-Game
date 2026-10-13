 using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of EndSceneUI.
    /// </summary>
    public static EndSceneManager Instance;

    /// <summary>
    /// The Winner component.
    /// </summary>
    private Winner _winner;

    private void Awake()
    {
        InitSingleton();
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
    /// Method which initializes the singleton.
    /// </summary>
    private void InitSingleton()
    {
        // If the Instance already exists and it's not this, then destroy this!
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Method which returns the winner player number.
    /// </summary>
    /// <returns> Int representing the winner. </returns>
    public int GetWinner()
    {
        return _winner.GetWinner();
    }

    /// <summary>
    /// Method which destroys the winner GameObject.
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
