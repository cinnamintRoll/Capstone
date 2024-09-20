using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(PathFollower))]
public class PathFollowerEditor : Editor
{
    private PathFollower pathFollower;
    private bool isEditing = false;
    private bool isStraightening = false;

    private void OnEnable()
    {
        pathFollower = (PathFollower)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Add buttons to enable editing and straightening
        if (GUILayout.Button(isEditing ? "Stop Editing" : "Edit Path"))
        {
            isEditing = !isEditing;
        }

        if (GUILayout.Button(isStraightening ? "Stop Straightening" : "Straighten Path"))
        {
            isStraightening = !isStraightening;
        }
    }

    void OnSceneGUI()
    {
        if (pathFollower.pathPoints == null || pathFollower.pathPoints.Length < 2) return;

        // Begin change tracking
        EditorGUI.BeginChangeCheck();

        // Handle point manipulation and path editing
        for (int i = 0; i < pathFollower.pathPoints.Length; i++)
        {
            if (pathFollower.pathPoints[i] != null)
            {
                Vector3 newPosition = Handles.PositionHandle(pathFollower.pathPoints[i].position, Quaternion.identity);
                if (pathFollower.pathPoints[i].position != newPosition)
                {
                    Undo.RecordObject(pathFollower.pathPoints[i], "Move Path Point");
                    pathFollower.pathPoints[i].position = newPosition;
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(pathFollower);
        }

        // Toggle the editing mode
        if (isEditing)
        {
            HandlePathEditing();
        }

        // Handle straightening if the user toggled that mode
        if (isStraightening)
        {
            HandleStraightening();
        }
    }

    private void HandlePathEditing()
    {
        Event e = Event.current;

        // Left-click to add new point
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Undo.RecordObject(pathFollower, "Add Path Point");
                Array.Resize(ref pathFollower.pathPoints, pathFollower.pathPoints.Length + 1);

                // Create a parent GameObject if it doesn't exist
                if (pathFollower.pathParent == null)
                {
                    GameObject parent = new GameObject("PathPoints");
                    parent.transform.parent = pathFollower.transform;
                    pathFollower.pathParent = parent.transform; // Store the parent for future reference
                }

                // Create new point under the parent
                Transform newPoint = new GameObject($"Point {pathFollower.pathPoints.Length}").transform;
                newPoint.position = hit.point;
                newPoint.parent = pathFollower.pathParent; // Set the parent of the new point
                pathFollower.pathPoints[pathFollower.pathPoints.Length - 1] = newPoint;
                e.Use();
            }
        }

        // Right-click to delete points
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                for (int i = 0; i < pathFollower.pathPoints.Length; i++)
                {
                    if (Vector3.Distance(pathFollower.pathPoints[i].position, hit.point) < 1f)
                    {
                        Undo.RecordObject(pathFollower, "Delete Path Point");
                        var point = pathFollower.pathPoints[i];
                        DestroyImmediate(point.gameObject);
                        pathFollower.pathPoints = RemovePoint(pathFollower.pathPoints, i);
                        e.Use();
                        break;
                    }
                }
            }
        }
    }

    private void HandleStraightening()
    {
        Handles.color = Color.yellow;

        for (int i = 0; i < pathFollower.pathPoints.Length - 1; i++)
        {
            // Draw a line between points
            Handles.DrawLine(pathFollower.pathPoints[i].position, pathFollower.pathPoints[i + 1].position);

            // Add a button to straighten the segment
            if (Handles.Button((pathFollower.pathPoints[i].position + pathFollower.pathPoints[i + 1].position) / 2, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
            {
                Undo.RecordObject(pathFollower, "Straighten Path");
                Vector3 start = pathFollower.pathPoints[i].position;
                Vector3 end = pathFollower.pathPoints[i + 1].position;

                // Straighten by averaging the positions of the points in this segment
                pathFollower.pathPoints[i + 1].position = new Vector3((start.x + end.x) / 2, start.y, start.z); // Straight line along x
                // Or use different logic based on your needs
            }
        }
    }

    // Remove a point from the array
    private Transform[] RemovePoint(Transform[] points, int index)
    {
        var newPoints = new Transform[points.Length - 1];
        for (int i = 0, j = 0; i < points.Length; i++)
        {
            if (i != index)
            {
                newPoints[j++] = points[i];
            }
        }
        return newPoints;
    }
}
