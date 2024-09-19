using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] pathPoints;  // Array of waypoints
    public float speed = 5.0f;
    private int currentPointIndex = 0;

    void Update()
    {
        if (currentPointIndex < pathPoints.Length)
        {
            // Move the object towards the next waypoint
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, speed * Time.deltaTime);

            // Check if we've reached the current waypoint
            if (Vector3.Distance(transform.position, pathPoints[currentPointIndex].position) < 0.1f)
            {
                currentPointIndex++;
            }
        }
    }

    // This will visualize the path in the Scene view
    private void OnDrawGizmos()
    {
        if (pathPoints != null && pathPoints.Length > 1)
        {
            // Draw a line between each point in the path
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }
    }
}
