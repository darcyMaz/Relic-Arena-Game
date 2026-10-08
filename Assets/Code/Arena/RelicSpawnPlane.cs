using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A singleton class which manages the arena.
/// To be precise, it spawns in buried relics for the players to find.
/// </summary>
[RequireComponent (typeof(Renderer))]
public class RelicSpawnPlane : MonoBehaviour
{
    /// <summary>
    /// Rows in the arena grid. The grid represents where relics are buried.
    /// Where the Row/X coordinate starts counting from the top at 1.
    /// </summary>
    [SerializeField] private int GridRows = 10;

    /// <summary>
    /// Columns in the arena grid. The grid represents where relics are buried.
    /// Where the Column/Z coordinate starts counting from the left at 1.
    /// </summary>
    [SerializeField] private int GridColumns = 10;

    /// <summary>
    /// Maximum number of Relics that can be buried at a time.
    /// </summary>
    [SerializeField] private int MaxRelicsBuried = 10;

    /// <summary>
    /// Buried relics in a dictionary mapping grid position to buried relics.
    /// </summary>
    private Dictionary<Vector2, BuriedRelic> _buriedRelics = new Dictionary<Vector2, BuriedRelic>();

    /// <summary>
    /// Prefab of a BuriedRelic.
    /// </summary>
    [SerializeField] private GameObject BuriedRelicPrefab;

    /// <summary>
    /// The arena's renderer. So that its size can be understood.
    /// </summary>
    private Renderer _renderer;

    /// <summary>
    /// Bool which indicates whether the ArenaManager instance has not been found OnEnable.
    /// </summary>
    private bool _arenaInstanceNotFound = false;

    /// <summary>
    /// Function called before the game starts.
    /// </summary>
    private void Awake()
    {
        InitAwake();
    }

    /// <summary>
    /// Initialization in the Awake function.
    /// </summary>
    private void InitAwake()
    {
        // Checks to see if the plane's arena size is valid.
        CheckArenaSize();

        // Gets the renderer.
        _renderer = GetComponent<Renderer>();   
    }

    private void OnEnable()
    {
        EventSubscriptions();
    }

    private void OnDisable()
    {
        EventUnsubscriptions();
    }

    private void EventSubscriptions()
    {
        if (ArenaManager.Instance != null)
        {
            ArenaManager.Instance.OnGameActivation += GameActivation;
        }
        else
        {
            _arenaInstanceNotFound = true;
        }
    }

    private void EventUnsubscriptions()
    {
        ArenaManager.Instance.OnGameActivation -= GameActivation;
    }

    private void Start()
    {
        if (_arenaInstanceNotFound)
        {
            ArenaManager.Instance.OnGameActivation += GameActivation;
        }
    }

    /// <summary>
    /// Function that ensures the size of the arena in terms of where relics can be buried is not less than the Maximum Number of buried relics at a time.
    /// </summary>
    private void CheckArenaSize()
    {
        // If there are fewer spots for relics than the max number allowed, lower the number allowed to bury.
        MaxRelicsBuried = (GridColumns * GridRows < MaxRelicsBuried) ? GridColumns * GridRows : MaxRelicsBuried;
    }

    /// <summary>
    /// Instantiate a BuriedRelic, set its Relic to a randomly chosen one, and then bury it at a grid position.
    /// </summary>
    public async void BuryRelic(RelicSO relicToBury, int delay)
    {
        // Generate a random coordinate position for the relic.
        Vector2 randomCoord = GenerateCoords();
        if (randomCoord.x == int.MinValue)
        {
            Debug.Log("The RelicSpawnPlane tried to bury a relic but there were no available arena coordinates to bury it in.");
            return;
        }
        // Translate the coordinates to a position in world space.
        Vector3 relicPosition = CoordToPosition(randomCoord);

        // Clone a new BuriedRelic and give it a random RelicSO.
        GameObject buriedRelicClone = InstantiateBuriedRelic(relicToBury, relicPosition, randomCoord);

        // Deactivate the GameObject so that players can't interact with it.
        buriedRelicClone.SetActive(false);

        // Finally, add the BuriedRelic to the list.
        AddBuriedRelicToList(buriedRelicClone, randomCoord);

        // Wait before activating the buried relic.
        await Task.Delay(delay);

        // Activate the BuriedRelic after a delay.
        buriedRelicClone.SetActive(true);
    }

    /// <summary>
    /// Method which adds a buried relic to the Dictionary of buried relics.
    /// </summary>
    /// <param name="goBuriedRelic"> The GameObject representing the buried relic at hand. </param>
    /// <param name="coords"> The Vector2 coordinates that are mapped to buried relics. </param>
    private void AddBuriedRelicToList(GameObject goBuriedRelic, Vector2 coords)
    {
        BuriedRelic buriedRelic;
        if (goBuriedRelic.TryGetComponent(out buriedRelic))
        {
            _buriedRelics.Add(coords, buriedRelic);
        }
        else
        {
            Debug.Log("The RelicSpawnPlane tried to add a BuriedRelic to its respective dictionary. However, the cloned gameObject did not have the component.");
        }
    }

    /// <summary>
    /// Instantiate and return a BuriedRelic GameObject. 
    /// Set its relic data before returning it.
    /// </summary>
    /// <param name="relicSO"> The RelicSO whose data will create the BuriedRelic. </param>
    /// <returns> A GameObject of the BuriedRelic prefab. </returns>
    private GameObject InstantiateBuriedRelic(RelicSO relicSO, Vector3 position, Vector2 coords)
    {
        // Instantiate the clone.
        GameObject buriedRelicObj = Instantiate(BuriedRelicPrefab, position, Quaternion.identity);

        // Change the RelicSO data from the default to the input.
        // Subscribe to its OnDestroy event.
        BuriedRelic buriedRelic;
        if (buriedRelicObj.TryGetComponent(out buriedRelic))
        {
            buriedRelic.OnBuriedRelicDugUp += RemoveRelic;
            buriedRelic.SetRelicData(relicSO);
            buriedRelic.SetArenaCoords(coords);
            return buriedRelicObj;
        }
        else
        {
            throw new Exception("There was an attempt by the Arena Manager to bury a BuriedRelic but the prefab did not have the BuriedRelic component.");
        }
    }

    /// <summary>
    /// This function translate a vector2 representing a coordinate in the arena to an in-game position.
    /// </summary>
    /// <param name="coord"> The coordinate to translate. </param>
    private Vector3 CoordToPosition(Vector2 coord)
    {
        // Get the center of the arena.
        Vector3 center = transform.position;

        // Get the size of the arena.
        Vector3 size = _renderer.bounds.size;

        // Build the position vector.
        Vector3 position = new Vector3(GetXPosFromCoord((int)coord.x, size.x), GetZPosFromCoord((int)coord.y, size.y), transform.position.z);

        // Return the coordinate also adding the transform.position as the derived coordinate is a local position.
        return position;
    }

    /// <summary>
    /// Return the local x position of the row coordinate.
    /// </summary>
    /// <param name="coord"> The x coordinate. </param>
    /// <param name="xLength"> The vertical length of the arena. </param>
    /// <returns></returns>
    private float GetXPosFromCoord(int coord, float xLength)
    {
        float cut = xLength / (GridColumns + 1);

        float topOfColumn = transform.position.x + (xLength/2);

        //Debug.Log(topOfColumn - (cut * coord));
        return topOfColumn - (cut * coord);
    }

    /// <summary>
    /// Return the local z position of the grid coordinate.
    /// </summary>
    /// <param name="coord"> The z coordinate. </param>
    /// <param name="zLength"> The horizontal length of the arena. </param>
    /// <returns></returns>
    private float GetZPosFromCoord(int coord, float zLength)
    {
        float cut = zLength / (GridRows + 1);

        float leftOfRow = transform.position.y + (zLength / 2);

        //Debug.Log("GetZPos() ~ leftOfRow var: " + (leftOfRow - (cut * coord)) + " trans.pos.y: " + transform.position.y);
        return leftOfRow - (cut * coord);
    }

    /// <summary>
    /// Get a set of arena coordinates that are not occupied by a BuriedRelic.
    /// If the result is a Vector2 with the min integer value, then it means all coordinates are occupied.
    /// </summary>
    /// <returns> A Vector2 representing a coordinate in the arena. </returns>
    private Vector2 GenerateCoords()
    {
        // Make a list of all coordinates which are not already occupied by BuriedRelics.
        List<Vector2> allCoords = new List<Vector2>();
        for (int i=1; i<GridRows+1; i++)
        {
            for (int j=1; j<GridColumns+1; j++)
            {
                if (!_buriedRelics.TryGetValue(new Vector2(i, j), out _))
                {
                    allCoords.Add(new Vector2(i, j));
                }
            }
        }

        if (allCoords.Count == 0)
        {
            return new Vector2(int.MinValue, int.MinValue);
        }

        // Randomly select one of those coordinates.
        return allCoords[UnityEngine.Random.Range(0, allCoords.Count)];
    }

    /// <summary>
    /// Method which removes a relic based on a given coordinate.
    /// </summary>
    /// <param name="coords"></param>
    private void RemoveRelic(Vector2 coords)
    {
        _buriedRelics.Remove(coords);
    }

    private void GameActivation(bool onOrOff)
    {
        if (!onOrOff)
        {
            // Delete all relics.
            DeleteRelics();
        }
    }

    /// <summary>
    /// Delete all buried relics.
    /// </summary>
    private void DeleteRelics()
    {
        // For each buried relic, destroy the game object.
        foreach (Vector2 relicCoord in _buriedRelics.Keys)
        {
            Destroy(_buriedRelics[relicCoord]);
        }
        // Clear the list.
        _buriedRelics.Clear();
    }

    /// <summary>
    /// Method which returns the number of buried relics.
    /// </summary>
    /// <returns> An int representing the number of buried relics. </returns>
    public int GetTotalRelicsBuried()
    {
        return _buriedRelics.Count;
    }

    /// <summary>
    /// Method which returns the maximum number of buried relics for this plane.
    /// </summary>
    /// <returns> An int representing the max number of buried relics allowed. </returns>
    public int GetBuriedRelicMax()
    {
        return MaxRelicsBuried;
    }
}
