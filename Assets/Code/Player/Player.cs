using System;
using UnityEngine;

public class Player : IEffectable
{
    [SerializeField] private int PlayerNumber = 0;

    // The Player is composed of the following components.
    private Inventory _inventory;
    private bool _hasInventory = false;
    
    private MetalDetector _metalDetector;
    private bool _hasMetalDetector = false;

    private Rigidbody2D _rigidBody;
    private bool _hasRigidBody = false;

    private void Start()
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
        if (TryGetComponent(out _rigidBody))
        {
            _hasRigidBody = true;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a Rigidbody2D component. The game will still work but the player will not be able to move.");
        }
    }

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
}
