using NUnit.Framework;
using System.Linq;
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

    //Countdown timers for relics.
    private float P1timer = 0f;
    private float P2timer = 0f;
    private bool P1timerActive = false;
    private bool P2timerActive = false;

    private void Update()
    {
        if (P1timerActive)
        {
            P1timer += Time.deltaTime;
            if (P1timer >= 2f)
            {
                P1timerActive = false;
                P1timer = 0f;
                P1SellRelics();

            }
        }
        if (P2timerActive)
        {
            P2timer += Time.deltaTime;
            if (P2timer >= 2f)
            {
                P2timerActive = false;
                P2timer = 0f;
                P2SellRelics();

            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        //Gather the inventory of the object entering.
        Inventory inventory;
        if (collision.TryGetComponent(out inventory))
        {
            
            Player player;
            if (collision.TryGetComponent(out player))
            {
                int playerID;
                playerID = player.GetPlayerNumber();
                if (playerID == 1)
                {
                    Debug.Log("P1 entered the shop");
                    _P1inventory = inventory;
                    P1timerActive = true;
                }
                else if(playerID == 2)
                {
                    Debug.Log("P2 entered the shop");
                    _P2inventory = inventory;
                    P2timerActive = true;
                }
            }
            else
            {
                Debug.Log("Inventory holder does not have a Player script");
            }
            
        }
        else
        {
            Debug.Log("Something that doesn't have an inventory entered the shop.");
        }


    }

    //When a player leaves, the sell timer resets. Doesn't work. Will need to investigate further.
    private void OnTriggerLeave(Collider collision)
    {
        Player player;
        if (collision.TryGetComponent(out player))
        {
            int playerID;
            playerID = player.GetPlayerNumber();
            if (playerID == 1)
            {
                P1timer = 0f;
                P1timerActive= false;
                Debug.Log("Player1 Left");
            }
            else if (playerID == 2)
            {
                P2timer = 0f;
                P2timerActive= false;
                Debug.Log("Player2 Left");

            }
            
        }
        
        
        
    }
    /// <summary>
    /// Takes P1's inventory and sells it, clearing their inventory and giving them score.
    /// </summary>
    private void P1SellRelics()
    {
        Debug.Log("Player 1 sells their relics.");
        float relicSubtotal = 0f;
        float relicTotal = 0;

        foreach (Relic relic in _P1inventory)
        {
            relicSubtotal += relic.GetPrice();
        }

        Debug.Log("Total Price: " + relicSubtotal);
        Debug.Log("Total amount of relics: " + _P1inventory.Count());

        relicTotal = (relicSubtotal * (1f + (_P1inventory.Count() * 0.1f)));
        Debug.Log("Total with multiplier: " + relicTotal);

        //line of code that adds the total to the score goes here

        //Remove all the relics from their inventory.
        foreach (Relic relic in _P1inventory)
        {
            _P1inventory.RemoveItem(relic);
        }
 
    }

    private void P2SellRelics()
    {
        Debug.Log("Player 2 sells their relics.");
        float relicSubtotal = 0f;
        float relicTotal = 0;

        foreach (Relic relic in _P2inventory)
        {
            relicSubtotal += relic.GetPrice();
        }

        Debug.Log("Total Price: " + relicSubtotal);
        Debug.Log("Total amount of relics: " + _P2inventory.Count());

        relicTotal = (relicSubtotal * (1f + (_P2inventory.Count() * 0.1f)));
        Debug.Log("Total with multiplier: " + relicTotal);

        //line of code that adds the total to the score goes here

        //Remove all the relics from their inventory.
        foreach (Relic relic in _P2inventory)
        {
            _P2inventory.RemoveItem(relic);
        }
    }
}
