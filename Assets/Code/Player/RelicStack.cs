using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RelicStack : MonoBehaviour
{
    //This script handles displaying the relics above the player.

    //Calling player's scripts.
    [SerializeField] private Player _player;
    [SerializeField] private Inventory _inventory;
    //Relic Icon Prefab
    [SerializeField] private GameObject _relicPreFab;
    /// <summary>
    /// The list of relics that are displayed on the player.
    /// </summary>
    private List<GameObject> _relics;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _inventory = GetComponent<Inventory>();

    }

    private void Start()
    {

        //Subscribe to the inventory change event
        _inventory.OnInventoryChange += StackAdjust;
    }

    /// <summary>
    /// Adjust the stack of relics according to the Inventory.
    /// </summary>
    private void StackAdjust(IEnumerator<Relic> relicsInInventory)
    {
        Debug.Log("Stacking Relics");
        //First, delete all the current GameObjects
        if (_relics != null)
        {
            foreach (GameObject relic in _relics)
            {
                GameObject.Destroy(relic);
            }
            _relics.Clear();
        }

        //Next, spawn a GameObject for each relic in the inventory
        
        //Set what iteration is happening in the foreach
        float i = 0f;
        foreach (Relic relic in _inventory)
        {
            i++;
            Instantiate(_relicPreFab, new Vector3(transform.position.x, (transform.position.y + (2f * i)), transform.position.z), transform.rotation);

        }
        

    }
}
