using System;
using System.Collections.Generic;
using System.Threading;
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
    /// The Relic currently in the player's hand.
    /// </summary>
    // private Relic _relicInHand;

    private int _relicInHandIndex = -1;

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

    /// <summary>
    /// Variable which allows a Player to get hit as a test.
    /// </summary>
    private InputAction _getHitTest;

    /// <summary>
    /// Event called when there is a call to cycle the relic in the player's hand.
    /// </summary>
    public event Action<bool> OnCycleRelicInHand;

    /// <summary>
    /// Event called when the relic in hand has changed.
    /// </summary>
    public event Action<int> OnRelicInHandChanged;

    private InputAction _cycleRelicAction;

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
        OnCycleRelicInHand += UpdateRelicInHand;

        OnRelicInHandChanged += CycleRelicTest;
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

        // For testing purposes.
        _getHitTest = _actions.Player.Crouch;
        _getHitTest.Enable();
        _getHitTest.performed += HitPlayerTest;

        // Cycling the relic in hand.
        CycleInit();

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
    /// Initialization for the cycle input.
    /// </summary>
    private void CycleInit()
    {
        if (PlayerNumber == 1) _cycleRelicAction = _actions.Player1.Cycle_Relic;
        else if (PlayerNumber == 2) _cycleRelicAction = _actions.Player2.Cycle_Relic;
        else _cycleRelicAction = _actions.Player.Jump;

        _cycleRelicAction.Enable();
        _cycleRelicAction.performed += CycleEffectRelic;
    }

    /// <summary>
    /// Initializations for the OnDisable function.
    /// </summary>
    private void OnDisableInit()
    {
        base.OnDisable();
        _dig.Disable();

        _getHitTest.Disable();

        OnRelicConsumed -= ConsumeRelic;
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

            // If this is an Effect relic.
            if (IsEffectRelic(relic))
            {
                effectRelics.Add(relic);
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
        // Debug.Log("Relic consumed: " + relic.GetName());

        // Remove the item from the inventory.
        _inventory.RemoveItem(relic);
        // Check whether the relic in hand needs to change.
        OnCycleRelicInHand?.Invoke(false);
        // Recalculate the passive effects.
        BuildEffectRelicList();
    }
    /// <summary>
    /// Consume a relic at an index. In other words delete it.
    /// </summary>
    /// <param name="index"> The inventory index to remove a relic. </param>
    private void ConsumeRelicAt(int index)
    {
        _inventory.RemoveItemAt(index);
        OnCycleRelicInHand?.Invoke(false);
        BuildEffectRelicList();
    }

    /// <summary>
    /// This method updates which Relic is currently in the player's hand.
    /// </summary>
    /// <param name="isCycleCalled"></param>
    private void UpdateRelicInHand(bool isCycleCalled)
    {
        // so this is called from an event
        // the event OnCycleRelicInHand invokes when the action is pressed for it and when the inventory change...
        // if isCycleCalled is true, then 

        // If the inventory is empty, set the index to -1.
        if (_inventory.Count() <= 0)
        {
            _relicInHandIndex = -1;
            OnRelicInHandChanged(-1);
        }
        // Otherwise, check whether the current index is an effect relic.
        // If it is, then go directly to the cycle check.
        // If it is not, cycle until a new Effect relic is found. If none are found then set _relicInHandIndex to -1 and break.
        else
        {
            // Set the _relicInHand index to 0 so that there is no out of range error.
            // This is only necessary if the index is previously -1.
            if (_relicInHandIndex == -1) _relicInHandIndex = 0;

            // Variables that will help indicate whether an effect relic was found in the inventory.
            int nearestEffectRelic = -1;
            int nextEffectRelic = -1;

            // If the current indexInHand is an effect relic then set that index to the nearestEffectRelic.
            if (IsEffectRelic(_inventory.GetRelicAt(_relicInHandIndex)))
            {
                nearestEffectRelic = _relicInHandIndex;
            }

            // Search through the inventory to find the two nearest effect relics.
            for (int cycleIndex = _relicInHandIndex; cycleIndex != _relicInHandIndex; cycleIndex++)
            {
                // Check to see if the loop needs to cycle to the beginning.
                if (cycleIndex >= _inventory.Count())
                {
                    cycleIndex = 0;
                }
                // If an effect relic is found, note its index.
                if (IsEffectRelic(_inventory.GetRelicAt(cycleIndex)))
                {
                    // If this loop has not yet found any Effect Relics in the inventory.
                    if (nearestEffectRelic == -1)
                    {
                        nearestEffectRelic = cycleIndex;
                    }
                    else if (nextEffectRelic == -1)
                    {
                        // Note down the next Effect Relic.
                        nextEffectRelic = cycleIndex;
                        // Break, because we only need the nearest Effect Relic and the one after it.
                        break;
                    }
                }
            }

            // If there were indeed no Effect relics, then note that and return.
            if (nearestEffectRelic == -1)
            {
                _relicInHandIndex = -1;
                OnRelicInHandChanged(-1);
                return;
            }
            // If there was an Effect Relic but ONLY ONE of them.
            if (nextEffectRelic == -1)
            {
                return;
            }
            // If there the index is on an Effect Relic AND the isCycleCalled is true, then cycle to that Effect relic.
            if (isCycleCalled)
            {
                _relicInHandIndex = nextEffectRelic;
                OnRelicInHandChanged(_relicInHandIndex);
            }
        }
        
    }

    private void CycleEffectRelic(InputAction.CallbackContext context)
    {
        if (context.performed) OnCycleRelicInHand?.Invoke(true);
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

            // If the extraLives var has gone from 1 to 0, the effect mustbe cancelled.
            if (_extraLives == 0)
            {
                // Cancel the extra lives effect.
                CancellationTokenSource token;
                if (_effectsCancellationTokens.TryGetValue(Effect.ExtraLives, out token))
                {
                    token.Cancel();
                }
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
        Debug.Log("There was an attempt to tank a hit but it was not implemented.");
    }

    /// <summary>
    /// This method is run when a Player receives a hit. I.e. they are hit and must face the consequences.
    /// </summary>
    private void ReceiveHit()
    {
        Debug.Log("There was an attempt to receive a hit but it was not implemented.");
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
            //// For now, 1 second.
            await Task.Delay(1000);

            ChangeSpeed(1);

            // If the Player found a relic.
            if (_isRelicFound && (_relicFound != null && _buriedRelicFound != null))
            {

                Debug.Log("Player.Dig() ~ adding item to dictionary and calling BuildEffectRelicList()");

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

    /// <summary>
    /// Method which helps remove the extra lives.
    /// This method is called when the relic which offered those extra lives is destroyed.
    /// </summary>
    protected override void CancelExtraLives()
    {
        if (_extraLives > 0)
        {
            OnExtraLifeChanged?.Invoke(0);
        }
        _extraLives = 0;
        _currentExtraLifeRelic = null;
    }
    
    private void ExtraLifeTest(int currentLives)
    {
        Debug.Log("extra life called: " + currentLives + " and the class variable: " + _extraLives);
    }
    private void HitPlayerTest(InputAction.CallbackContext context)
    {
        if (context.performed) PlayerHit();
    }

    private void CycleRelicTest(int index)
    {
        Debug.Log("cycle relic test: " + index);
        Debug.Log("inventory");
        foreach (Relic relic in _inventory)
        {
            Debug.Log("\t\t" + relic.GetName());
        }
    }
}
