using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour
{
    // look through its own children to see what relicspawnplanes it has
    // understand how many spawn points each plane will have based on their size
    //   they will have an equal density
    // spawn relics and doll them out at random based on the size of the planes

    // so im not gonna make a density var, im gonna make one var for the number of spawn points
    // and those spawn points will be distributed to the arena managers who will decide on their own how to make their grid system

    // the decision of when to spawn is in AM, the decision of which plane to spawn on is in AM, the decision of where in the plane is in RSP
    // the AM does not have a global coordinate system. Each plane has its own system.
    // so the global coord system is more like: Plane #Z -> (x,y)

    // ok wait...
    //   so how about I DON'T have the arenamanager managing the number of spawn points
    //   i leave that in the relic spawners
    //   this is the way to make the spawning more versatile

    public static ArenaManager Instance { get; private set; }

    /// <summary>
    /// List of all of the spawn planes in the ArenaManager.
    /// </summary>
    private List<RelicSpawnPlane> _relicSpawnPlanes = new List<RelicSpawnPlane>();

    /// <summary>
    /// Bool indicating whether a game is active or not.
    /// </summary>
    public bool IsGameActive { get; private set; }

    /// <summary>
    /// A public Vector3 representing the player's z axis.
    /// </summary>
    public float PlayerZPosition { get; private set; }

    /// <summary>
    /// Method called on awake.
    /// </summary>
    private void Awake()
    {
        InitSingleton();
        GetSpawnPlanes();
        PlayerZPosition = 0f;
    }

    /// <summary>
    /// Method called when this object is enabled.
    /// </summary>
    private void OnEnable()
    {
        
    }

    /// <summary>
    /// Method called when this object is disabled.
    /// </summary>
    private void OnDisable()
    {
        
    }

    /// <summary>
    /// Method called before the first frame.
    /// </summary>
    private void Start()
    {
        
    }

    /// <summary>
    /// Initialize the singleton for ArenaManager.
    /// </summary>
    private void InitSingleton()
    {
        if (Instance != null && this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Method which retrieves the RelicSpawnPlanes from the Transform's children.
    /// </summary>
    private void GetSpawnPlanes()
    {
        foreach (Transform child in transform)
        {
            RelicSpawnPlane spawnPlane;
            if (child.gameObject.TryGetComponent(out spawnPlane))
            {
                _relicSpawnPlanes.Add(spawnPlane);
            }
        }
    }

    /// <summary>
    /// Method which turns the spawning of relics on and off, removing all relics when the game is deactivated and spawning them in when turned on.
    /// This method listens to the round manager for when to stop and start.
    /// </summary>
    private void FlipGameActivation(bool isGameActive)
    {
        // Check whether the bools representing the game being active do not match.

        // Start spawning relics.
        if (isGameActive && !IsGameActive)
        {
            foreach (RelicSpawnPlane relicSpawnPlane in _relicSpawnPlanes)
            {
                // turn off spawner
            }
        }
        // Remove all relics and stop spawning more.
        else if (!isGameActive && IsGameActive)
        {
            foreach (RelicSpawnPlane relicSpawnPlane in _relicSpawnPlanes)
            {
                // turn off spawning
                // remove all relics
                // or maybe just call one func, turn off spawner
            }
        }
    }


}
