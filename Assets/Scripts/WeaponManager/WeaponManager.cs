using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum representing the different weapons, including Fist (No Weapon)
public enum WeaponType
{
    Fist,   // No weapon equipped, or using fists
    Pistol,
    Sword
}

public class WeaponManager : MonoBehaviour
{
    // Singleton instance
    public static WeaponManager Instance { get; private set; }

    // Dictionary to map WeaponType to weapon GameObjects
    public Dictionary<WeaponType, GameObject> weaponMap = new Dictionary<WeaponType, GameObject>();

    // Assign the corresponding weapons in the Inspector
    public GameObject fist;
    public GameObject pistol;
    public GameObject sword;

    [SerializeField] private WeaponType currentWeapon;

    private void Awake()
    {
        // Ensure that only one instance of WeaponManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }

        DontDestroyOnLoad(gameObject); // Keep the singleton across scenes
    }

    public GameObject GetWeaponGameObject(WeaponType weaponType)
    {
        if (weaponMap.ContainsKey(weaponType))
        {
            return weaponMap[weaponType];
        }
        return null;
    }

    void Start()
    {
        // Initialize the weapon map
        weaponMap.Add(WeaponType.Fist, fist);  // Fist or No Weapon has a GameObject now
        weaponMap.Add(WeaponType.Pistol, pistol);
        weaponMap.Add(WeaponType.Sword, sword);

        // Disable all weapons except the default one (Fist)
        foreach (var weapon in weaponMap)
        {
            if (weapon.Value != null)
                weapon.Value.SetActive(false);  // Only disable actual GameObjects
        }
        SwapWeapon(currentWeapon);
    }

    // Call this method when the player touches the floating item to swap the weapon
    public void SwapWeapon(WeaponType newWeapon)
    {
        // Disable the current weapon if it's not "Fist"
        if (weaponMap[currentWeapon] != null)
        {
            weaponMap[currentWeapon].SetActive(false);
        }

        // Enable the new weapon if it's not "Fist"
        if (weaponMap[newWeapon] != null)
        {
            weaponMap[newWeapon].SetActive(true);
        }

        // Update current weapon
        currentWeapon = newWeapon;
    }

    // Example method for automatic weapon swap (e.g., after a certain action or event)
    public void AutoSwapNextWeapon()
    {
        // Disable the current weapon if it's not "Fist"
        if (weaponMap[currentWeapon] != null)
        {
            weaponMap[currentWeapon].SetActive(false);
        }

        // Get the next weapon type in the enum (loop back to the first weapon if at the end)
        currentWeapon = (WeaponType)(((int)currentWeapon + 1) % System.Enum.GetValues(typeof(WeaponType)).Length);

        // Enable the new weapon if it's not "Fist"
        if (weaponMap[currentWeapon] != null)
        {
            weaponMap[currentWeapon].SetActive(true);
        }
    }
}
