using System.Collections;
using UnityEngine;

public class BulletEjector : MonoBehaviour
{
    public GameObject bulletCasingPrefab;  // The prefab of the bullet casing
    public Transform ejectPoint;           // The point where the casing will be ejected
    public float despawnTime = 5f;         // Time in seconds before the casing despawns
    public Vector3 ejectionForce = new Vector3(0.5f, 1f, 0.2f);  // Force to apply on the casing when ejected

    // This function can be triggered by an Animation Event
    public void EjectCasing()
    {
        if (bulletCasingPrefab != null && ejectPoint != null)
        {
            // Instantiate the bullet casing at the ejectPoint position and rotation
            GameObject casingInstance = Instantiate(bulletCasingPrefab, ejectPoint.position, ejectPoint.rotation);

            // Apply force to the casing relative to the gun's current rotation
            Rigidbody rb = casingInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Instead of applying force directly in world space, transform it to local space
                Vector3 localEjectionForce = ejectPoint.TransformDirection(ejectionForce);
                rb.AddForce(localEjectionForce, ForceMode.Impulse);
            }

            // Start the despawn coroutine to destroy the casing after a set time
            StartCoroutine(DespawnCasing(casingInstance));
        }
    }


    private IEnumerator DespawnCasing(GameObject casing)
    {
        // Wait for the specified despawn time
        yield return new WaitForSeconds(despawnTime);

        // Destroy the casing
        if (casing != null)
        {
            Destroy(casing);
        }
    }
}
