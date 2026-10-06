using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public event Action OnZombieDeath;

    [SerializeField] private Transform _target;
    [SerializeField] private float _munchTime = 2f;
    private float _munchTimer = Mathf.Infinity;
    [SerializeField] private float _smoothness = 2f;

    // Update is called once per frame
    void Update()
    {
        // If the timer is infinity, keep it that way.
        // Else if the timer is greater than 0, decrement the timer.
        // Else (if less than or equal to zero) keep it zero.
        _munchTimer =  
            (_munchTimer == Mathf.Infinity) ? Mathf.Infinity: 
            (_munchTimer > 0) ? _munchTimer - Time.deltaTime: 
            0;
        Chase();
        DeathCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        // start counting down
        _munchTimer = _munchTime;
    }
    private void OnTriggerExit(Collider other)
    {
        // Reset the timer to infinity.
        _munchTimer = Mathf.Infinity;
    }

    private void Chase()
    {
        // Chase the target.
        transform.position = Vector3.Lerp(transform.position, _target.position, _smoothness);
    }

    private void DeathCheck()
    {
        if (_munchTimer <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        // Inform listeners of the death of the zombie.
        OnZombieDeath?.Invoke();

        // Destroy the gameObject.
        Destroy(gameObject);
    }

    /// <summary>
    /// Set the zombie's target.
    /// </summary>
    /// <param name="target"> The zombie's target as a Transform. </param>
    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
