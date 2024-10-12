using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 rotationOffset; // Offset for rotation
    private void OnEnable()
    {
        player = Camera.main.transform;
    }
    void Update()
    {
        if (player != null)
        {
            // Make the object face the player
            transform.LookAt(player.position);

            // Apply the rotation offset
            transform.Rotate(rotationOffset); // Adjust the rotation based on the offset
        }
    }
}
