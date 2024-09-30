using UnityEngine;
using System.Collections;
using BNG;
using UnityEngine.Events;

public class AutoAimGun : MonoBehaviour
{
    // Public variables for customization
    public float aimRange = 50f;                 // Max distance for enemy detection
    public float aimAssistAngle = 15f;           // The field of view for auto-aim
    public float shootRange = 100f;              // Range of the raycast
    public LayerMask enemyLayer;                 // Layer to detect enemies
    public Transform gunBarrel;                  // The point where the raycast shoots from
    public ParticleSystem muzzleFlash;           // Optional: Particle effect for shooting

    public int InternalAmmo = 30;
    public int MaxInternalAmmo = 30;
    public float RecoilForce = 5f;
    public float RecoilDuration = 0.1f;
    public Rigidbody weaponRigid;
    public Grabber thisGrabber;
    public float GunShotVolume = 1f;
    public AudioClip GunShotSound;
    public UnityEvent onShootEvent;
    public UnityEvent onReloadEvent;

    private Transform closestEnemy;
    InputBridge input;

    private void Awake()
    {
        input = InputBridge.Instance;
    }
    void Update()
    {
        // Check for trigger press using InputBridge
        if (input.RightTriggerDown)
        {
            FindClosestEnemy();
            Fire();
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
        // Raycast from gun barrel
        if (Physics.Raycast(gunBarrel.position, shootDirection, out hit, shootRange))
        {
            Debug.DrawRay(gunBarrel.position, shootDirection * shootRange, Color.red, 1f);

            // If hit an enemy
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit: " + hit.collider.name);
                // You can add damage logic here (like calling enemy's damage handler)
            }
        }

        // Play the muzzle flash if assigned
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Apply recoil and haptics
        ApplyRecoil();

        // Trigger shoot event
        if (onShootEvent != null)
        {
            onShootEvent.Invoke();
        }

        // Decrease ammo
        InternalAmmo--;
    }

    // Recoil and Haptics
    private void ApplyRecoil()
    {
        if (weaponRigid != null)
        {

            // Apply recoil force
            weaponRigid.AddForceAtPosition(gunBarrel.forward * -RecoilForce, gunBarrel.position, ForceMode.Impulse);
        }


            input.VibrateController(0.1f, 0.2f, 0.1f, thisGrabber.HandSide);
    }

    public void Reload()
    {
        InternalAmmo = MaxInternalAmmo;
        // Trigger reload event
        if (onReloadEvent != null)
        {
            onReloadEvent.Invoke();
        }
    }
}
