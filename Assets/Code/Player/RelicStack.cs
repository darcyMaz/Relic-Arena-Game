using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles displaying the relics above the player.
/// </summary>
public class RelicStack : MonoBehaviour
{
    /// <summary>
    /// The player attached to this gameObject.
    /// </summary>
    private Player _player;
    private bool _hasPlayer = false;

    /// <summary>
    /// Relic Icon Prefab
    /// </summary>
    [SerializeField] private GameObject _relicPreFab;

    ///<summary>
    /// Relic per relic spacing distance in Stack
    /// </summary>
    [SerializeField] private float relicSpacing = 2;

    /// <summary>
    /// The list of relics that are displayed on the player's head.
    /// </summary>
    private List<GameObject> _relics = new List<GameObject>();

    private void Awake()
    {
        if (TryGetComponent(out _player))
        {
            _hasPlayer = true;
        }
    }

    private void OnEnable()
    {
        if (_hasPlayer) _player.OnRelicInHandChanged += StackAdjust;
    }
    private void OnDisable()
    {
        if (_hasPlayer) _player.OnRelicInHandChanged -= StackAdjust;
    }

    /// <summary>
    /// Adjust the stack of relics according to the Inventory.
    /// </summary>
    private void StackAdjust(List<Relic> relicsInInventory)
    {
        //First, delete all the current GameObjects
        ClearStackChildren();  

        //Next, spawn a GameObject for each relic in the inventory
        
        //Set what iteration is happening in the foreach
        float i = 0f;
        foreach (Relic relic in relicsInInventory)
        {
            //Increment the index of what relic we're on
            i++;
            Sprite relicSprite = relic.GetSprite();
            // Transform transform = GetComponent<Transform>();
            GameObject go = Instantiate(_relicPreFab, transform);
            
            //Offset the stacking relic
            go.transform.position = new Vector3(transform.position.x, transform.position.y + (relicSpacing * i), transform.position.z);
            
            // Change the sprite to reflect the relic's sprite.
            SpriteRenderer _spriteRenderer = go.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = relicSprite;
            
            // Add the GameObject to the list.
            _relics.Add(go);
        } 
        

    }
    /// <summary>
    /// Function to delete all the GamoeObjects that are children of the player.
    /// </summary>
    private void ClearStackChildren()
    {
        if (_relics != null)
        {
            foreach (GameObject child in _relics)
            { 
                Destroy(child);
            }
            _relics.Clear();
        }
    }
}
