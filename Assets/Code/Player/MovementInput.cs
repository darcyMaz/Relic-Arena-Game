using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads and interprets movement input using the New Input System and a CharacterController.
/// </summary>
[RequireComponent (typeof(CharacterController))]
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
    private CharacterController _characterController;

    /// <summary>
    /// Movement speed.
    /// </summary>
    [SerializeField] private float Speed = 2f;

    private Vector3 _input = Vector3.zero;
    private Vector3 _gravity = Vector3.zero;
    [SerializeField] private float GravityFloat = 9.8f;

    /// <summary>
    /// Initialization before the game starts.
    /// </summary>
    private void Awake()
    {
        ActionsInit();
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
    /// Set the movement speed.
    /// </summary>
    /// <param name="speed"> The new speed. </param>
    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    /// <summary>
    /// Get the movement speed.
    /// </summary>
    /// <returns> The speed of this gameObject as a float. </returns>
    public float GetSpeed()
    {
        return Speed;
    }

    /// <summary>
    /// Check for input.
    /// </summary>
    private void Input()
    {
        // Poll the movement from the Input Action.
        Vector2 pollMovement = _move.ReadValue<Vector2>();

        // Translate the polled value from Vect2 to vect3.
        Vector3 movement = new Vector3(pollMovement.x, 0, pollMovement.y);

        // Set the _input vector such that Speed and normalization are accounted for.
        _input = movement.normalized * Speed;
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
        _characterController.Move( (_gravity + _input) * Time.fixedDeltaTime);
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
        _characterController = GetComponent<CharacterController>();
    }
}
