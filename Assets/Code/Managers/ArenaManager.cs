using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.ShaderGraph.Internal;
using System.Threading;
using System.Threading.Tasks;

public class ArenaManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of ArenaManager.
    /// </summary>
    public static ArenaManager Instance { get; private set; }

    /// <summary>
    /// Event which informs others, plane spawners in particular, whether the game is active.
    /// </summary>
    public event Action<bool> OnGameActivation;

    /// <summary>
    /// Int representing the maximum number of relics allowed to be buried at a time.
    /// </summary>
    [SerializeField] private int MaxRelicsInGame = 5;

    /// <summary>
    /// Int representing the minimum delay in milliseconds between calling the burying of relics and actually burying it.
    /// </summary>
    [SerializeField] private int MinRelicSpawnDelay = 2000;

    /// <summary>
    /// Int representing the upper bound added to the minimum spawn delay for spawning relics.
    /// The delay will be randomly chosen between the minimum and the minimum + this variable.
    /// </summary>
    [SerializeField] private int UpperBoundRelicSpawnDelay = 500;


    /// <summary>
    /// Spawn rate of Relics with effects.
    /// </summary>
    [SerializeField] private float EffectRelicSOSpawnRate = 0.01f;

    /// <summary>
    /// All RelicSOs with no effect.
    /// </summary>
    private List<RelicSO> _relicSOs = new List<RelicSO>();

    /// <summary>
    /// All RelicSOs with an effect.
    /// </summary>
    private List<RelicSO> _relicSOsEffect = new List<RelicSO>();
    
    /// <summary>
    /// List of all of the spawn planes in the ArenaManager.
    /// </summary>
    private List<RelicSpawnPlane> _relicSpawnPlanes = new List<RelicSpawnPlane>();

    /// <summary>
    /// Bool indicating whether a game is active or not.
    /// </summary>
    [SerializeField] private bool IsGameActive = false;
    

    /// <summary>
    /// Method called on awake.
    /// </summary>
    private void Awake()
    {
        InitSingleton();
        GetSpawnPlanes();
        GetRelicSOs();
    }

    /// <summary>
    /// Method called when this object is enabled.
    /// </summary>
    private void OnEnable()
    {
        GameActivationSubscription();
    }

    /// <summary>
    /// Method called when this object is disabled.
    /// </summary>
    private void OnDisable()
    {
        GameActivationUnsubscribe();
    }

    /// <summary>
    /// Method called before the first frame.
    /// </summary>
    private void Start()
    {
        
    }

    /// <summary>
    /// Method which plays once on every frame.
    /// Checks for the need to bury relics and does so.
    /// </summary>
    private void Update()
    {
        BuryRelics();
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

    private void BuryRelics()
    {
        // Count how many relics have been buried.
        int totalBuried = 0;
        foreach (RelicSpawnPlane plane in _relicSpawnPlanes)
        {
            totalBuried += plane.GetTotalRelicsBuried();
        }

        // For each missing relic to bury, bury it.
        for (; totalBuried < MaxRelicsInGame; totalBuried++)
        {
            // Plane chosen at random, weighted by the relics currently buried at each plane.
            RelicSpawnPlane plane = ChooseRandomPlane();

            // Get a random relicSO.
            RelicSO relic = GetRandomRelicSO();

            // Bury a relic there after a delay.
            BuryRelic(plane, relic, GetRandomDelay());
        }
    }

    /// <summary>
    /// Method which subscribes to an event that turns the game on and off.
    /// </summary>
    private void GameActivationSubscription()
    {
        // LISTEN TO SOMETHING THAT TURNS ROUNDS ON AND OFF
    }

    /// <summary>
    /// Method which unsubscribes to an event that turns the game on and off.
    /// </summary>
    private void GameActivationUnsubscribe()
    {
        // Unsubscribe to something that turns rtounds on and off.
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
            if (RelicSO.GetOtherSystemSO() != null)
            {
                _relicSOsEffect.Add(RelicSO);
                continue;
            }

            // If this relic has at least one Effect that is not the None effect.
            int notNoneEffectCount = 0;
            foreach (Effect effect in RelicSO.GetPassiveEffects())
            {
                if (effect != Effect.None)
                {
                    notNoneEffectCount++;
                }
            }

            // 
            if (notNoneEffectCount > 0)
            {
                _relicSOsEffect.Add(RelicSO);
            }
            else
            {
                _relicSOs.Add(RelicSO);
            }
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
    /// Method which randomly chooses a spawn plane weighted by: 
    /// The difference between that plane's max # of relics allowed and the number buried there, divided by the sum of that difference for all planes.
    /// </summary>
    private RelicSpawnPlane ChooseRandomPlane()
    {
        // Get the sum of all the difference as described in the summary.
        float diffSum = 0;

        // Go through each plane.
        foreach (RelicSpawnPlane plane in _relicSpawnPlanes)
        {
            diffSum += plane.GetBuriedRelicMax() - plane.GetTotalRelicsBuried();
        }

        // Generate the random float between 0 and 1.
        float rand = UnityEngine.Random.Range(0f, 1f);

        // Debug.Log("Choosing random plane rand val: " + rand);


        // The sum of the previous odds as the list below progresses.
        float prevOdds = 0;

        // Go through the list again, checking to see if the random choice corresponds to that plane.
        foreach (RelicSpawnPlane plane in _relicSpawnPlanes)
        {
            // The odds of this plane being chosen.
            float odds = (plane.GetBuriedRelicMax() - plane.GetTotalRelicsBuried()) / diffSum;

            // Debug.Log("Choosing random plane loop ~ odds: " + odds + " prevOdds:" + prevOdds);

            // Is this plane within the range of the odds?
            if (rand <= prevOdds + odds)
            {
                return plane;
            }

            prevOdds += odds;
        }

        Debug.LogError("There was an attempt to choose a random RelicSpawnPlane in ArenaManager, but the logic was flawed and led to no plane being chosen.");
        return null;
    }

    /// <summary>
    /// Method which buries a relic in a given plane after a given delay.
    /// </summary>
    /// <param name="plane"> A RelicSpawnPlane where the relic will be buried. </param>
    /// <param name="relic"> A RelicSO which holds data on the relic. </param>
    /// <param name="delay"> An int delay between calling the act to bury and the actual burying of the relic. </param>
    private void BuryRelic(RelicSpawnPlane plane, RelicSO relic, int delay)
    {
        if (IsGameActive)
        {
            plane.BuryRelic(relic, delay);
        }
    }

    private int GetRandomDelay()
    {
        return UnityEngine.Random.Range(MinRelicSpawnDelay, MinRelicSpawnDelay + UpperBoundRelicSpawnDelay);
    }

    /// <summary>
    /// Method which turns the spawning of relics on and off, removing all relics when the game is deactivated and spawning them in when turned on.
    /// This method listens to the round manager for when to stop and start.
    /// </summary>
    private void GameActivationListener(bool isGameActive)
    {
        IsGameActive = isGameActive;

        if (!IsGameActive)
        {
            OnGameActivation?.Invoke(false);
        }

        // Check whether the bools representing the game being active do not match.
        /*
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
        */
    }


}
