using NUnit.Framework;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    //This script is for when the player enters the area to sell all their relics.

    /// <summary>
    /// The collider of the selling area.
    /// </summary>
    [SerializeField] private CircleCollider2D _collider;

    //Separate inventory values for P1 and P2

    /// <summary>
    /// Player 1's inventory script
    /// </summary>
    private Inventory _P1inventory;
    /// <summary>
    /// List of relics in P1's inventory
    /// </summary>
    private List _P1Relics;

    /// <summary>
    /// Player 2's inventory script
    /// </summary>
    private Inventory _inventoryP2;
    /// <summary>
    /// List of relics in P2's inventory
    /// </summary>
    private List _P2Relics;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if what enters is a Player.
        if (collision.gameObject.CompareTag("Player"))
        {
            _P1inventory = collision.GetComponent<Inventory>();
            
        }
    }

}
