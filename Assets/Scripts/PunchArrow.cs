using UnityEngine;

public class PunchArrow : Coin
{
    public Transform player; // Reference to the player's transform (e.g., head or body)
    public float playerDistanceWindow = 1.5f; // The ideal distance from the punch arrow
    public float maxTimingScore = 100f; // Maximum score for perfect timing
    public float maxSpeedScore = 100f; // Maximum score for punch speed
    public float maxAngleScore = 100f; // Maximum score for perfect angle
    public float maxDistance = 3.0f; // Maximum distance before it's considered a miss
    public float maxAllowedAngle = 30f; // Maximum angle deviation (in degrees) for a valid punch

    private bool punchRegistered = false;

    public override void OnCollisionEvent(Collision collision)
    {
        if (punchRegistered) return; // Ignore multiple punches

        if (collision.gameObject.layer.Equals("Hand"))
        {
            // Calculate angle first; if it's a miss, exit early
            float anglePoints = CalculateAnglePoints(collision);
            if (anglePoints == 0)
            {
                Debug.Log("Punch missed due to wrong angle.");
                punchRegistered = true;
                return; // Early exit as it's a miss
            }

            // Calculate all factors for point calculation
            float timingPoints = CalculateTimingPoints();
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer.Equals("Hand"))
        {
            // Distance from the player to the punch arrow
            float distanceToPlayer = (transform.position - player.position).magnitude;
            Debug.Log($"Distance to Player: {distanceToPlayer}");
        }
    }

    private float CalculateTimingPoints()
    {
        // Calculate the distance between the player and the punch arrow
        float distanceToPlayer = (transform.position - player.position).magnitude;

        // If the player is farther than maxDistance, it's a miss and no points are awarded
        if (distanceToPlayer > maxDistance)
        {
            Debug.Log("Player missed the punch (too far).");
            return 0f;
        }

        // The closer the player is to the ideal distance, the more points they get
        float distanceDifference = Mathf.Abs(distanceToPlayer - playerDistanceWindow);

        // Calculate points based on how close the player is to the ideal distance
        float timingPoints = Mathf.Max(0, maxTimingScore - (distanceDifference / playerDistanceWindow) * maxTimingScore);
        Debug.Log($"Timing Points: {timingPoints}");

        return timingPoints;
    }

    private float CalculateAnglePoints(Collision collision)
    {
        // X-axis of the PunchArrow object (local forward direction)
        Vector3 punchArrowXAxis = transform.forward;

        // Get the direction of the punch (player's hand velocity normalized)
        Vector3 punchDirection = collision.gameObject.GetComponent<Rigidbody>().velocity.normalized;

        // Calculate the angle between the punch direction and the object's X-axis
        float angle = Vector3.Angle(punchDirection, punchArrowXAxis);

        // If the angle is greater than the maxAllowedAngle, it's considered a miss
        if (angle > maxAllowedAngle)
        {
            Debug.Log($"Punch missed due to large angle: {angle} degrees.");
            return 0f; // No points for the wrong angle
        }

        // Normalize angle (0 degrees = max points, close to maxAllowedAngle = no points)
        float anglePoints = Mathf.Max(0, maxAngleScore - (angle / maxAllowedAngle) * maxAngleScore);
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
