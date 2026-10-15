using UnityEngine;
// using System.Collections.Generic;

/// <summary>
/// Class made for the Excalibur Effect.
/// It requires a Collider and hits Players that enter it.
/// </summary>
public class Excalibur : MonoBehaviour
{
    /// <summary>
    /// A list holding which Player's have been hit by this excalibur.
    /// </summary>
    // private List<int> playersHit = new List<int>();

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
            /* If OnTriggerEnter is flawed for this, then change it to OnTriggerStay and use this code.
             * It keeps track of who has been hit during the lifetime of the excalibur.
             * Is not a great system for destructable objects.
             * 
            if (!playersHit.Contains(otherPlayer.GetPlayerNumber()))
            {
                // hit player
                // add this player number to list
            }
            */

            // Hit the player!
            otherPlayer.PlayerHit();
        }

        // If Excalibur hit a destructible object.
        Destructibleobject destructibleObject;
        if (other.gameObject.TryGetComponent(out destructibleObject))
        {
            // The destructibleObject is hit!
            destructibleObject.TakeHit();
        }


    }
}
