using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class made for the Excalibur Effect.
/// It requires a Collider and hits Players that enter it.
/// </summary>
public class Excalibur : MonoBehaviour
{
    /// <summary>
    /// A list holding which Player's have been hit by this excalibur.
    /// </summary>
    private List<int> playersHit = new List<int>();

    /// <summary>
    /// A list holding Destructibleobjects that have been hit so they won't get hit twice.
    /// </summary>
    private List<Destructibleobject> destructibleObjects = new List<Destructibleobject>();

    /// <summary>
    /// Method which runs when a Collider enters the trigger.
    /// If the Collider is a Player, hit that Player.
    /// </summary>
    /// <param name="other"> The Collider overlapping with Excalibur. </param>
    private void OnTriggerEnter(Collider other)
    {
        // Check whether this is a player.
        Player otherPlayer;
        if (other.gameObject.TryGetComponent(out otherPlayer))
        {
            // This ensures that the Player does not get hit twice.
            if (!playersHit.Contains(otherPlayer.GetPlayerNumber()))
            {
                // Hit the player!
                otherPlayer.PlayerHit();

                // Add this player's number to the list so it won't get hit again.
                playersHit.Add(otherPlayer.GetPlayerNumber());
            }
        }

        // If Excalibur hit a destructible object.
        Destructibleobject destructibleObject;
        if (other.gameObject.TryGetComponent(out destructibleObject))
        {
            // This ensures that the destructibleobject does not get hit twice.
            if (!destructibleObjects.Contains(destructibleObject))
            {
                // The destructibleObject is hit!
                destructibleObject.TakeHit();

                // Add it to the list so it won't get hit again.
                destructibleObjects.Add(destructibleObject);
            }

            
        }
    }
}
