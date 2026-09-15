using UnityEngine;
using UnityEngine.InputSystem;

public class InputMovement2D : MonoBehaviour
{
    [SerializeField] private float Speed = 2f;

    private InputSystem_Actions _inputActions;
    private InputAction _movement;

    private Rigidbody2D _rigidBody;
    private bool _hasRB = false;

    private void Awake()
    {
        // Initialize the Input System object.
        _inputActions = new InputSystem_Actions();

        // Try to get the RigidBody2D
        if (TryGetComponent(out _rigidBody)) _hasRB = true;
        else Debug.Log("A PlayerMovement component could not find its respective Rigidbody2D.");
    }

    private void OnEnable()
    {
        // Retrieve and enable the movement input.
        _movement = _inputActions.Player.Move;
        _movement.Enable();
    }
    private void OnDisable()
    {
        // Disable the movement input.
        _movement.Disable();
    }

    private void Update()
    {
        // Call the move function.
        if (_hasRB) Move();
    }
    private void Move()
    {
        // Poll for movement
        Vector2 movement = _movement.ReadValue<Vector2>();

        // Change the linear velocity to apply the movement.
        // Movement for player's in the game is stiff, no momentum. This is all the movement needs.
        _rigidBody.linearVelocity = movement.normalized * Speed;
    }

}
