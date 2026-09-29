using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which manages the selling of Relics.
/// </summary>
public class SellingManager : MonoBehaviour
{
    /// <summary>
    /// The collider of the selling area.
    /// </summary>
    [SerializeField] private SphereCollider _collider;

    /// <summary>
    /// Score manager.
    /// </summary>
    [SerializeField] private ScoreManager _ScoreManager;

    /// <summary>
    /// The list of durations each player can spend in the shop before selling.
    /// </summary>
    [SerializeField] private List<float> _playerSellTimes;

    /// <summary>
    /// The list of 
    /// </summary>
    private List<float> _playerSellTimers = new List<float>();

    /// <summary>
    /// Boolean lock which ensures players only sell once per entry into the sell zone.
    /// </summary>
    private List<bool> _playerSellLocks = new List<bool>();

    /// <summary>
    /// Method played on awake.
    /// Initializes the _playerSellTimes and _playerSellLocks lists.
    /// </summary>
    private void Awake()
    {
        // For each time duration representing a player, add a timer and set it to infinity.
        // Set all of the locks to true.
        _playerSellTimes.ForEach((time) => { _playerSellTimers.Add(Mathf.Infinity); _playerSellLocks.Add(true); });
    }

    private void Update()
    {
        // Decrement all timers by delta time if they are greater than 0.
        for (int index = 0; index < _playerSellTimers.Count; index++)
        {
            _playerSellTimers[index] = _playerSellTimers[index] - Time.deltaTime;
            if (_playerSellTimers[index] < 0)
            {
                _playerSellTimers[index] = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Entered trigger zone.");
        Player player;
        if (collision.TryGetComponent(out player))
        {
            //Debug.Log("\t found player");
            int playerNum = player.GetPlayerNumber();
            int indexCorrectPlayerNum = playerNum - 1;

            // If this is a valid player.
            if (IsValidPlayer(playerNum))
            {
                //Debug.Log("Valid player");
                
                // Set the timer to its time duration.
                // The update function will decrement the timer, and OnTriggerStay will watch for access to selling.
                _playerSellTimers[indexCorrectPlayerNum] = _playerSellTimes[indexCorrectPlayerNum];

                // Unlock the selling mechanic.
                _playerSellLocks[indexCorrectPlayerNum] = false;

                
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Get the Player if this is one.
        Player player;
        if (other.TryGetComponent(out player))
        {
            // Get the player's number to check if it is valid.
            int playerNum = player.GetPlayerNumber();
            int indexCorrectPlayerNum = playerNum - 1;

            // Get the player's number and check whether it is a valid number.
            if (IsValidPlayer(playerNum))
            {
                //Debug.Log("play 1 timer: " + _playerSellTimers[indexCorrectPlayerNum]);

                // If the player selling mechanic has not been locked and the timer is zero.
                if (!_playerSellLocks[indexCorrectPlayerNum] && _playerSellTimers[indexCorrectPlayerNum] <= 0)
                {
                    // Lock the selling until they leave the zone.
                    _playerSellLocks[indexCorrectPlayerNum] = true;

                    // Income from all of the relics.
                    float income = SellRelics(player);

                    // Inform the ScoreManager of this change.
                    _ScoreManager.PlayerScore(income, playerNum);

                    // Clear the player's inventory.
                    player.ConsumeAllRelics();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player;
        if (other.TryGetComponent(out player))
        {
            // Get the player's number to check if it is valid.
            int playerNum = player.GetPlayerNumber();
            int indexCorrectPlayerNum = playerNum - 1;

            // Get the player's number and check whether it is a valid number.
            if (IsValidPlayer(playerNum))
            {
                // Set the timer to zero.
                _playerSellTimers[indexCorrectPlayerNum] = Mathf.Infinity;
                // Lock the selling mechanic.
                _playerSellLocks[indexCorrectPlayerNum] = true;
            }
        }
    }

    /// <summary>
    /// Method which validates whether a Player's number is valid for use in the SellingManager.
    /// Checks whether the number is greater than 0 and is not equal or greater than the size of the time duration list.
    /// </summary>
    /// <param name="playerNum"> The Player Number as an int. </param>
    /// <returns> Whether it is valid as a bool. </returns>
    private bool IsValidPlayer(int playerNum)
    {
        if (playerNum >= _playerSellTimes.Count || playerNum < 1)
        {
            Debug.LogError("A Player entered the SellingManager's zone but its Player Number was invalid (less then 1 or greater than the size of the _playerSellTimes list).");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Method which sells relics and returns their value after passing through an exponential formula.
    /// </summary>
    /// <param name="player"> The Player whose relics will be sold. </param>
    /// <returns> The value of the Relics as a float. </returns>
    private float SellRelics(Player player)
    {
        // Check if the player has an inventory.
        Inventory inventory;
        if (player.gameObject.TryGetComponent(out inventory))
        {
            // Values to help calculate the income for selling these relics.
            float relicSubtotal = 0f;
            float relicTotal = 0;

            foreach (Relic relic in inventory)
            {
                relicSubtotal += relic.GetPrice();
            }

            // Calculate the real value, according to an exponential formula.
            relicTotal = (relicSubtotal * (1f + (inventory.Count() * 0.1f)));

            // Return the value.
            return relicTotal;
        }
        return 0;
    }
}
