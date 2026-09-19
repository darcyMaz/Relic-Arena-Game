using UnityEngine;
using System.Collections.Generic;
using System;

public class ArenaManager : MonoBehaviour
{
    /// <summary>
    /// Event called when the number of buried relics is lower than the maximum.
    /// </summary>
    public event Action OnLackingBuriedRelics;

    /// <summary>
    /// Rows in the arena grid. The grid represents where relics are buried.
    /// </summary>
    [SerializeField] private int GridRows = 10;

    /// <summary>
    /// Columns in the arena grid. The grid represents where relics are buried.
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
    [SerializeField] private float EffectRelicSOSpawnRate = 0.1666f;

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
            if (RelicSO.GetEffect() == Effect.None)
            {
                _relicSOs.Add(RelicSO);
            }
            else
            {
                _relicSOsEffect.Add(RelicSO);
            }
        }
    }

    /// <summary>
    /// Instantiate a BuriedRelic, set its Relic to a randomly chosen one, and then bury it at a grid position.
    /// </summary>
    private void BuryRelicSO()
    {

    }

    /// <summary>
    /// Randomly select a RelicSO from the lists of RelicSOs.
    /// </summary>
    /// <returns> A RelicSO at random. </returns>
    private RelicSO GetRandomRelicSO()
    {
        return null;
    }

    /// <summary>
    /// Instantiate and return a BuriedRelic GameObject. 
    /// </summary>
    /// <param name="relicSO"> The RelicSO whose data will create the BuriedRelic. </param>
    /// <returns> A GameObject of the BuriedRelic prefab. </returns>
    private GameObject InstantiateBuriedRelic(RelicSO relicSO)
    {
        return null;
    }

    private void Start()
    {

        /*
        // TO-DO: Change this such that the code can use a maximum range to see if there are any RelicSOs nearby (so RelicSOs don't spawn in too close).
        // I can use or create a comparator for this. Go through all other positions, O(n).
        for (int RelicSOIndex = 0; RelicSOIndex < 10; RelicSOIndex++)
        {
            int RelicSOAttempts = 0;
            int maxRelicSOAttempts = 20;

            // Bury 10 RelicSOs.
            for (; RelicSOAttempts < maxRelicSOAttempts; RelicSOAttempts++)
            {
                // Check if we are randomly generating duplicates.
                // 20 attempts, and if we somehow generate 20 duplicates attempting to bury 1 RelicSO, we'll throw an error.
                Vector2 potentialRelicSOPosition = GetRandomPosition();
                if (_RelicSOTree.Search( potentialRelicSOPosition ))
                {
                    continue;
                }
                break;
            }
            
            if (maxRelicSOAttempts == 20)
            {
                Debug.LogError("The ArenaManager treid to bury a RelicSO but it generated duplicate coordinates (tried to bury a RelicSO on top of another) 20 times in a row. There is likely a flaw in the code logic.");
            }

        }
        */

    }

    /*
    private Vector2 GetRandomPosition()
    {
        // This function creates a random coordinate inside of the 2D arena.
        // It finds a random x and y position by taking the x and y positions and adding/subtracting half of the scale to get a range.
        float randX = UnityEngine.Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2));
        float randY = UnityEngine.Random.Range(transform.position.y - (transform.localScale.y / 2), transform.position.y + (transform.localScale.y / 2));

        return new Vector2(randX, randY);
    }
    */


    

    

    /*
    private RelicSO RandomlyChooseRelicSO()
    {
        // There is certainly a more concise way to do this random choice of Item type.
        float randResult = UnityEngine.Random.Range(0,1);
        if (randResult >= 0 && randResult < EffectRelicSOSpawnRate)
        {
            //return _effectRelicSOs[UnityEngine.Random.Range(0,_effectRelicSOs.Length)];
        }
        else if (randResult >= EffectRelicSOSpawnRate && randResult < RelicSOSpawnRate)
        {
            //return _RelicSOs[UnityEngine.Random.Range(0,_RelicSOs.Length)];
        }
        else if (randResult >= RelicSOSpawnRate || randResult <= 1)
        {
            Debug.Log("ArenaManager.BurRelicSO() tried to randomly choose what kind of Item to spawn, but the random number did not fit the code logic: " + randResult);
            return null;
        }
        else
        {
            Debug.Log("ArenaManager.BurRelicSO() tried to randomly choose what kind of Item to spawn, but the random number did not fit the code logic: " + randResult);
            return null;
        }
    }
    */

}
