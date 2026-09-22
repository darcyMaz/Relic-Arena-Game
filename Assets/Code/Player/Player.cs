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
    /// <summary>
    /// Bool var which says whether this player has an inventory.
    /// </summary>
    private bool _hasInventory = false;
    
    /// <summary>
    /// The Player's Metal Detector.
    /// </summary>
    private MetalDetector _metalDetector;

    /// <summary>
    /// The Movement Input component attached to this Player.
    /// </summary>
    private MovementInput _movementInput;
    /// <summary>
    /// A bool variable that indicates whether a Player has a Movement Input component.
    /// </summary>
    private bool _hasMovementInput = false;

    /// <summary>
    /// Input system actions variable.
    /// </summary>
    private InputSystem_Actions _actions;

    /// <summary>
    /// The Dig input action.
    /// </summary>
    private InputAction _dig;

    /// <summary>
    /// Bool variable that is true when the player is digging.
    /// </summary>
    private bool _isDigging = false;
    /// <summary>
    /// Bool variable that is true when a relic has been found. Used for the digging mechanic.
    /// </summary>
    private bool _isRelicFound = false;
    /// <summary>
    /// BuriedRelic variable used to store buried relics that have just been found. Used for the digging mechanic. This variable will be null nearly all the time.
    /// </summary>
    private BuriedRelic _buriedRelicFound;
    /// <summary>
    /// Relic variable used to store relics that have just been found. Used for the digging mechanic. This variable will be null nearly all the time.
    /// </summary>
    private Relic _relicFound;

    /// <summary>
    /// Int variable which indicates the number of extra lives the player has.
    /// </summary>
    private int _extraLives = 0;
    /// <summary>
    /// Relic variable which holds the Relic which currently effects the extra life Effect.  
    /// </summary>
    private Relic _currentExtraLifeRelic;

    /// <summary>
    /// event which announces the amount of extra lives a player has.
    /// </summary>
    public event Action<int> OnExtraLifeChanged;

    private InputAction _getHitTest;

    /// <summary>
    /// Method which runs on awake.
    /// </summary>
    protected override void Awake()
    {
        AwakeInit();
    }

    /// <summary>
    /// Runs when this gameObject is Enabled.
    /// </summary>
    protected override void OnEnable()
    {
        OnEnableInit();
    }

    /// <summary>
    /// When this gameObject is disabled.
    /// </summary>
    protected override void OnDisable()
    {
        OnDisableInit();
    }

    /// <summary>
    /// Method which runs on Start.
    /// </summary>
    protected override void Start()
    {
        StartInit();
    }

    /// <summary>
    /// Method holding initializations for the Awake function.
    /// </summary>
    private void AwakeInit()
    {
        base.Awake();
        _actions = new InputSystem_Actions();

        OnExtraLifeChanged += ExtraLifeTest;

    }

    /// <summary>
    /// Initializations for the OnEnable function.
    /// </summary>
    private void OnEnableInit()
    {
        // Subscribe to the OnRelicConsumed event.
        OnRelicConsumed += ConsumeRelic;

        // Run the IEffectable OnEnable().
        base.OnEnable();

        // Initialize the input system.
        DigInit();

        _getHitTest = _actions.Player.Crouch;
        _getHitTest.Enable();
        _getHitTest.performed += HitPlayerTest;
    }

    /// <summary>
    /// Initialize the digging mechanic.
    /// </summary>
    private void DigInit()
    {
        // I AM AWARE that this is not a good way to do this.
        // Depending on the player number, grab the correct input map.
        if (PlayerNumber == 1)
        {
            _dig = _actions.Player1.Interact;
        }
        else if (PlayerNumber == 2)
        {
            _dig = _actions.Player2.Interact;
        }
        else
        {
            _dig = _actions.Player.Interact;
        }

        // Initialize the dig mechanic.
        _dig.performed += Dig;
        _dig.Enable();
    }

    /// <summary>
    /// Initializations for the OnDisable function.
    /// </summary>
    private void OnDisableInit()
    {
        base.OnDisable();
        _dig.Disable();

        _getHitTest.Disable();
    }

    /// <summary>
    /// The method holding all of the initializations that the Player component must do at the Start function. 
    /// </summary>
    private void StartInit()
    {
        // Call the IEffectable Start function.
        base.Start();

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
            _metalDetector.OnRelicVeryClose += AcceptRelic;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a Metal Detector component. The game will still work but the player will not be able to find items.");
        }
        if (TryGetComponent(out _movementInput))
        {
            _hasMovementInput = true;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a Movement Input component. The game will still work but the player will not be able to move.");
        }
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
    /// Build the EffectRelic list and call the function which invokes the OnUpdateEffectRelics
    /// </summary>
    protected override void BuildEffectRelicList()
    {
        List<Relic> effectRelics = new List<Relic>();

        // This foreach loop will check each relic.
        // If they have effects then add them to the list.
        // Otherwise, don't.
        foreach (Relic relic in _inventory)
        {
            // All the passive effects on this relic.
            Dictionary<Effect, string>.KeyCollection thisRelicsEffects = relic.GetPassiveEffects().Keys;

            // If this list is greater than 0, then double check to see if they're not all the None Effect.
            // Yes, I know this would be strange.
            if (thisRelicsEffects.Count > 0)
            {
                // Use a counter. If it is greater than 0 at the end of the below foreach loop, then it has an actual passive effect.
                int checkForNonNoneEffect = 0;
                foreach (Effect passiveEffect in thisRelicsEffects)
                {
                    if (passiveEffect == Effect.None) continue;
                    checkForNonNoneEffect++;
                }
                if (checkForNonNoneEffect > 0)
                {
                    effectRelics.Add(relic);
                }
            }
        }

        InvokePassiveEffectEvent(effectRelics);
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
        BuildEffectRelicList();
    }
    /// <summary>
    /// Consume a relic at an index. In other words delete it.
    /// </summary>
    /// <param name="index"> The inventory index to remove a relic. </param>
    private void ConsumeRelicAt(int index)
    {
        _inventory.RemoveItemAt(index);
        BuildEffectRelicList();
    }

    /// <summary>
    /// This method runs when a Player is hit. Whether they tank the hit or receive it is determined by this method.
    /// </summary>
    private void PlayerHit()
    {
        // If there are more than 0 extra lives.
        if (_extraLives >= 1)
        {
            // Reduce and announce the loss of a life.
            OnExtraLifeChanged?.Invoke(--_extraLives);
            // Tank the hit.
            TankHit();

            // If the extraLives var has gone from 1 to 0, meaning the Relic must now be consumed.
            if (_extraLives == 0)
            {
                // The relic at that variable should exist, if it does not then it is logged and consuming the relic is ignored.
                if (_currentExtraLifeRelic != null) ConsumeRelic(_currentExtraLifeRelic);
                else Debug.Log("There was an attempt by a player to destroy a relic which gave the player an extra hit. The Relic was not properly assigned to the _currentExtraLifeRelic variable: " + PlayerNumber);
                _currentExtraLifeRelic = null;
            }
        }
        else
        {
            // Ensure that the _extraLives var does not go below zero.
            _extraLives = 0;
            ReceiveHit();
        }
        
    }
    
    /// <summary>
    /// This method is run when a Player "tanks a hit." I.e. they are hit but it has no effect.
    /// </summary>
    private void TankHit()
    {

    }

    /// <summary>
    /// This method is run when a Player receives a hit. I.e. they are hit and must face the consequences.
    /// </summary>
    private void ReceiveHit()
    {
        Debug.Log("There was an attempt ");
    }

    /// <summary>
    /// Implemented method which launches active effects.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    protected override void LaunchActiveEffect()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Async method which actviates when the player digs.
    /// </summary>
    /// <param name="context"> The CallbackContext for this action.   </param>
    private async void Dig(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDigging)
        {

            // The Player is digging.
            _isDigging = true;

            // Play the digging animation.
            ////

            // Stop the player's movement.
            ChangeSpeed(0);

            // Await the duration of the animation.
            //// For now, 3 seconds.
            await Task.Delay(1000);

            ChangeSpeed(1);

            // If the Player found a relic.
            if (_isRelicFound && (_relicFound != null && _buriedRelicFound != null))
            {

                // Add it to the inventory.
                _inventory.AddItem(_relicFound);
                BuildEffectRelicList();


                // Play found relic animation.
                ////

                // Destroy the buried relic.
                Destroy(_buriedRelicFound.gameObject);

                // Reset relic found vars.
                _isRelicFound = false;
                _relicFound = null;
                _buriedRelicFound = null;

            }

            // When no longer digging, set this to false.
            _isDigging = false;
        }
    }

    /// <summary>
    /// Change the speed of the player if they have a Movement Input component.
    /// This function is not additive. That is, every call to this function sets the speed to be a percentage of the base speed.
    /// </summary>
    /// <param name="percentage"> Percentage of the base speed. </param>
    private void ChangeSpeed(float percentage)
    {
        if (_hasMovementInput)
        {
            _movementInput.AffectSpeed(percentage);
        }
    }

    /// <summary>
    /// Apply the Lightning Effect.
    /// </summary>
    protected override void ApplyLightning()
    {
        PlayerHit();
    }

    protected override void ApplyFog()
    {
        throw new NotImplementedException();
    }

    protected override void ApplyExtraLives(int extraLives, Relic extraLivesRelic)
    {
        // If there is not already a relic applying this effect.
        if (_extraLives == 0 && _currentExtraLifeRelic == null)
        {
            // Set the extra life variables.
            _extraLives = extraLives;
            _currentExtraLifeRelic = extraLivesRelic;
            // Announce the extra life info to event listeners.
            OnExtraLifeChanged?.Invoke(_extraLives);
        }
        else
        {
            Debug.Log("The Player #" + PlayerNumber + " tried to apply the ExtraLives effect to itself but found that there was already a relic applying that effect.");
        }
    }

    protected override void CancelExtraLives()
    {
        throw new NotImplementedException();
    }
    
    private void ExtraLifeTest(int currentLives)
    {
        Debug.Log("extra life called: " + currentLives + " and the class variable: " + _extraLives);
    }
    private void HitPlayerTest(InputAction.CallbackContext context)
    {
        if (context.performed) PlayerHit();
    }
}
