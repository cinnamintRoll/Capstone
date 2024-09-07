using UnityEditor;
using UnityEngine;

public class BarlightController : MonoBehaviour
{
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;
    [SerializeField] private Light LightSource;
    [SerializeField] private Renderer LightRenderer;

    [SerializeField] private bool enableFlicker = false; // Checkbox to enable/disable flicker
    [SerializeField] private float minFlickerInterval = 0.05f; // Minimum interval for flicker
    [SerializeField] private float maxFlickerInterval = 0.3f;  // Maximum interval for flicker

    [SerializeField] private AudioSource audioSource; // Audio source component
    [SerializeField] private AudioClip flickerSound;  // Sound for flickering
    [SerializeField] private AudioClip electricSound; // Sound for when the light turns off

    private float nextFlickerTime;
    [SerializeField] private bool lightIsOn = true; // Keeps track of the intended state of the light

    void Start()
    {
        UpdateLightState(lightIsOn);
        nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
    }

    void Update()
    {
        if (lightIsOn && enableFlicker) // Only flicker if the light should be on and flicker is enabled
        {
            FlickerLight();
        }
    }

    public void SetLight(bool light)
    {
        lightIsOn = light; // Set the intended state of the light

        if (!enableFlicker) // If flicker is disabled, set light to the intended state immediately
        {
            UpdateLightState(lightIsOn);
        }

        // Reset flicker timer if the light is turned on
        if (lightIsOn)
        {
            nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
        }
        else
        {
            audioSource.Stop();
            PlayElectricSound(); // Play electric sound when the light is turned off
        }
    }

    private void FlickerLight()
    {
        if (Time.time >= nextFlickerTime)
        {
            // Toggle between on and off to create a flicker effect
            bool flickerState = !LightSource.enabled;
            UpdateLightState(flickerState);

            if (flickerState)
            {
                PlayFlickerSound(); // Play flicker sound when light is on
            }
            else
            {
                audioSource.Stop();
                PlayElectricSound(); // Play electric sound when light flickers off
            }

            // Schedule the next flicker with a random interval between the min and max values
            nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
        }
    }

    private void UpdateLightState(bool state)
    {
        LightSource.enabled = state;
        Material[] materials = LightRenderer.materials;
        materials[1] = state ? OnMaterial : OffMaterial;
        LightRenderer.materials = materials;
    }

    private void PlayFlickerSound()
    {
        if (flickerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(flickerSound);
        }
    }

    private void PlayElectricSound()
    {
        if (electricSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(electricSound);
        }
    }
}
