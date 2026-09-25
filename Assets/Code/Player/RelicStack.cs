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
        //dsssss0-ews_inventory.OnInventoryChange += StackAdjust;
        _player.OnRelicInHandChanged += StackAdjust;
    }

    /// <summary>
    /// Adjust the stack of relics according to the Inventory.
    /// </summary>
    private void StackAdjust(List<Relic> relicsInInventory)
    {
        Debug.Log("Stacking Relics");
        //First, delete all the current GameObjects

        ClearStackChildren();  

        //Next, spawn a GameObject for each relic in the inventory
        
        //Set what iteration is happening in the foreach
        float i = 0f;
        foreach (Relic relic in _inventory)
        {
            //Increment the index of what relic we're on
            i++;
            Sprite relicSprite = relic.GetSprite();
            Transform transform = GetComponent<Transform>();
            GameObject go = Instantiate(_relicPreFab, transform);
            //Offset the stacking relic
            go.transform.position = new Vector3(transform.position.x, transform.position.y + (2 * i), transform.position.z);
            SpriteRenderer _spriteRenderer = go.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = relicSprite;

        } 
        

    }
    /// <summary>
    /// Function to delete all the GamoeObjects that are children of the player.
    /// </summary>
    private void ClearStackChildren()
    {
        if (_relics != null)
        {
            foreach (Transform child in transform)
            { 
                Destroy(child);
            }
            _relics.Clear();
        }
    }
}
