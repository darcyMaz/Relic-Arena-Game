using UnityEngine;

/// <summary>
/// Class made for the Excalibur Effect.
/// It requires a Collider and hits Players that enter it.
/// </summary>
public class Excalibur : MonoBehaviour
{
    /// <summary>
    /// Method which runs when a Collider enters the trigger.
    /// If the Collider is a Player, hit that Player.
    /// </summary>
    /// <param name="other"> The Collider overlapping with Excalibur. </param>
    private void OnTriggerEnter(Collider other)
    {
        // Check whether this is a player.
        Player player;
        if (other.gameObject.TryGetComponent(out player))
        {
            // Hit the player!
            player.PlayerHit();
        }
    }
}
