using UnityEngine;
using System.Collections.Generic;
using System;

public class ArenaManager : MonoBehaviour
{

    // the arena manager's first purpose is to spawn in RelicSOs at random and communicate their position for anyone to hear
    // So the arena needs a coordinate system and access to the RelicSOs themselves
    // I'll need to load in the RelicSOs from here or have a seperate class which does that and ArenaManager quickly grabs them.

    public event Action OnLackingBuriedRelicSOs;

    private SpriteRenderer _spriteRenderer;
    private bool _hasSR = false;


    // private EffectRelicSO[] _effectRelicSOs;
    // private RelicSO[] _RelicSOs;
    private List<RelicSO> _RelicSOs;

    [SerializeField] private float EffectRelicSOSpawnRate = 0.1666f;
    [SerializeField] private float RelicSOSpawnRate = 1;

    [SerializeField] private int MaxRelicSOsBuried = 10;
    private TwoDTree _RelicSOTree = new TwoDTree();


    private void Awake()
    {
        // TO DO: Make this a singleton.


        // Load in all of the RelicSO scriptable objects.
        // Load in all of the EffectRelicSO scriptable objects.

        // EffectRelicSO[] effectRelicSOs = Resources.LoadAll<EffectRelicSO>("EffectRelicSOs");
        RelicSO[] RelicSOs = Resources.LoadAll<RelicSO>("RelicSOs");

        foreach (RelicSO RelicSO in RelicSOs)
        {

        }

        // __effectRelicSOs = new List<EffectRelicSO>(_effectRelicSOs);  // Error here!

        // TO DO: Switching from the resources folder to addressables.

        // Get values representing the bounds of the coordinate system for spawning RelicSOs.
        if (TryGetComponent(out _spriteRenderer))
        {
            _hasSR = true;
        }
        else
        {
            Debug.Log("The ArenaManager could not find its Sprite Renderer.");
        }
    }

    // This is async as burying a RelicSO could take more than one frame. Maybe not.
    private void Update()
    {
        

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

    private Vector2 GetRandomPosition()
    {
        // This function creates a random coordinate inside of the 2D arena.
        // It finds a random x and y position by taking the x and y positions and adding/subtracting half of the scale to get a range.
        float randX = UnityEngine.Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2));
        float randY = UnityEngine.Random.Range(transform.position.y - (transform.localScale.y / 2), transform.position.y + (transform.localScale.y / 2));

        return new Vector2(randX, randY);
    }



    private void BuryRelicSO(RelicSO RelicSOToBury)
    {

    }

    

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
