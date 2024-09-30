using UnityEngine;
using System.Collections;
using BNG;
using UnityEngine.Events;

public class AutoAimGun : MonoBehaviour
{
    // Firing modes
    public enum FiringMode { SemiAuto, FullAuto }
    public FiringMode currentFiringMode = FiringMode.SemiAuto;

    // Public variables for customization
    public float aimRange = 50f;                 // Max distance for enemy detection
    public float aimAssistAngle = 15f;           // The field of view for auto-aim
    public float shootRange = 100f;              // Range of the raycast
    public LayerMask enemyLayer;                 // Layer to detect enemies
    public Transform gunBarrel;                  // The point where the raycast shoots from
    public ParticleSystem muzzleFlash;           // Optional: Particle effect for shooting
    public AudioSource muzzleSound;
    public int InternalAmmo = 30;
    public int MaxInternalAmmo = 30;
    public float RecoilDuration = 0.1f;          // Duration for recoil animation
    public Vector3 recoilAmount = new Vector3(0.05f, 0.05f, -0.1f); // Recoil direction and magnitude (customizable)
    public Rigidbody weaponRigid;
    public Grabber thisGrabber;
    public float GunShotVolume = 1f;
    public AudioClip GunShotSound;
    public UnityEvent onShootEvent;
    public UnityEvent onReloadEvent;
    public float triggerThreshold = 0.5f;
    private Transform closestEnemy;
    private bool canFire = true;  // Semi-auto control
    private InputBridge input;
    private float bulletDamage = 1f;
    public float fullAutoFireRate = 0.2f; // Delay between shots for full-auto
    private float nextFireTime = 0f;

    // Angle thresholds for reloading
    public float reloadAngleThreshold = 60f;   // How far up or down the barrel needs to point to trigger reload
    public float reloadCooldown = 2f;          // Time between reloads
    private float nextReloadTime = 0f;         // Timer for reload cooldown

    private Vector3 originalPosition;
    private Coroutine recoilCoroutine;

    public LineRenderer bulletTrailPrefab; // Assign this in the Inspector
    public float trailDuration = 0.5f;     // Duration before the trail fades


    private void Awake()
    {
        input = InputBridge.Instance;
        originalPosition = transform.localPosition;  // Store the original position of the gun
    }

    void Update()
    {
        // Handle firing based on the selected firing mode
        if (currentFiringMode == FiringMode.SemiAuto)
        {
            HandleSemiAutoFire();
        }
        else if (currentFiringMode == FiringMode.FullAuto)
        {
            HandleFullAutoFire();
        }

        // Check for reloading by pointing gun up or down
        CheckForReload();
    }

    bool IsTriggerPressed()
    {
        return InputBridge.Instance.RightTrigger > triggerThreshold;
    }

    void HandleSemiAutoFire()
    {
        // Semi-auto: fire only once per trigger press
        if (IsTriggerPressed() && canFire)
        {
            FindClosestEnemy();
            Fire();
            canFire = false;
        }
        else if (!IsTriggerPressed())
        {
            canFire = true;  // Reset when trigger is released
        }
    }

    void HandleFullAutoFire()
    {
        // Full-auto: fire repeatedly while the trigger is held down
        if (IsTriggerPressed() && Time.time >= nextFireTime)
        {
            FindClosestEnemy();
            Fire();
            nextFireTime = Time.time + fullAutoFireRate;  // Delay for next shot
        }
    }

    void CheckForReload()
    {
        // Get the angle between the gun barrel and the world's up direction
        float angleUp = Vector3.Angle(gunBarrel.forward, Vector3.up);
        float angleDown = Vector3.Angle(gunBarrel.forward, Vector3.down);

        // Check if the gun is pointed up or down within the reload angle threshold
        if ((angleUp <= reloadAngleThreshold || angleDown <= reloadAngleThreshold) && Time.time >= nextReloadTime)
        {
            Reload();
            nextReloadTime = Time.time + reloadCooldown;  // Set the cooldown for the next reload
        }
    }

    void FindClosestEnemy()
    {
        closestEnemy = null;
        Collider[] enemiesInRange = Physics.OverlapSphere(gunBarrel.position, aimRange, enemyLayer);
        float closestAngle = Mathf.Infinity;

        foreach (var enemy in enemiesInRange)
        {
            Vector3 directionToEnemy = enemy.transform.position - gunBarrel.position;
            float angle = Vector3.Angle(gunBarrel.forward, directionToEnemy);

            // Prioritize the enemy within the aim assist angle and the closest to the gun's forward direction
            if (angle < aimAssistAngle && angle < closestAngle)
            {
                closestAngle = angle;
                closestEnemy = enemy.transform;
            }
        }
    }

    void Fire()
    {
        // Ensure there is ammo
        if (InternalAmmo <= 0) return;

        // Determine the shooting direction with aim assist
        Vector3 shootDirection = (closestEnemy != null)
            ? (closestEnemy.position - gunBarrel.position).normalized
            : gunBarrel.forward;

        RaycastHit hit;
        Vector3 hitPoint = gunBarrel.position + shootDirection * shootRange;

        // Raycast from gun barrel
        if (Physics.Raycast(gunBarrel.position, shootDirection, out hit, shootRange))
        {
            Debug.DrawRay(gunBarrel.position, shootDirection * shootRange, Color.red, 1f);

            // If hit an enemy
            if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
            {
                Debug.Log("Enemy hit: " + hit.collider.name);
                hitPoint = hit.point;
                Damageable damageEnemy = hit.collider.gameObject.GetComponent<Damageable>();
                if (damageEnemy != null) {
                    damageEnemy.DealDamage(bulletDamage);
                }
                // You can add damage logic here (like calling enemy's damage handler)
            }
        }

        // Create the bullet trail before applying recoil
        CreateBulletTrail(gunBarrel.position, hitPoint);

        // Play the muzzle flash if assigned
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (GunShotSound != null && muzzleSound != null)
        {
            muzzleSound.PlayOneShot(GunShotSound, GunShotVolume);
        }

        // Apply recoil and haptics after creating the trail
        ApplyRecoil();

        // Trigger shoot event
        if (onShootEvent != null)
        {
            onShootEvent.Invoke();
        }

        // Decrease ammo
        InternalAmmo--;
    }
    void CreateBulletTrail(Vector3 startPoint, Vector3 endPoint)
    {
        // Instantiate the bullet trail prefab
        LineRenderer bulletTrail = Instantiate(bulletTrailPrefab);

        // Make sure the trail is using world space
        bulletTrail.useWorldSpace = true;

        // Set the start and end points of the trail
        bulletTrail.SetPosition(0, startPoint);  // Ensure this is the exact position of the gun barrel
        bulletTrail.SetPosition(1, endPoint);    // Set the end point where the bullet hit

        // Start a coroutine to fade the trail over time (optional)
        StartCoroutine(FadeBulletTrail(bulletTrail));
    }


    IEnumerator FadeBulletTrail(LineRenderer bulletTrail)
    {
        float duration = 0.5f; // The time it takes for the trail to disappear
        float startWidth = bulletTrail.startWidth;
        float endWidth = bulletTrail.endWidth;

        // Gradually reduce the alpha and width of the trail
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float lerpFactor = t / duration;

            // Fade out the width over time
            bulletTrail.startWidth = Mathf.Lerp(startWidth, 0f, lerpFactor);
            bulletTrail.endWidth = Mathf.Lerp(endWidth, 0f, lerpFactor);

            // Optionally, you can fade out the color here as well (if using transparency)
            yield return null;
        }

        // Destroy the trail after fading
        Destroy(bulletTrail.gameObject);
    }


    // Recoil and Haptics
    private void ApplyRecoil()
    {
        // Cancel any ongoing recoil effect
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }

        // Start the recoil effect
        recoilCoroutine = StartCoroutine(RecoilAnimation());

        // Apply haptics
        input.VibrateController(0.1f, 0.2f, 0.1f, thisGrabber.HandSide);
    }

    // Coroutine to animate the recoil
    private IEnumerator RecoilAnimation()
    {
        // Target recoil position based on recoilAmount (customizable Vector3)
        Vector3 recoilPosition = originalPosition + recoilAmount;

        // Animate recoil (backward and upward movement)
        float recoilTime = 0;
        while (recoilTime < RecoilDuration)
        {
            recoilTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, recoilTime / RecoilDuration);
            yield return null;
        }

        // Animate return to original position
        recoilTime = 0;
        while (recoilTime < RecoilDuration)
        {
            recoilTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, recoilTime / RecoilDuration);
            yield return null;
        }

        // Ensure the final position is set to original
        transform.localPosition = originalPosition;
    }

    public void Reload()
    {
        InternalAmmo = MaxInternalAmmo;
        Debug.Log("Gun reloaded!");

        // Trigger reload event
        if (onReloadEvent != null)
        {
            onReloadEvent.Invoke();
        }
    }
}
