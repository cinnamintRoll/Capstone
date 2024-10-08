using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    // Singleton instance
    public static Settings Instance { get; private set; }

    // Variables for storing settings
    private float volume = 1.0f;
    private float audioOffset = 0f;
    private int qualityIndex = 0;

    // Public properties for getting the values (getters)
    public float Volume { get { return volume; } }
    public float AudioOffset { get { return audioOffset; } }
    public int QualityIndex { get { return qualityIndex; } }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Destroy the duplicate instance
            return;
        }
        Instance = this;  // Assign the instance
        DontDestroyOnLoad(gameObject);  // Make the singleton persistent across scenes
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        // Load saved settings from PlayerPrefs
        LoadSettings();
    }

    // Load settings from PlayerPrefs and apply them
    private void LoadSettings()
    {
        // Retrieve saved values or set defaults if they don't exist
        volume = PlayerPrefs.GetFloat("Volume", 1.0f);
        audioOffset = PlayerPrefs.GetFloat("AudioOffset", 0.0f);
        qualityIndex = PlayerPrefs.GetInt("Quality", 0);

        // Apply loaded settings
        ApplySettings();
    }

    // Apply the current settings to the game
    private void ApplySettings()
    {
        // Apply volume and quality level
        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(qualityIndex);

        // You may want to apply audioOffset in your sound system depending on how it's used.
    }

    // Set the master volume level
    public void SetVolumeLevel(float inputVolume)
    {
        volume = Mathf.Clamp01(inputVolume);  // Ensure volume is between 0 and 1
        AudioListener.volume = volume;  // Apply volume immediately
    }

    // Set the audio offset
    public void SetAudioOffset(float inputAudioOffset)
    {
        audioOffset = inputAudioOffset;
        // Apply the audio offset in your sound system depending on how it's used.
    }

    // Set the quality level
    public void SetQualityLevel(int index)
    {
        qualityIndex = Mathf.Clamp(index, 0, QualitySettings.names.Length - 1);  // Clamp to valid quality levels
        QualitySettings.SetQualityLevel(qualityIndex);  // Apply quality level immediately
    }

    // Save the current settings to PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("AudioOffset", audioOffset);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        PlayerPrefs.Save();  // Save changes to disk
    }

    // Discard unsaved changes by reloading the previously saved settings
    public void DiscardSettings()
    {
        LoadSettings();  // Reload saved settings and reapply them
    }

    // Reset all settings to default values
    public void ResetSettings()
    {
        // Reset to default values
        volume = 1.0f;
        audioOffset = 0.0f;
        qualityIndex = 0;

        // Apply and save the reset settings
        ApplySettings();
        SaveSettings();
    }
}
