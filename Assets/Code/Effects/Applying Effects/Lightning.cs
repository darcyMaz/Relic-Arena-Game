using System.Threading.Tasks;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class Lightning : MonoBehaviour
{
    /// <summary>
    /// The LineRenderer represents the bolt of lightning to hit an IEffectable.
    /// </summary>
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Determines how many kinks there are in the lightning strike line.
    /// </summary>
    [SerializeField] private int TotalKinksInLine = 6;

    /// <summary>
    /// Anchor position at which to start the lightning strike.
    /// </summary>
    [SerializeField] private Vector3 AnchorPosition;

    public async void LightningStrike(Vector3 endPos)
    {
        // Create the lightning line    
        BuildLine(endPos);

        await Task.Delay(750);

        // destroy the lightning line
        RemoveLine();
    }

    /// <summary>
    /// Builds the line using the line renderer which represents the lightning strike.
    /// </summary>
    private void BuildLine(Vector3 endPos)
    {
        // Set the positionCount to be Anchor+TotalKinks+End


        for (int i = 0; i < TotalKinksInLine; i++)
        {

        }

    }

    private void RemoveLine()
    {
        _lineRenderer.positionCount = 0;
        _lineRenderer.SetPositions(new Vector3[0]);
    }
}
