using UnityEngine;
using UnityEngine.InputSystem;

public class BulletSpawner : MonoBehaviour
{

    //For testing purposes, click the left mouse button to spawn bullet.

    /// <summary>
    /// The position where the bullet spawns.
    /// </summary>
    public Transform shootPoint;

    /// <summary>
    /// Type of bullet prefab being spawned.
    /// </summary>
    public GameObject bullet;

    /// <summary>
    /// Set of input actions.
    /// </summary>
    private InputSystem_Actions _actions;
    /// <summary>
    /// The Fire Input Action.
    /// </summary>
    private InputAction _fire;

    /// <summary>
    /// Spawns different types of bullets depending on arguement.
    /// </summary>
    /// <param name="bullet">The type of bullet prefab being spawned.</param>
    public void SpawnBullet(GameObject bullet)
    {
        Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        Debug.Log("Bang!");
    }

    private void Awake()
    {
        ActionsInit();
    }

    /// <summary>
    /// When enabling the game object.
    /// </summary>
    private void OnEnable()
    {
        _fire = _actions.Player.Attack;
        _fire.Enable();
        _fire.performed += onFirePerformed;
    }
    /// <summary>
    /// When disabling the game object.
    /// </summary>
    private void OnDisable()
    {
        _fire.performed -= onFirePerformed;
        _fire.Disable();
    }

    /// <summary>
    /// Initialize the actions.
    /// </summary>
    private void ActionsInit()
    {
        _actions = new InputSystem_Actions();
    }

    private void onFirePerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpawnBullet(bullet);
        }
    }
}
