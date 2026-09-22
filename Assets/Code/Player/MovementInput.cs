using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads and interprets movement input using the New Input System and a Rigidbody.
/// </summary>
// [RequireComponent (typeof(Rigidbody))]
public class MovementInput : MonoBehaviour
{

    /// <summary>
    /// Set of input actions.
    /// </summary>
    private InputSystem_Actions _actions;
    /// <summary>
    /// The movement Input Action.
    /// </summary>
    private InputAction _move;

    /// <summary>
    /// Character controller on this gameObject.
    /// </summary>
    // private CharacterController _characterController;

    /// <summary>
    /// Rigidbody on the player. Used to move and for collisions.
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// Base movement speed.
    /// </summary>
    [SerializeField] private float BaseSpeed = 2f;

    /// <summary>
    /// Dynamic movement speed.
    /// </summary>
    private float _speed;

    private Vector3 _input = Vector3.zero;
    private Vector3 _gravity = Vector3.zero;
    [SerializeField] private float GravityFloat = 9.8f;

    /// <summary>
    /// Initialization before the game starts.
    /// </summary>
    private void Awake()
    {
        ActionsInit();
        _speed = BaseSpeed;
    }

    /// <summary>
    /// Initialization as the game is starting.
    /// </summary>
    private void Start()
    {
        ComponentsInit();
    }

    /// <summary>
    /// When enabling the game object.
    /// </summary>
    private void OnEnable()
    {
        _move = _actions.Player.Move;
        _move.Enable();
    }
    /// <summary>
    /// When disabling the game object.
    /// </summary>
    private void OnDisable()
    {
        _move.Disable();
    }

    /// <summary>
    /// Every frame, check for movement.
    /// </summary>
    private void FixedUpdate()
    {
        Input();
        Gravity();
        Move();
    }

    /// <summary>
    /// Initialize the actions.
    /// </summary>
    private void ActionsInit()
    {
        _actions = new InputSystem_Actions();
    }

    /// <summary>
    /// Initialize the components.
    /// </summary>
    private void ComponentsInit()
    {
        // _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Set the movement speed.
    /// </summary>
    /// <param name="speed"> The new speed. </param>
    public void AffectSpeed(float multiplier)
    {
        _speed = BaseSpeed * multiplier;
    }

    /// <summary>
    /// Get the movement speed.
    /// </summary>
    /// <returns> The speed of this gameObject as a float. </returns>
    public float GetCurrentSpeed()
    {
        return _speed;
    }

    /// <summary>
    /// Check for input.
    /// </summary>
    private void Input()
    {
        // Poll the movement from the Input Action.
        Vector2 pollMovement = _move.ReadValue<Vector2>();

        // Set the _input vector such that Speed and normalization are accounted for.
        _input = pollMovement.normalized * _speed;
    }

    /// <summary>
    /// Set the gravity.
    /// </summary>
    private void Gravity()
    {
        _gravity = new Vector3(0,-GravityFloat,0);
    }

    /// <summary>
    /// Move the character controller.
    /// </summary>
    private void Move()
    {
        // _characterController.Move( (_gravity + _input) * Time.fixedDeltaTime);
        _rigidbody.linearVelocity = (_gravity + _input) * Time.fixedDeltaTime;
    }

    
}
