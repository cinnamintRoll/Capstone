using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // Required for Button class

public class ButtonMenuOnSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField, Tooltip("This button input is only for when you want to hover select change the menu.")]
    private Button targetSelectButton;  // Button that we check for selection

    [SerializeField] private List<GameObject> PanelsTurnOn = new List<GameObject>();
    [SerializeField] private List<GameObject> PanelsTurnOff = new List<GameObject>();

    // Triggered when any UI element is selected
    public void OnSelect(BaseEventData eventData)
    {
        // Check if the selected object is the specified target button
        if (targetSelectButton != null)
        {
            if (EventSystem.current.currentSelectedGameObject == targetSelectButton.gameObject)
            {
                Debug.Log("Target Button selected!");
                TriggerButton();
            }
        }
    }

    // Method to toggle panel visibility
    public void TriggerButton()
    {
        // Turn off panels
        foreach (GameObject panel in PanelsTurnOff)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }

        // Turn on panels
        foreach (GameObject panel in PanelsTurnOn)
        {
            if (panel != null && !panel.activeSelf)
            {
                panel.SetActive(true);
            }
        }
    }
}
