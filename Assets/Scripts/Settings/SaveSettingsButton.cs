using UnityEngine;

public class SaveSettingsButton : MonoBehaviour
{
    // This method can be assigned to the button in the Unity Editor
    public void SaveSettings()
    {
        // Call the SaveSettings method on the Settings singleton instance
        if (Settings.Instance != null)
        {
            Settings.Instance.SaveSettings();
            Debug.Log("Settings saved successfully.");
        }
        else
        {
            Debug.LogWarning("Settings instance not found. Make sure the Settings script is initialized.");
        }
    }
}
