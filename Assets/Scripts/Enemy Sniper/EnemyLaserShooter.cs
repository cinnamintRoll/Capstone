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
    public AudioClip chargeSound; // Sound to play while charging
    public AudioClip shootSound; // Sound to play when shooting
    public AudioSource audioSource; // AudioSource component reference
    public GameObject visuals;

    public float laserOffsetWidth = 1f; // Width of the offset for the laser
    public float chargeTime = 1f; // Duration of the charge-up effect
    public Color laserColorCharged = Color.red; // Color of the charged laser
    public Color laserColorNormal = Color.green; // Color of the normal laser
    public Color shootBeamColor = Color.white; // Color of the beam when shooting

    // Width settings for the laser
    public float initialLaserWidth = 0.05f; // Initial width of the laser
    public float chargeLaserWidth = 0.2f; // Maximum width during the charge-up phase
    public float shootLaserWidth = 0.4f; // Width of the white beam during shooting
    public float bigBeamDuration = 0.2f; // Duration of the big beam effect

    private bool isShooting = false;
    private Vector3 laserOffset; // Fixed laser offset

    void Start()
    {
        if (!player)
        {
            SetPlayer(GameObject.FindWithTag("PlayerHead"));
        }
        // Get the AudioSource component attached to this GameObject
        laser.startColor = laserColorNormal; // Set the initial color of the laser
        laser.endColor = laserColorNormal;
        laser.startWidth = initialLaserWidth; // Set initial laser width
        laser.endWidth = initialLaserWidth;
        UpdateLaserOffset();
    }

    public void SetPlayer(GameObject Object)
    {
        player = Object.transform;
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

    private void UpdateLaserOffset()
    {
        // Only calculate offset if player is assigned
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float offsetAmount = 0.5f; // Set the offset distance (adjust this value as needed)

            // Determine offset direction based on the player's position
            if (Vector3.Dot(Vector3.right, directionToPlayer) > 0) // Player is on the right side
            {
                laserOffset = Vector3.Cross(directionToPlayer, Vector3.up) * offsetAmount; // Right offset
            }
            else // Player is on the left side
            {
                laserOffset = -Vector3.Cross(directionToPlayer, Vector3.up) * offsetAmount; // Left offset
            }
        }
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
                if (((1 << hit.collider.gameObject.layer) & deflectLayer) != 0)
                {
                    audioSource.Stop();
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

        // Start playing the charging sound on loop
        audioSource.clip = chargeSound;
        audioSource.loop = true;
        audioSource.Play();

        // Charge up effect
        float chargeElapsed = 0f;
        while (chargeElapsed < chargeTime)
        {
            chargeElapsed += Time.deltaTime;
            float t = chargeElapsed / chargeTime;

            // Adjust laser color based on charge
            laser.startColor = Color.Lerp(laserColorNormal, laserColorCharged, t);
            laser.endColor = Color.Lerp(laserColorNormal, laserColorCharged, t);

            // Gradually increase the laser width during the charge-up phase
            float currentWidth = Mathf.Lerp(initialLaserWidth, chargeLaserWidth, t);
            laser.startWidth = currentWidth;
            laser.endWidth = currentWidth;

            yield return null; // Wait for the next frame
        }

        // Stop the charging sound before shooting
        audioSource.PlayOneShot(shootSound);

        // White shooting beam with animated expansion and contraction
        yield return StartCoroutine(AnimateShootBeam());

        // Play shooting sound
       

        // Now shoot the laser (apply damage or any action here)
        ShootPlayer();

        // Reset laser width and color back to normal after shooting
        laser.startWidth = initialLaserWidth;
        laser.endWidth = initialLaserWidth;
        laser.startColor = laserColorNormal;
        laser.endColor = laserColorNormal;

        isShooting = false;
    }

    // Function to animate the shoot beam expanding quickly and reverting back
    IEnumerator AnimateShootBeam()
    {
        float expandTime = 0.1f; // Time to expand the beam
        float contractTime = 0.1f; // Time to contract the beam

        // Expand phase
        float elapsedTime = 0f;
        while (elapsedTime < expandTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / expandTime;

            // Increase laser width to shootLaserWidth
            float currentWidth = Mathf.Lerp(chargeLaserWidth, shootLaserWidth, t);
            laser.startWidth = currentWidth;
            laser.endWidth = currentWidth;

            laser.startColor = shootBeamColor; // Set the beam color to white
            laser.endColor = shootBeamColor;

            yield return null;
        }

        yield return new WaitForSeconds(bigBeamDuration); // Hold the big beam for a short duration

        // Contract phase
        elapsedTime = 0f;
        while (elapsedTime < contractTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / contractTime;

            // Reduce laser width back to chargeLaserWidth
            float currentWidth = Mathf.Lerp(shootLaserWidth, chargeLaserWidth, t);
            laser.startWidth = currentWidth;
            laser.endWidth = currentWidth;

            yield return null;
        }
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
