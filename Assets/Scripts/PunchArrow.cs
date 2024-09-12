using UnityEngine;

public class PunchArrow : Coin
{
    public Transform playerHand; // Reference to the player's hand (which has the punch direction and speed)
    public float timingWindow = 1.5f; // The window in which the punch timing should occur (in seconds)
    public float maxTimingScore = 100f; // Maximum score for perfect timing
    public float maxSpeedScore = 100f; // Maximum score for punch speed
    public float maxAngleScore = 100f; // Maximum score for perfect angle

    private float timeSinceStart = 0f; // Track the time since the punch arrow appeared
    private bool punchRegistered = false;

    void Start()
    {
        timeSinceStart = 0f; // Reset the timer when the punch arrow spawns
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime; // Increment the timer
    }

    public override void OnCollisionEvent(Collision collision)
    {
        if (punchRegistered) return; // Ignore multiple punches

        if (collision.gameObject == playerHand.gameObject)
        {
            // Calculate all factors for point calculation
            float timingPoints = CalculateTimingPoints();
            float anglePoints = CalculateAnglePoints();
            float speedPoints = CalculateSpeedPoints(collision);

            // Sum all points
            float totalPoints = timingPoints + anglePoints + speedPoints;
            Debug.Log($"Punch Successful! Total Points: {totalPoints}");

            // Modify points in PointsManager
            if (PointsManager.Instance != null)
            {
                PointsManager.Instance.ModifyPoints((int)totalPoints);
            }

            punchRegistered = true; // Prevent multiple punches from being registered
        }
    }

    private float CalculateTimingPoints()
    {
        // The ideal time is half of the timingWindow
        float idealTime = timingWindow / 2f;
        float timeDifference = Mathf.Abs(timeSinceStart - idealTime);

        // The further from the ideal time, the fewer points (clamped to zero)
        float timingPoints = Mathf.Max(0, maxTimingScore - (timeDifference / idealTime) * maxTimingScore);
        Debug.Log($"Timing Points: {timingPoints}");
        return timingPoints;
    }

    private float CalculateAnglePoints()
    {
        // X-axis of the PunchArrow object (local forward direction)
        Vector3 punchArrowXAxis = transform.forward;

        // Get the direction of the punch (player's hand velocity normalized)
        Vector3 punchDirection = playerHand.GetComponent<Rigidbody>().velocity.normalized;

        // Calculate the angle between the punch direction and the object's X-axis
        float angle = Vector3.Angle(punchDirection, punchArrowXAxis);

        // Normalize angle (0 degrees = max points, 90 degrees = no points)
        float anglePoints = Mathf.Max(0, maxAngleScore - (angle / 90f) * maxAngleScore);
        Debug.Log($"Angle Points: {anglePoints}");
        return anglePoints;
    }

    private float CalculateSpeedPoints(Collision collision)
    {
        // Speed is based on the velocity magnitude of the player's hand Rigidbody at the point of collision
        float punchSpeed = collision.relativeVelocity.magnitude;

        // Normalize speed to give more points for higher speeds
        // Assuming maxSpeedScore is achieved at some max speed threshold, say 10 units/second
        float maxSpeedThreshold = 10f;
        float speedPoints = Mathf.Min(maxSpeedScore, (punchSpeed / maxSpeedThreshold) * maxSpeedScore);
        Debug.Log($"Speed Points: {speedPoints}");
        return speedPoints;
    }
}
