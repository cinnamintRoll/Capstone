using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BNG
{
    public class VRButtonInputHandler : MonoBehaviour
    {
        public EventSystem eventSystem; // Reference to the EventSystem managing UI
        public UnityEngine.UI.Button defaultBackButton; // A button to go back


        public void SelectButtonPress()
        {
            // Handle the "Select" button press
            GameObject currentSelected = eventSystem.currentSelectedGameObject;

            if (currentSelected != null)
            {
                // Check if the selected object is a button and trigger its click event
                UnityEngine.UI.Button button = currentSelected.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.Invoke(); // Simulate button click
                }
            }
        }

        public void HandleBackButtonPress()
        {
            // Handle the "Back" button press

            // You can implement custom back logic here. For example, go to a previous menu or deselect current UI
            if (defaultBackButton != null)
            {
                defaultBackButton.onClick.Invoke(); // Invoke the back button's functionality
            }
            else
            {
                Debug.Log("No back button assigned.");
            }
        }
    }
}
