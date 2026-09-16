using System;
using UnityEngine;

public class Player : IEffectable
{
    /// <summary>
    /// The Player's number, set in the inspector.
    /// </summary>
    [SerializeField] private int PlayerNumber = 0;

    /// <summary>
    /// The Player's Inventory.
    /// </summary>
    private Inventory _inventory;
    private bool _hasInventory = false;
    
    /// <summary>
    /// The Player's Metal Detector.
    /// </summary>
    private MetalDetector _metalDetector;
    private bool _hasMetalDetector = false;

    /// <summary>
    /// Initialize the components of the Player.
    /// </summary>
    private void Start()
    {
        StartInit();
    }

    /// <summary>
    /// Get this Player's number.
    /// </summary>
    /// <returns> The Player's number as an integer. </returns>
    public int GetPlayerNumber()
    {
        return PlayerNumber;
    }

    private void RelicHitPlayer()
    {
        // When the player gets hit by an item.
        // Need specification.
        // Does the player lose all of their items?
        // Lose (idk) 40% at random?

        throw new NotImplementedException();
    }

    /// <summary>
    /// The method holding all of the initializations that the Player component must do at the Start function. 
    /// </summary>
    private void StartInit()
    {
        // The Player has many components, try to find them and get them.
        if (TryGetComponent(out _inventory))
        {
            _hasInventory = true;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have an Inventory component. The game will still work but the player will not be able to acquire items.");
        }
        if (TryGetComponent(out _metalDetector))
        {
            _hasMetalDetector = true;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a Metal Detector component. The game will still work but the player will not be able to find items.");
        }
    }

    /// <summary>
    /// Applies the knockout effect.
    /// </summary>
    /// <param name="knockoutTime"> Amount of time to be knocked out. </param>
    protected override void ApplyKnockout(float knockoutTime)
    {
        Debug.Log("Knockout Effect not implemented onto the Player.");
    }

    protected override void ApplyChangeSpeed(float speed, float duration)
    {
        Debug.Log("ChangeSpeed Effect nto implemented by Player.");
    }

    protected override void ApplyLightning()
    {
        Debug.Log("A Player was struck by lightning but the effect is not applied to the Player yet.");
    }
}
