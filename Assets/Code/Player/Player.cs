using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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
    // private bool _hasMetalDetector = false;

    private InputSystem_Actions _actions;
    private InputAction _dig;

    private bool _isDigging = false;
    private bool _isRelicFound = false;
    private BuriedRelic _buriedRelicFound;
    private Relic _relicFound;

    /// <summary>
    /// Intizalize before start.
    /// </summary>
    private void Awake()
    {
        AwakeInit();
    }

    /// <summary>
    /// When this gameObject is Enabled.
    /// </summary>
    private void OnEnable()
    {
        // I AM AWARE that this is not a good way to do this.
        if (PlayerNumber == 1)
        {
            _dig = _actions.Player1.Interact;
            Debug.Log("Player 1 found.");
        }
        else if (PlayerNumber == 2)
        {
            _dig = _actions.Player2.Interact;
        }
        else
        {
            _dig = _actions.Player.Interact;
        }

        _dig.performed += Dig;
        _dig.Enable();
    }

    /// <summary>
    /// When this gameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        _dig.Disable();
    }

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
            // _hasMetalDetector = true;
            _metalDetector.OnRelicVeryClose += AcceptRelic;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a Metal Detector component. The game will still work but the player will not be able to find items.");
        }
    }

    /// <summary>
    /// Method holding initializations for the Awake function.
    /// </summary>
    private void AwakeInit()
    {
        _actions = new InputSystem_Actions();
    }

    /// <summary>
    /// Accepts a Relic into the inventory.
    /// Runs when _metalDetector invokes an event for proximity to relic and dig is pressed.
    /// Will also play animations for picking up relics.
    /// </summary>
    private void AcceptRelic(Relic relic, BuriedRelic buriedRelic)
    {
        if (_hasInventory && _isDigging && !_isRelicFound)
        { 
            _isRelicFound = true;
            _relicFound = relic;
            _buriedRelicFound = buriedRelic;
        }
    }
    
    

    /// <summary>
    /// Consume a relic. In other words delete it.
    /// </summary>
    /// <param name="relic"> The relic to remove. </param>
    private void ConsumeRelic(Relic relic)
    {
        _inventory.RemoveItem(relic);
    }
    /// <summary>
    /// Consume a relic at an index. In other words delete it.
    /// </summary>
    /// <param name="index"> The inventory index to remove a relic. </param>
    private void ConsumeRelicAt(int index)
    {
        _inventory.RemoveItemAt(index);
    }

    private async void Dig(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDigging)
        {

            // The Player is digging.
            _isDigging = true;

            // Play the digging animation.
            ////

            // Await the duration of the animation.
            //// For now, 3 seconds.
            await Task.Delay(3000);

            // If the Player found a relic.
            if (_isRelicFound && (_relicFound != null && _buriedRelicFound != null))
            {

                // Add it to the inventory.
                _inventory.AddItem(_relicFound);

                // Play found relic animation.
                ////

                // Destroy the buried relic.
                Destroy(_buriedRelicFound.gameObject);

                // Reset relic found vars.
                _isRelicFound = false;
                _relicFound = null;
                _buriedRelicFound = null;

                foreach (var item in _inventory)
                {
                    item.GetName();
                }
            }

            // When no longer digging, set this to false.
            _isDigging = false;
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
