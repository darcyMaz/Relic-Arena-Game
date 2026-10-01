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

    /// <summary>
    /// The Use relic InputAction.
    /// </summary>
    private InputAction _use;

    /// <summary>
    /// The RigidBody component attached to this Player.
    /// </summary>
    private Rigidbody _rigidBody;
    /// <summary>
    /// A boolean which denotes whether a RigidBody was found. 
    /// </summary>
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
    private InputAction _cycle;

    /// <summary>
    /// Invincibility time.
    /// </summary>
    [SerializeField] private float ITime = 2f;

    /// <summary>
    /// Invincibility timer.
    /// </summary>
    private float ITimer = 0;

    /// <summary>
    /// The last direction the player was moving in.
    /// </summary>
    private float _lastDirection = 0;

    /// <summary>
    /// Bool which helps the script understand whether the EffectsManager.Instance was found onEnable.
    /// </summary>
    private bool _wasEffectsManagerFound = false;

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
        GetComponentsAwake();
    }

    /// <summary>
    /// Method which tries to get the components attached to the player.
    /// </summary>
    private void GetComponentsAwake()
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
    /// Initializations for the OnEnable function.
    /// </summary>
    private void OnEnableInit()
    {
        // Run the IEffectable OnEnable().
        base.OnEnable();

        // Initialize the input system.
        DigInit();
        CycleInit();
        UseInit();

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
        _cycle.performed += CycleEffectRelic;
        _cycle.Enable();
        
        _use.performed += LaunchEffectPressed;
        _use.Enable();

        // When the player is damaged, receive the hit.
        OnPlayerDamaged += ReceiveHit;

        // Subscribe to the OnRelicConsumed event.
        OnRelicConsumed += ConsumeRelic;

        // Subscribe the UpdateRelicInHand function to the related event.
        OnCycleRelicInHand += UpdateRelicInHand;
        
        // Subscribe to inventory events.
        if (_hasInventory)
        {
            // _inventory.OnInventoryCleared += ConsumeAllAfterClear;
            _inventory.OnInventoryChange += InventoryChangeCycleRelic;
        }

        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.OnActiveEffectLaunched += ReceiveActiveEffect;
            _wasEffectsManagerFound = true;
        }
        
        // For testing purposes.
        _getHitTest = _actions.Player.Crouch;
        _getHitTest.Enable();
        _getHitTest.performed += HitPlayerTest;

    }

    private void OnDisableEventSubscribers()
    {
        // Disable event actions
        _dig.Disable();
        _use.Disable();
        _cycle.Disable();

        _getHitTest.Disable();

        // Unsubscribe from events.
        OnRelicConsumed -= ConsumeRelic;
        OnPlayerDamaged -= ReceiveHit;
        OnCycleRelicInHand -= UpdateRelicInHand;
        _cycle.performed -= CycleEffectRelic;
        if (_hasInventory)
        {
            // _inventory.OnInventoryCleared -= ConsumeAllAfterClear;
            _inventory.OnInventoryChange -= InventoryChangeCycleRelic;
        }

        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.OnActiveEffectLaunched -= ReceiveActiveEffect;
        }

        _dig.performed -= Dig;
        _getHitTest.performed -= HitPlayerTest;

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
    /// Initialize the use relic mechanic.
    /// </summary>
    private void UseInit()
    {
        if (PlayerNumber == 1)
        {
            _use = _actions.Player1.Use;
        }
        else if (PlayerNumber == 2)
        {
            _use = _actions.Player2.Use;
        }
        else
        {
            _use = _actions.Player.Attack;
        }
    }

    /// <summary>
    /// Initialize the cycle mechanic.
    /// </summary>
    private void CycleInit()
    {
        if (PlayerNumber == 1)
        {
            _cycle = _actions.Player1.Cycle;
        }
        else if (PlayerNumber == 2)
        {
            _cycle = _actions.Player2.Cycle;
        }
        else
        {
            _cycle = _actions.Player.Jump;
        }
    }

    /// <summary>
    /// The method holding all of the initializations that the Player component must do at the Start function. 
    /// </summary>
    private void StartInit()
    {
        // Call the IEffectable Start function.
        base.Start();

        StartHelper();
    }

    private void StartHelper()
    {
        // If the EffectsManager instance was not found OnEnable.
        if (!_wasEffectsManagerFound)
        {
            // Subscribe to the event here.
            EffectsManager.Instance.OnActiveEffectLaunched += ReceiveActiveEffect;
        }
    }

    /// <summary>
    /// Method which helps the Update function.
    /// </summary>
    private void UpdateHelper()
    {
        // If the ITimer is above 0, then the invisibility frames have begun and they must be counted down.
        ITimer = (ITimer > 0) ? ITimer -= Time.deltaTime : 0;

        AnimationDirection(GetDirection());
        AnimationRunState();
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

            /*
             * Note on throwing and digging being the same button:
             * Some logic may have to change to make this work.
             * Check first if there is a relic found, if not, then call the throw method.
             * Otherwise, dig as usual.
             */


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
    /// Method called when the effect relic cycle action is pressed.
    /// </summary>
    /// <param name="context"> CallbackContext context </param>
    private void CycleEffectRelic(InputAction.CallbackContext context)
    {
        if (context.performed) OnCycleRelicInHand?.Invoke(true);
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
    /// This method updates which Relic is currently in the player's hand and invokes an event showing the list of relics in the right order.
    /// </summary>
    /// <param name="isCycleCalled"></param>
    private void UpdateRelicInHand(bool isCycleCalled)
    {
        // Debug.Log("UpdateRelicInHand called");

        // If the inventory is empty, set the index to -1 and return an empty list.
        if (_inventory.Count() <= 0)
        {
            // Debug.Log("- Inventory is empty");
            _relicInHandIndex = -1;
            //OnRelicInHandChanged?.Invoke(-1); 
            OnRelicInHandChanged?.Invoke( new List<Relic>() );
            return;
        }
        // Otherwise, check whether the current index is an effect relic.
        // If it is, then go directly to the cycle check.
        // If it is not, cycle until a new Effect relic is found. If none are found then set _relicInHandIndex to -1 and break.
        else
        {
            // Debug.Log("- Inventory has more than one relic");

            // Ensure the relicInHandIndex is not out of bounds.
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

            // For each relic in the inventory.
            for (int cycleIndex = 0; cycleIndex < _inventory.Count(); cycleIndex++)
            {
                // Do not go through the relics in order from start to finish, instead start at the _relicInHandIndex.
                int adjustedIndex = (cycleIndex + _relicInHandIndex) % _inventory.Count();

                // If the relic at the adjusted index is an effect relic.
                if (IsEffectRelic( _inventory.GetRelicAt(adjustedIndex) ))
                {
                    // If this loop has not yet found an effect relic before this, then set this index to be the nearest effect relic.
                    if (nearestEffectRelic == -1)
                    {
                        nearestEffectRelic = adjustedIndex;
                        continue;
                    }
                    // If this loop has found exactly one effect relic before this, note the index of the second effect relic in the list.
                    else if (nextEffectRelic == -1)
                    {
                        nextEffectRelic = adjustedIndex;
                        break;
                    }
                }
            }
            
            // Debug.Log("- UpdateHand after for loop ~ nearestEffectRelicIndex: " + nearestEffectRelic + " nextEffectRelic " + nextEffectRelic);

            // If there were indeed no Effect relics, then note that and return.
            if (nearestEffectRelic == -1)
            {
                // Debug.Log("-- nearest effect relic is -1");

                _relicInHandIndex = -1;
                OnRelicInHandChanged?.Invoke( BuildDisplayList(0) );
                return;
            }
            // If there was an Effect Relic but ONLY ONE of them.
            // Then there will be no cycling.
            if (nextEffectRelic == -1)
            {
                // Debug.Log("-- next effect relic is -1");

                OnRelicInHandChanged?.Invoke(BuildDisplayList( _relicInHandIndex ));
                return;
            }
            // At this point in the method, there are two effect relics found.

            // If the index is on an Effect Relic AND the isCycleCalled is true, then cycle to that Effect relic.
            // This function may be called without a call to cycle to the next relic because there may simply be a reordering of the inventory.
            if (isCycleCalled)
            {
                // Debug.Log("-- cycle was called, and thus there was more than one effect relic and Q was pressed");
                _relicInHandIndex = nextEffectRelic;
                OnRelicInHandChanged?.Invoke( BuildDisplayList( _relicInHandIndex ) );
            }
            else
            {
                // Debug.Log("-- cycle was not called, two effect relics and q was nor pressed");
                _relicInHandIndex = nearestEffectRelic;
                OnRelicInHandChanged?.Invoke(BuildDisplayList(_relicInHandIndex));
            }
        }
    }

    /// <summary>
    /// Build and return a list of relics where a specified index is at the beginning of the list.
    /// </summary>
    /// <param name="firstIndex"> The index that will start the list. </param>
    /// <returns> An adjusted list of relics. </returns>
    private List<Relic> BuildDisplayList(int firstIndex)
    {
        // Initialize the displayList
        List<Relic> displayList = new List<Relic>();

        if (firstIndex < 0 || firstIndex >= _inventory.Count())
        {
            return displayList;
        }

        for (int buildIndex = 0; buildIndex < _inventory.Count(); buildIndex++)
        {
            int adjustedIndex = (firstIndex + buildIndex) % _inventory.Count();
            displayList.Add(_inventory.GetRelicAt(adjustedIndex));
        }

        return displayList;
    }

    /// <summary>
    /// Method which subscribes to the inventory change and then calls the OnCycleRelicInHand event so that the display is updated.
    /// </summary>
    /// <param name="unused"> Unused relic list. </param>
    private void InventoryChangeCycleRelic(IEnumerator<Relic> unused)
    {
        OnCycleRelicInHand?.Invoke(false);
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
    /// Method which receives an Active Effect while listening to the EffectsManager. It attempts to apply the Effect if this Player has the same number as the target parameter.
    /// </summary>
    /// <param name="activeEffect"> The active Effect to be potentially applied to the Player. </param>
    /// <param name="effectDetails"> The details of that active Effect. </param>
    /// <param name="source"> The source of the Effect as an int representing the player number. </param>
    /// <param name="target"> The target of the Effect as an int representing the player number. </param>
    private void ReceiveActiveEffect(Effect activeEffect, string effectDetails, int source, int target)
    {
        // Check whether this player is accepting this Effect.
        bool wasEffectAdded = false;
        if (target == PlayerNumber)
        {
            // Apply the Effect.
            wasEffectAdded = ApplyActiveEffect(activeEffect, effectDetails);
        }

        if (wasEffectAdded)
        {
            Debug.Log("The Active Effect " + activeEffect + " was potentially added to the player #" + target + " from the player #" + source);
        }
    }

    /// <summary>
    /// Method invoked when the launch button was pressed.
    /// </summary>
    private void LaunchEffectPressed(InputAction.CallbackContext context)
    {
        // Initialize the relic.
        Relic relicInHand;

        // Check the validity of the _relicInHandIndex first.
        if (_relicInHandIndex < 0 || _relicInHandIndex >= _inventory.Count())
        {
            return;
        }

        // Get the relic in hand.
        relicInHand = _inventory.GetRelicAt(_relicInHandIndex);

        // Check if the relic in hand is actually an effect relic (it should be anyway).
        if (!IsEffectRelic(relicInHand))
        {
            return;
        }

        // Call LaunchActiveEffect()
        LaunchActiveEffect(relicInHand.GetActiveEffect(), relicInHand.GetActiveEffectDetails(), relicInHand.GetLaunchType());
    }

    /// <summary>
    /// Implemented method which launches active effects.
    /// </summary>
    /// <param name="direction"> The direction this relic is being launched. </param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void LaunchActiveEffect(Effect activeEffect, string activeEffectDetails, LaunchType launchType)
    {
        // Call a specific function mapped to this LaunchType.
        Action<Effect, string> _launchFunc;
        if (_launchTypeFuncs.TryGetValue(launchType, out _launchFunc)) 
        {
            _launchFunc.Invoke(activeEffect, activeEffectDetails);
        }
        else
        {
            Debug.LogError("There was an attempt by Player#" + PlayerNumber + " to launch an active Effect, but the LaunchType was invalid.");
        }
    }

    protected override void LaunchRaycast(Effect activeEffect, string activeEffectDetails)
    {
        // get the direction of the pointer
        // raycast that way
        // check for the nearest player
        // apply the effect to them

        throw new NotImplementedException();
    }
    private Vector3 GetPointerDirection()
    {
        return Vector3.zero;
    }

    protected override void LaunchThrow(Effect activeEffect, string activeEffectDetails)
    {
        throw new NotImplementedException();
    }

    protected override void LaunchImmediate(Effect activeEffect, string activeEffectDetails)
    {
        // If the EffectsManager exists.
        if (EffectsManager.Instance != null) 
        {
            if (PlayerNumber < 1 || PlayerNumber > 2)
            {
                Debug.LogError("Player #" + PlayerNumber+ " tried to perform an immediate active Effect but the playerNum was not valid.");
                return;
            }

            // Ideally, there'd be a way to decide which player gets hit.
            // Maybe random, maybe a selection.
            // Either way, this should not be so particular.
            int targetNum = (PlayerNumber == 1) ? 2 : 1;

            // Tell the EffectsManager that someone is getting an Effect placed on them.
            EffectsManager.Instance.LaunchActiveEffect(activeEffect, activeEffectDetails, PlayerNumber, targetNum);
        }
    }

    protected override void LaunchNone(Effect activeEffect, string activeEffectDetails)
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

    /// <summary>
    /// Animates the Player either running or idle depending on current velocity
    /// </summary>
    private void AnimationRunState()
    {
        // If the Player has a RigidBody, set the animation's speed value to the magnitude of the linear velocity.
        if (_hasRigidBody)
        {
            anim.SetFloat("mSpeed", _rigidBody.linearVelocity.magnitude);
        }
        else
        {
            anim.SetFloat("mSpeed", 0);
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
    /// Runs when _metalDetector invokes an event for proximity to relic.
    /// Accepts a relic if dig is pressed and a relic is found.
    /// </summary>
    private void AcceptRelic(Relic relic, BuriedRelic buriedRelic)
    {
        // If this player has an inventory, and the player is digging, and there is no relic temporarily stored at this moment.
        if (_hasInventory && _isDigging && !_isRelicFound)
        { 
            _isRelicFound = true;
            _relicFound = relic;
            _buriedRelicFound = buriedRelic;
        }
    }

    /// <summary>
    /// Add a relic to the inventory.
    /// </summary>
    /// <param name="relic"> Relic to add to inventory. </param>
    private void AddRelicToInventory(Relic relic)
    {
        // Add it to the inventory.
        _inventory.AddItem(relic);

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
        // OnCycleRelicInHand?.Invoke(false);

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
        // OnCycleRelicInHand?.Invoke(false);
        BuildEffectRelicList();
    }

    /// <summary>
    /// Apply the Lightning Effect.
    /// </summary>
    protected override void ApplyLightning()
    {
        PlayerHit();
    }

    /// <summary>
    /// Applications to the Fog Effect for Players.
    /// The Fog Effect has no extra effects at the moment.
    /// </summary>
    protected override void ApplyFog()
    {
        
    }

    /// <summary>
    /// Application of the Extra Lives Effect for Players.
    /// </summary>
    /// <param name="extraLives"> The number of extra lives as an int. </param>
    /// <param name="extraLivesRelic"> The Relic associated to the active Extra Lives Effect. </param>
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

    /// <summary>
    /// A test function activated by pressing the Hit button according to the input map.
    /// </summary>
    /// <param name="context"> Context of the button press. </param>
    private void HitPlayerTest(InputAction.CallbackContext context)
    {
        if (context.performed) PlayerHit();
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
    /// Method which consumes all relics and cancels their effects.
    /// </summary>
    public void ConsumeAllRelics()
    {
        // Clear the inventory. For reference: this line will not rebuild the effect list on its own.
        _inventory.Clear();

        // Cancel all effects.
        CancelAllEffects();

        // Rebuild the effect relic list.
        BuildEffectRelicList();
    }
}
