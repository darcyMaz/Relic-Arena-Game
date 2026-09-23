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
        //First delete all 
    }
}
