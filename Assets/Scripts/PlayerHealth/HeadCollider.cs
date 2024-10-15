using UnityEngine;

public class HeadCollider : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object's layer is "Obstacle"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("Head hit an obstacle!");

            // Access the PlayerHealth instance and call TakeDamage
            PlayerHealth.Instance.TakeDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger collider's layer is "Obstacle"
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("Head hit an obstacle (trigger)!");

            Destroy(other.gameObject);
            // Access the PlayerHealth instance and call TakeDamage
            PlayerHealth.Instance.TakeDamage();
        }
    }
}
