using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BNG
{
    public class VRButtonInputHandler : MonoBehaviour
    {
        public EventSystem eventSystem; // Reference to the EventSystem managing UI


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
            Input.GetButtonDown("Cancel");
        }
    }
}
