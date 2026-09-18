using UnityEngine;
using UnityEngine.InputSystem;

public interface IDiggable
{
    protected void Dig(InputAction.CallbackContext context);
}
