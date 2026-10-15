using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : MonoBehaviour
{

    /// <summary>
    /// Set of input actions.
    /// </summary>
    private InputSystem_Actions _actions;
    /// <summary>
    /// The melee Input Action.
    /// </summary>
    private InputAction _melee;
    /// <summary>
    /// The Player's number, set in the inspector.
    /// </summary>
    [SerializeField] private int PlayerNumber = 0;
    [SerializeField] private float range = 2f;
    [SerializeField] private Vector3 offset = new Vector3(1.5f, 0f, 0f);

    /// <summary>
    /// Method which runs on awake.
    /// </summary>
    private void Awake()
    {
        _actions = new InputSystem_Actions();
    }

    /// <summary>
    /// Runs when this gameObject is Enabled.
    /// </summary>
    private void OnEnable()
    {
        if (PlayerNumber == 1)
        {
            _melee = _actions.Player1.Melee;
        }
        else if (PlayerNumber == 2)
        {
            _melee = _actions.Player2.Melee;
        }
        _melee.performed += MeleeAttack;
        _melee.Enable();
    }

    /// <summary>
    /// When this gameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        _melee.performed -= MeleeAttack;
        _melee.Disable();
    }

    private void MeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed) Swing();
    }
    void Update()
{
        //if (Keyboard.current.rKey.wasPressedThisFrame)
    //Swing();
}


    public void Swing()
    {
        Vector3 center = transform.position + transform.rotation * offset;

        foreach (Collider hit in Physics.OverlapSphere(center, range)){

            if (hit.gameObject == gameObject)
            {
                continue;
            }

        if (hit.TryGetComponent(out Destructibleobject target))
        {
        target.TakeHit();
        }
       /* if (hit.TryGetComponent(out MovementInput target2)){
        target2.Pushback(transform.position);
        }
        
        */
        }
    }
 
}
