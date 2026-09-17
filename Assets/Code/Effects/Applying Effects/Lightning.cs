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
    [SerializeField] private Transform AnchorPosition;

    /// <summary>
    /// A value between 0 and 1 that determines how exaggerated the lightning kinks will look.
    /// </summary>
    [SerializeField] private float LightningKinkExaggeration = 0;
    /// <summary>
    /// The maximum distance for which a lightning can kink if LightningKinkExaggeration is 1.
    /// </summary>
    [SerializeField] private float LightningKinkDistance = 3;

    /// <summary>
    /// The lifetime of the lightning strike in milliseconds.
    /// </summary>
    [SerializeField] private int LightningLifetime = 750;

    /// <summary>
    /// Initializations before the game starts.
    /// </summary>
    private void Awake()
    {
        InitComponents();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endPos"></param>
    public async void LightningStrike(Vector3 endPos)
    {
        // Create the lightning line    
        BuildLine(endPos);

        // Allow the lightning to exist for a short time.
        await Task.Delay(LightningLifetime);

        // Destroy the lightning line
        RemoveLine();
    }

    /// <summary>
    /// Builds the line using the line renderer which represents the lightning strike.
    /// This function 
    ///     (1) Creates a vector between the start and end of the lightning strike,
    ///     (2) Cuts this line by the number of kinks set in the inspector,
    ///     (3) Builds the lightning strike with the line renderer based on those cuts, where those cuts have small variance.
    ///         This gives the line the appearance of a straight line with kinks, i.e. a lightning strike.
    /// </summary>
    private void BuildLine(Vector3 endPos)
    {
        // Set the positionCount to be Anchor+TotalKinks+End
        int positionCount = TotalKinksInLine + 2;
        int positionIndex = 0;

        _lineRenderer.positionCount = positionCount;

        // Set the first position as the anchor.
        _lineRenderer.SetPosition(positionIndex++, AnchorPosition.position);

        // Build the vector that represents the direction from start to finish
        Vector3 direction = (endPos - AnchorPosition.position).normalized;

        // Get the distance from the start to the finish
        float distance = Vector3.Distance(AnchorPosition.position, endPos);

        // This for loop gets points between the start and end of the line.
        // Those points are equidistant, and have as many cuts as there are TotalKinks.
        // These points won't be added to the _lineRenderer, rather, points a bit off will be added.
        for (; positionIndex < positionCount - 1; positionIndex++)
        {
            // This point represents the equidistant cut on the line, each loop grabs the next cut.
            Vector3 pointAhead = AnchorPosition.position + (direction * (distance * ((float) positionIndex/ (float) (TotalKinksInLine+1)) ) );

            // Create the random variance, then add that new point to the line renderer.
            _lineRenderer.SetPosition(positionIndex, SmallRandomVariance(pointAhead));
        }

        // Set the final position as the last position on the line.
        _lineRenderer.SetPosition(positionIndex, endPos);
    }

    /// <summary>
    /// End the lightning strike by removing the line from LineRenderer.
    /// </summary>
    private void RemoveLine()
    {
        _lineRenderer.positionCount = 0;
        _lineRenderer.SetPositions(new Vector3[0]);
    }

    /// <summary>
    /// Take a Vector3 point as input and randomly move its three values according to a range.
    /// </summary>
    /// <param name="point"> The input point. </param>
    /// <returns> The input point with random adjustments. </returns>
    private Vector3 SmallRandomVariance(Vector3 point)
    {
        // Point to change and return.
        Vector3 newPoint = point;

        // Range to change the point.
        float range = LightningKinkDistance * LightningKinkExaggeration;
        
        // Randomly alter point based on range.
        newPoint.x = UnityEngine.Random.Range(newPoint.x - range, range + newPoint.x);
        newPoint.y = UnityEngine.Random.Range(newPoint.y - range, range + newPoint.y);
        newPoint.z = UnityEngine.Random.Range(newPoint.z - range, range + newPoint.z);

        return newPoint;
    }

    private void InitComponents()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
}
