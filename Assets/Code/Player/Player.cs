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
    /// The index of the Relic currently in the player's hand.
    /// </summary>
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

    private Rigidbody _rigidBody;
    private bool _hasRigidBody = false;

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
    /// Reference to Player Animator
    /// </summary>
    [SerializeField] private Animator anim;

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
    /// Event that runs when a player is damaged.
    /// </summary>
    public event Action OnPlayerDamaged;

    /// <summary>
    /// Variable which allows a Player to get hit as a test.
    /// </summary>
    private InputAction _getHitTest;

    /// <summary>
    /// Event called when there is a call to cycle the relic in the player's hand.
    /// This is necessary on top of the InputAction because a reording of items and thus of what's in the player's hand happens without pressing the input.
    /// </summary>
    public event Action<bool> OnCycleRelicInHand;

    /// <summary>
    /// Event called when the relic in hand has changed. Returns a list of the inventory where the first item is the item in hand.
    /// </summary>
    public event Action<List<Relic>> OnRelicInHandChanged;

    /// <summary>
    /// The InputAction related to cycling the relic in hand.
    /// </summary>
    private InputAction _cycleRelicAction;

    /// <summary>
    /// Invincibility time.
    /// </summary>
    [SerializeField] private float ITime = 0.5f;

    /// <summary>
    /// Invincibility timer.
    /// </summary>
    private float ITimer = 0;

    /// <summary>
    /// The last direction the player was moving in.
    /// </summary>
    private float _lastDirection = 0;

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
    /// Method which runs every frame.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        UpdateHelper();
    }

    /// <summary>
    /// Method holding initializations for the Awake function.
    /// </summary>
    private void AwakeInit()
    {
        base.Awake();
        _actions = new InputSystem_Actions();
    }

    /// <summary>
    /// Initializations for the OnEnable function.
    /// </summary>
    private void OnEnableInit()
    {
        // Run the IEffectable OnEnable().
        base.OnEnable();

        // Initialize the input system.
        DigInit();
        CycleInit();

        // Subscribe to events.
        OnEnableEventSubscribers();
    }

    /// <summary>
    /// Initializations for the OnDisable function.
    /// </summary>
    private void OnDisableInit()
    {
        base.OnDisable();
        OnDisableEventSubscribers();
    }

    private void OnEnableEventSubscribers()
    {
        // Initialize the dig mechanic.
        _dig.performed += Dig;
        _dig.Enable();

        // Instantiate the action for cycling a relic.
        _cycleRelicAction.Enable();
        _cycleRelicAction.performed += CycleEffectRelic;

        // When the player is damaged, receive the hit.
        OnPlayerDamaged += ReceiveHit;

        // Subscribe to the OnRelicConsumed event.
        OnRelicConsumed += ConsumeRelic;

        // Subscribe the UpdateRelicInHand function to the related event.
        OnCycleRelicInHand += UpdateRelicInHand;

        // For testing purposes.
        _getHitTest = _actions.Player.Crouch;
        _getHitTest.Enable();
        _getHitTest.performed += HitPlayerTest;

        // Test event calls.
        OnExtraLifeChanged += ExtraLifeTest;
        OnRelicInHandChanged += CycleRelicTest;
    }

    private void OnDisableEventSubscribers()
    {
        // Disable event actions
        _dig.Disable();
        _getHitTest.Disable();
        _cycleRelicAction.Disable();

        // Unsubscribe from events.
        OnRelicConsumed -= ConsumeRelic;
        OnPlayerDamaged -= ReceiveHit;
        OnCycleRelicInHand -= UpdateRelicInHand;
        _cycleRelicAction.performed -= CycleEffectRelic;
        _dig.performed -= Dig;
        _getHitTest.performed -= HitPlayerTest;

        // Test event calls.
        OnExtraLifeChanged -= ExtraLifeTest;
        OnRelicInHandChanged -= CycleRelicTest;
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
    }

    /// <summary>
    /// Initialization for the cycle input.
    /// </summary>
    private void CycleInit()
    {
        if (PlayerNumber == 1)
        {
            _cycleRelicAction = _actions.Player1.Cycle_Relic;
        }
        else if (PlayerNumber == 2)
        {
            _cycleRelicAction = _actions.Player2.Cycle_Relic;
        }
        else
        {
            _cycleRelicAction = _actions.Player.Jump;
        }
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
        if (TryGetComponent(out _rigidBody))
        {
            _hasRigidBody = true;
        }
        else
        {
            Debug.Log("Player #" + PlayerNumber + " does not have a RigidBody component. The game will still work but the player will not collide properly.");
        }
    }

    /// <summary>
    /// Method which helps the Update function.
    /// </summary>
    private void UpdateHelper()
    {
        // If the ITimer is above 0, then the invisibility frames have begun and they must be counted down.
        ITimer = (ITimer <= 0) ? ITimer -= Time.deltaTime : 0;

        AnimationDirection(GetDirection());
        AnimationRunState();
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
            anim.SetTrigger("IsDigging");

            // Stop the player's movement.
            ChangeSpeed(0);

            // Await the duration of the animation.
            //// For now, 1 second.
            await Task.Delay(1000);

            ChangeSpeed(1);

            // If the Player found a relic.
            if (_isRelicFound && (_relicFound != null && _buriedRelicFound != null))
            {
                // Add it to the inventory.
                AddRelicToInventory(_relicFound);

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
    /// This method updates which Relic is currently in the player's hand.
    /// </summary>
    /// <param name="isCycleCalled"></param>
    private void UpdateRelicInHand(bool isCycleCalled)
    {
        // so instead of returning an index
        // return the inventory listed with this item first

        // If the inventory is empty, set the index to -1 and return an empty list.
        if (_inventory.Count() <= 0)
        {
            _relicInHandIndex = -1;
            //OnRelicInHandChanged?.Invoke(-1); 
            OnRelicInHandChanged?.Invoke( new List<Relic>() );
        }
        // Otherwise, check whether the current index is an effect relic.
        // If it is, then go directly to the cycle check.
        // If it is not, cycle until a new Effect relic is found. If none are found then set _relicInHandIndex to -1 and break.
        else
        {
            _relicInHandIndex = 
                // If the relicInHandIndex > the size of the inventory. set it to be the last index.
                (_relicInHandIndex >= _inventory.Count()) ? _relicInHandIndex = _inventory.Count() - 1: 
                // If the relicInHandIndex is -1, representing nothing in the hand, set it to zero.
                (_relicInHandIndex == -1) ? 0:
                // Otherwise, keep it the same.
                _relicInHandIndex;

            // Variables that will help indicate whether an effect relic was found in the inventory.
            int nearestEffectRelic = -1;
            int nextEffectRelic = -1;

            // If the current indexInHand is an effect relic then set that index to the nearestEffectRelic.
            if (IsEffectRelic(_inventory.GetRelicAt(_relicInHandIndex))) // this causes an error? WAIT did this relic get deleted WHILE this func was running??? sick... oh wait no lol uhh maybe actually
            {
                nearestEffectRelic = _relicInHandIndex;
            }

            // Search through the inventory to find the two nearest effect relics.
            for (int cycleIndex = _relicInHandIndex + 1; cycleIndex != _relicInHandIndex; cycleIndex++)
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
                //OnRelicInHandChanged?.Invoke(-1);
                OnRelicInHandChanged?.Invoke( BuildDisplayList(0) );
                return;
            }
            // If there was an Effect Relic but ONLY ONE of them.
            // Then there will be no cycling.
            if (nextEffectRelic == -1)
            {
                return;
            }
            // If the index is on an Effect Relic AND the isCycleCalled is true, then cycle to that Effect relic.
            // This function may be called without a call to cycle to the next relic because there may simply be a reordering of the inventory.
            if (isCycleCalled)
            {
                _relicInHandIndex = nextEffectRelic;
                //OnRelicInHandChanged?.Invoke(_relicInHandIndex);
                OnRelicInHandChanged?.Invoke( BuildDisplayList( _relicInHandIndex ) );
            }
        }
    }

    private List<Relic> BuildDisplayList(int firstIndex)
    {
        // Initialize the displayList
        List<Relic> displayList = new List<Relic>();

        // Add the relic at the firstIndex before the loop.
        displayList.Add( _inventory.GetRelicAt(firstIndex) );

        Debug.Log("THIS GOES ON FOREVER!!!");
        int i = 0;

        // Loop across the whole list and stop before adding the firstIndex.
        for (int index = firstIndex+1; index != firstIndex ; index++)
        {
            // If the index reaches the end of the list, loop back to zero.
            if (index >= _inventory.Count())
            {
                index = 0;
            }
            // Add the item at the index.
            displayList.Add( _inventory.GetRelicAt(index) );

            Debug.Log("index: " + index + " firstIndex: " + firstIndex);
            i++;
            if (i == 100) break;
        }

        return displayList;
    }

    /// <summary>
    /// Method called when the effect relic cycle action is pressed.
    /// </summary>
    /// <param name="context"> CallbackContext context </param>
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
            // check if iframe timer is going


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
            // If the invincibility timer is not running.
            if (ITimer <= 0)
            {
                // Damage the player.
                OnPlayerDamaged?.Invoke();
            }
            // Ensure that the _extraLives var does not go below zero.
            _extraLives = 0;
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
        // LOSE ALL RELICS
        _inventory.Clear();
        BuildEffectRelicList();

        ITimer = ITime;

        anim.SetTrigger("GetHit");
    }

    /// <summary>
    /// Implemented method which launches active effects.
    /// </summary>
    /// <param name="direction"> The direction this relic is being launched. </param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void LaunchActiveEffect(Vector2 direction)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the angled direction of the player.
    /// </summary>
    /// <returns> A float value representing the direction as an angle. </returns>
    private float GetDirection()
    {
        // Get the direction of the movement.
        Vector2 direction = _movementInput.GetDirection();

        // If the player is stationary, then return the previous position.
        if (direction.x == 0 && direction.y == 0)
        {
            return _lastDirection;
        }
        
        // Otherwise, calculate the angle.
        if (direction.y >= 0)
        {
            // If y is positive, then calculate the angle between (1,0) and the direction.
            _lastDirection = Vector2.Angle(new Vector2(1, 0), direction);
            return _lastDirection;
        }
        // If y is negative, then calculate the angle between (-1,0) and the direction and add 180.
        else
        {
            // Otherwise, set the current position and return it.
            _lastDirection = Vector2.Angle(new Vector2(-1, 0), direction) + 180;
            return _lastDirection;
        }
    }

    //Animates the Player in the direction they are moving in
    private void AnimationDirection(float dir)
    {
        //Player facing down
        if (dir >= 225 && dir <= 315)
        {
            anim.SetInteger("moveDirection", 0);
        }
        //Player facing left
        else if (dir > 135 && dir < 225)
        {
            anim.SetInteger("moveDirection", 1);
        }
        //Player facing right
        else if (dir > 315 || dir < 45)
        {
            anim.SetInteger("moveDirection", 2);
        }
        //Player facing up
        else if (dir >= 45 && dir <= 135)
        {
            anim.SetInteger("moveDirection", 3);
        }
    }

    //Animates the Player either running or idle depending on current velocity
    private void AnimationRunState()
    {
        anim.SetFloat("mSpeed", _rigidBody.linearVelocity.magnitude);
    }
    
    // Play the digging animation.
    // anim.SetTrigger("UseRelic");

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
    /// Runs when _metalDetector invokes an event for proximity to relic AND dig is pressed.
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

    private void AddRelicToInventory(Relic relic)
    {
        // Add it to the inventory.
        _inventory.AddItem(relic);

        // Adding a relic to the inventory may require a cycle to an Effect relic in the case where this relic is the only one that will be an Effect Relic in the inventory.
        OnCycleRelicInHand?.Invoke(false);

        BuildEffectRelicList();
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
    /// Apply the Lightning Effect.
    /// </summary>
    protected override void ApplyLightning()
    {
        PlayerHit();
    }

    protected override void ApplyFog()
    {
        
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

    /// <summary>
    /// Implementation of the LightningForray Effect.
    /// </summary>
    protected override void ApplyLightningForray()
    {
        PlayerHit();
    }

    private void ExtraLifeTest(int currentLives)
    {
        Debug.Log("extra life called: " + currentLives + " and the class variable: " + _extraLives);
    }
    private void HitPlayerTest(InputAction.CallbackContext context)
    {
        if (context.performed) PlayerHit();
    }

    
    private void CycleRelicTest(List<Relic> displayList)
    {
        Debug.Log("Display List");
        foreach (Relic relic in displayList)
        {
            Debug.Log("\t\t" + relic.GetName());
        }
    }

    
}
