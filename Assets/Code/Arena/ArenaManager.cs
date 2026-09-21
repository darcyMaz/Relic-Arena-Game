using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// A singleton class which manages the arena.
/// To be precise, it spawns in buried relics for the players to find.
/// </summary>
[RequireComponent (typeof(Renderer))]
public class ArenaManager : MonoBehaviour
{
    /// <summary>
    /// Boolean that determines whether relics will be buried or whether the game is done.
    /// </summary>
    [SerializeField] private bool IsGameActive = true;

    /// <summary>
    /// Event called when the number of buried relics is lower than the maximum.
    /// </summary>
    // public event Action OnLackingBuriedRelics;

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
    /// All RelicSOs with no effect.
    /// </summary>
    private List<RelicSO> _relicSOs = new List<RelicSO>();

    /// <summary>
    /// All RelicSOs with an effect.
    /// </summary>
    private List<RelicSO> _relicSOsEffect = new List<RelicSO>();

    /// <summary>
    /// Spawn rate of Relics with effects.
    /// </summary>
    [SerializeField] private float EffectRelicSOSpawnRate = 0.01f;

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
    /// The shop gameObject so that its size can be understood.
    /// </summary>
    [SerializeField] private GameObject _shop;

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
        GetRelicSOs();
        CheckArenaSize();
        _renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Runs every frame. It checks to see whether relics need to be buried.
    /// </summary>
    private void Update()
    {
        // Check whether the number of relics is too low.
        if (_buriedRelics.Count < MaxRelicsBuried && IsGameActive)
        {
            BuryRelic();
        }
    }

    /// <summary>
    /// Get all RelicSOs from the resource folder.
    /// </summary>
    private void GetRelicSOs()
    {
        // Grab the RelicSOs from the resources folder.
        RelicSO[] RelicSOs = Resources.LoadAll<RelicSO>("RelicSOs");

        // Organize them into Effect and No Effect lists.
        foreach (RelicSO RelicSO in RelicSOs)
        {
            

            /*
            if (RelicSO.GetPassiveEffects() == 0)
            {
                _relicSOs.Add(RelicSO);
            }
            else
            {
                _relicSOsEffect.Add(RelicSO);
            }
             */

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
    private void BuryRelic()
    {
        // Generate a random coordinate position for the relic.
        Vector2 randomCoord = GenerateCoords();
        if (randomCoord.x == int.MinValue)
        {
            Debug.Log("The ArenaManager tried to bury a relic but there were no available arena coordinates to bury it in.");
            return;
        }

        ////Debug.Log("Coord successfully generated: " + randomCoord);

        // Translate the coordinates to a position in world space.
        Vector3 relicPosition = CoordToPosition(randomCoord);

        //Debug.Log("World position of generated coord: " + (relicPosition+transform.position));

        // Clone a new BuriedRelic and give it a random RelicSO.
        GameObject buriedRelicClone = InstantiateBuriedRelic(GetRandomRelicSO(), relicPosition, randomCoord);

        // Finally, add the BuriedRelic to the list.
        AddBuriedRelicToList(buriedRelicClone, randomCoord);
    }

    private void AddBuriedRelicToList(GameObject goBuriedRelic, Vector2 coords)
    {
        BuriedRelic buriedRelic;
        if (goBuriedRelic.TryGetComponent(out buriedRelic))
        {
            _buriedRelics.Add(coords, buriedRelic);
        }
        else
        {
            Debug.Log("The ArenaManager tried to add a BuriedRelic to its respective dictionary. However, the cloned gameObject did not have the component.");
        }
    }

    /// <summary>
    /// Randomly select a RelicSO from the lists of RelicSOs.
    /// </summary>
    /// <returns> A RelicSO at random. </returns>
    private RelicSO GetRandomRelicSO()
    {
        // Randomly enerate a float in the range from 0 to 1.
        float rand = UnityEngine.Random.Range(0f, 1f);
        int randIndex;

        // If the float is higher than the effect relic spawn rate, return a normal relic.
        if (rand > EffectRelicSOSpawnRate)
        {
            randIndex = UnityEngine.Random.Range(0, _relicSOs.Count);
            return _relicSOs[randIndex];
        }
        // If the float is lower, return an effect relic.
        else
        {
            randIndex = UnityEngine.Random.Range(0, _relicSOsEffect.Count);
            return _relicSOsEffect[randIndex];
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

        //Debug.Log("Renderer size: " + size);

        // Build the position vector.
        Vector3 position = new Vector3(GetXPosFromCoord((int)coord.x, size.x), GetZPosFromCoord((int)coord.y, size.y), -5);

        //Debug.Log("Local position of generated coord: " + position);

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
    /// This function checks if a coordinate is inside of the shop.
    /// </summary>
    /// <returns></returns>
    private bool IsCoordInShop()
    {
        // Access the shops transform
        // compare the coordination's position to the range representing the shop's area
        // return

        return false;
    }

    private void RemoveRelic(Vector2 coords)
    {
        _buriedRelics.Remove(coords);
    }
}
