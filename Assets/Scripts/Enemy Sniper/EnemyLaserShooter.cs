using System.Collections;
using UnityEngine;

public class EnemyLaserShooter : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Transform gun; // The enemy's gun
    public Transform gunMuzzle; // Position where the laser starts (muzzle of the gun)
    public LineRenderer laser; // Laser LineRenderer
    public float laserRange = 50f; // Maximum range of the laser
    public float aimDuration = 2f; // Time before shooting
    public LayerMask deflectLayer; // Layer for detecting player's weapon
    public float playerDamage = 10f; // Damage to apply to player
    public AudioClip deflectionSound; // Sound to play on deflection
    private AudioSource audioSource; // AudioSource component reference
    public GameObject visuals;

    public float laserOffsetWidth = 1f; // Width of the offset for the laser
    public float chargeTime = 1f; // Duration of the charge-up effect
    public Color laserColorCharged = Color.red; // Color of the charged laser
    public Color laserColorNormal = Color.green; // Color of the normal laser
    private bool isShooting = false;

    private Vector3 laserOffset; // Fixed laser offset

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        laser.startColor = laserColorNormal; // Set the initial color of the laser
        laser.endColor = laserColorNormal;

        // Calculate fixed laser offset based on player's position relative to the enemy
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 offsetDirection = Vector3.Cross(directionToPlayer, Vector3.up).normalized; // Perpendicular direction for offset
        float offsetAmount = 0.5f; // Set the offset distance (adjust this value as needed)

        // Determine if the player is on the left or right side
        float dotProduct = Vector3.Dot(offsetDirection, directionToPlayer);
        if (dotProduct > 0) // Player is on the right side
        {
            laserOffset = -offsetDirection * offsetAmount; // Positive offset
        }
        else // Player is on the left side
        {
            laserOffset = offsetDirection * offsetAmount; // Negative offset
        }
    }

    void Update()
    {
        AimAtPlayer();
        UpdateLaser();
        if (!isShooting)
        {
            StartCoroutine(ShootAfterDelay());
        }
    }

    // Aim the gun towards the player
    void AimAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        gun.rotation = Quaternion.LookRotation(direction);
    }

    // Update the laser to point towards the player
    void UpdateLaser()
    {
        if (gunMuzzle)
        {
            laser.SetPosition(0, gunMuzzle.position); // Set the start position of the laser to the gun muzzle
            RaycastHit hit;

            // Apply the fixed offset
            Vector3 offsetPosition = gunMuzzle.position + laserOffset;

            // Laser direction towards player
            Vector3 laserDirection = (player.position - offsetPosition).normalized;

            if (Physics.Raycast(gunMuzzle.position, laserDirection, out hit, laserRange))
            {
                Debug.DrawLine(gunMuzzle.position, hit.point, Color.red);
                laser.SetPosition(1, hit.point);
                if (hit.collider != null && (((1 << hit.collider.gameObject.layer) & deflectLayer) != 0))
                {
                    DeflectShot(hit.point);
                }
            }
            else
            {
                // Set laser end point to max range if no hit
                laser.SetPosition(1, gunMuzzle.position + laserDirection * laserRange);
            }
        }
    }

    // Coroutine to shoot after a delay
    IEnumerator ShootAfterDelay()
    {
        isShooting = true;

        // Charge up effect
        float chargeElapsed = 0f;
        while (chargeElapsed < chargeTime)
        {
            chargeElapsed += Time.deltaTime;
            float t = chargeElapsed / chargeTime;

            // Adjust laser color based on charge
            laser.startColor = Color.Lerp(laserColorNormal, laserColorCharged, t);
            laser.endColor = Color.Lerp(laserColorNormal, laserColorCharged, t);

            yield return null; // Wait for the next frame
        }

        // Reset laser color back to normal after charging
        laser.startColor = laserColorNormal;
        laser.endColor = laserColorNormal;

        // Now shoot the laser
        ShootPlayer();

        isShooting = false;
    }

    // Function to apply damage to the player
    void ShootPlayer()
    {
        Debug.Log("Player hit! Apply damage.");
        // You can add the player's health component and apply damage here.
        // Example: playerHealth.TakeDamage(playerDamage);
    }

    // Function to deflect the shot and kill the enemy
    void DeflectShot(Vector3 deflectPoint)
    {
        Debug.Log("Shot deflected! Enemy killed.");
        laser.enabled = false;
        Destroy(visuals);

        // Instantiate a temporary GameObject to play the sound
        GameObject soundObject = new GameObject("DeflectionSound");
        soundObject.transform.position = deflectPoint; // Set position to the deflect point

        // Add an AudioSource component and play the sound
        AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
        tempAudioSource.clip = deflectionSound;
        tempAudioSource.Play();

        // Destroy the temporary GameObject after the sound has played
        Destroy(soundObject, deflectionSound.length);

        // Destroy the enemy object after a short delay for sound to play
        Destroy(gameObject, 0.5f);
    }
}
