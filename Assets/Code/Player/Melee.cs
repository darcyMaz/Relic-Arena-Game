using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : MonoBehaviour
{
    public int damage = 1;
    public int HitPoints;
    [SerializeField] private float range = 2f;
    [SerializeField] private Vector3 offset = new Vector3(1.5f, 0f, 0f);

 void Update()
{
        if (Keyboard.current.rKey.wasPressedThisFrame)
    Swing();
}

    public void Swing()
    {
        Vector3 center = transform.position + transform.rotation * offset;

        foreach (Collider hit in Physics.OverlapSphere(center, range))
        if (hit.TryGetComponent(out Destructibleobject target))
        target.TakeHit(damage);
    }
 
}
