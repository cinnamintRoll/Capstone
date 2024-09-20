using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] pathPoints;  // Array of waypoints
    public float speed = 5.0f;      // Speed at which to move along the path
    private int currentSegment = 0; // The current segment of the path
    private float t = 0;            // Parameter for interpolation (0 to 1 over each segment)
    public Transform pathParent;  // Parent transform for path points

    void Update()
    {
        if (pathPoints.Length < 2)
        {
            Debug.LogError("You need at least 2 points to follow the path.");
            return;
        }

        // Move to the next segment if we're at the end of the current segment
        if (currentSegment < pathPoints.Length - 1)
        {
            // Calculate the distance to the next point
            float segmentLength = Vector3.Distance(pathPoints[currentSegment].position, pathPoints[currentSegment + 1].position);

            // Calculate the step size based on speed and time
            float step = speed * Time.deltaTime / segmentLength;

            // Update t for the current segment
            t += step;

            // Move along the spline using Catmull-Rom interpolation
            transform.position = CatmullRom(
                GetControlPoint(currentSegment - 1),
                pathPoints[currentSegment].position,
                pathPoints[currentSegment + 1].position,
                GetControlPoint(currentSegment + 2),
                t
            );

            // If we've reached the end of the segment, move to the next segment
            if (t >= 1f)
            {
                t = 0f;
                currentSegment++;
            }
        }
    }

    // This will visualize the path in the Scene view
    private void OnDrawGizmos()
    {
        if (pathPoints != null && pathPoints.Length >= 2)
        {
            Gizmos.color = Color.red;

            // Draw Catmull-Rom splines between the points
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Vector3 previousPosition = pathPoints[i].position;
                for (float j = 0; j < 1; j += 0.05f)  // 0.05f determines the resolution of the drawn line
                {
                    Vector3 point = CatmullRom(
                        GetControlPoint(i - 1),
                        pathPoints[i].position,
                        pathPoints[i + 1].position,
                        GetControlPoint(i + 2),
                        j
                    );
                    Gizmos.DrawLine(previousPosition, point);
                    previousPosition = point;
                }
            }
        }
    }

    // Catmull-Rom interpolation between four points
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }

    // Helper method to get the control point, using boundary points when needed
    private Vector3 GetControlPoint(int index)
    {
        if (index < 0)
        {
            return pathPoints[0].position; // Use the first point
        }
        else if (index >= pathPoints.Length)
        {
            return pathPoints[pathPoints.Length - 1].position; // Use the last point
        }

        return pathPoints[index].position;
    }   
}
