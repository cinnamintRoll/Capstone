using System.Collections.Generic;
using UnityEngine;

public class TargetSwitcher : MonoBehaviour
{
    public List<GameObject> targetObjects;  // List of GameObjects to switch between
    private Dictionary<string, GameObject> targetDictionary; // Dictionary to store GameObjects by name
    private GameObject currentActiveObject;  // Tracks the current active object
    public string initialScreenName;  // Name of the initial screen to show first

    void Start()
    {
        targetDictionary = new Dictionary<string, GameObject>();

        // Populate the dictionary with GameObjects and their names
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                targetDictionary[obj.name] = obj;
                obj.SetActive(false);  // Ensure all are initially inactive
            }
        }

        // Activate the initial screen if it's provided and valid
        if (!string.IsNullOrEmpty(initialScreenName) && targetDictionary.ContainsKey(initialScreenName))
        {
            currentActiveObject = targetDictionary[initialScreenName];
            currentActiveObject.SetActive(true);  // Show the initial screen
        }
        else
        {
            Debug.LogWarning("Initial screen name is invalid or empty. No screen is activated initially.");
        }
    }

    // Method to switch to a specific target by name
    public void SwitchToTarget(string targetName)
    {
        if (!targetDictionary.ContainsKey(targetName))
        {
            Debug.LogWarning("Invalid target name: " + targetName);
            return; // Do nothing if the name doesn't exist
        }

        // Deactivate the current object if there is one
        if (currentActiveObject != null)
        {
            currentActiveObject.SetActive(false);
        }

        // Activate the new object by name
        currentActiveObject = targetDictionary[targetName];
        currentActiveObject.SetActive(true);
    }
}
