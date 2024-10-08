using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you're using TextMeshPro for volume text

public class SoundPanel : MonoBehaviour
{
    // References to UI components
    public Slider volumeSlider; // Assign your Slider in the Inspector
    public TMP_Text volumeText; // Assign your TextMeshPro Text in the Inspector

    private void Start()
    {
        // Load saved volume from Settings instance or use default
        float savedVolume = Settings.Instance.Volume; // Assuming you have a getter for volume
        SetVolume(savedVolume);

        // Set slider value to the saved volume
        volumeSlider.value = savedVolume;

        // Add listener to the slider to call SetVolume when value changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Method to set the volume based on the slider's value
    public void SetVolume(float value)
    {
        // Clamp the value between 0 and 1
        value = Mathf.Clamp01(value);

        // Set the volume in the Settings instance
        Settings.Instance.SetVolumeLevel(value);

        // Update the AudioListener volume
        AudioListener.volume = value;

        // Update volume text if you have a TextMeshPro Text component to display it
        if (volumeText != null)
        {
            volumeText.text = $"Volume: {(value * 100).ToString("0")}%"; // Display as a percentage
        }
    }
}
