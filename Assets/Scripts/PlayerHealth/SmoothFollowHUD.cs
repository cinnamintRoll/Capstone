using UnityEngine;

public class SmoothFollowHUD : MonoBehaviour
{
    public Transform playerCamera;   // Reference to the player's camera (VR headset)
    public float followDistance = 2f;  // Distance in front of the player
    public float followHeight = 0.5f;  // Height offset from the player's view
    public float smoothSpeed = 5f;     // Speed of the smooth movement
    public float rotationSpeed = 5f;   // Speed of smooth rotation

    private Vector3 targetPosition;

    void Update()
    {
        // Target position is in front of the player's camera, at a slight height offset
        targetPosition = playerCamera.position + playerCamera.forward * followDistance;
        targetPosition.y += followHeight;

        // Smoothly move the UI to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Smoothly rotate the UI to face the same direction as the player's camera
        Quaternion targetRotation = Quaternion.LookRotation(playerCamera.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
