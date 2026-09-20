using UnityEngine;

public class BulletTemplate : MonoBehaviour
{

    //This script is a template for bullet behaviour. It can be updated for all bullet pattern behaviour.
    //This one just shoots forward.

    /// <summary>
    /// The bullet's Rigidbody.
    /// </summary>
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private float _speed = 20f;

    private void Awake()
    {
        _rb.linearVelocity = transform.up * _speed;
    }
}
