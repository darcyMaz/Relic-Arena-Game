using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    //This script is for when the player enters the area to sell all their relics.

    /// <summary>
    /// The collider of the selling area.
    /// </summary>
    [SerializeField] private SphereCollider _collider;

    //Separate inventory values for P1 and P2

    /// <summary>
    /// Player 1's inventory script
    /// </summary>
    private Inventory _P1inventory;
    /// <summary>
    /// Player 2's inventory script
    /// </summary>
    private Inventory _P2inventory;
    /// <summary>
    /// List of relics in P1's inventory
    /// </summary>
    private List _P1Relics;
    /// <summary>
    /// List of relics in P2's inventory
    /// </summary>
    private List _P2Relics;

    /// <summary>
    /// When timer is over a set time, P1 sells their items.
    /// </summary>
    [SerializeField] private float _P1SellTimer;
    /// <summary>
    /// When timer is over a set time, P2 sells their items.
    /// </summary>
    [SerializeField] private float _P2SellTimer;



    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        //Gather the inventory of the object entering.
        Inventory inventory;
        if (collision.TryGetComponent(out inventory))
        {
            Debug.Log("Something with an inventory entered the shop");
            Player player;
            if (collision.TryGetComponent(out player))
            {
                int playerID;
                playerID = player.GetPlayerNumber();
                if (playerID == 1)
                {
                    _P1inventory = inventory;
                    
                }
                else if(playerID == 2)
                {
                    _P2inventory = inventory;

                }
            }
            else
            {
                Debug.Log("Inventory holder does not have a Player script");
            }
            
            //SellAll();
        }
        else
        {
            Debug.Log("Something that doesn't have an inventory entered the shop.");
        }


    }

    private void OnTriggerLeave(Collider collision)
    {
        //Check if what enters is a Player.
        if (collision.gameObject.CompareTag("Player"))
        {
            //_P1inventory = collision.GetComponent<Inventory>();
            Debug.Log("Player Left");
        }
    }
    private void SellAll()
    {
        foreach (Relic relic in _P1inventory)
        {
            Debug.Log(relic.GetName());
        }
    }
}
