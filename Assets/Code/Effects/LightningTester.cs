using UnityEngine;
using UnityEngine.InputSystem;

public class LightningTester : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame){
                EffectsManager.Instance.Lightning(target.position);
        }
    }
}
