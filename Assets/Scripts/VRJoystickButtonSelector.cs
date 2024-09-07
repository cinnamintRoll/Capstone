using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BNG
{
    public class VRJoystickButtonSelector : MonoBehaviour
    {
        public JoystickControl joystick; // Reference to the custom JoystickControl script
        public EventSystem eventSystem; // Event system that manages UI interactions
        public float inputDelay = 0.2f; // Delay to avoid rapid navigation
        private float lastInputTime;
        [SerializeField] private UnityEngine.UI.Button StartingButton;

        private Vector2 lastJoystickVector; // Track the previous joystick input
        public bool singleSelectMode = true; // Toggle for single select mode

        // UnityEvent triggered when the selection moves
        [System.Serializable]
        public class SelectionMoveEvent : UnityEvent<GameObject, MoveDirection> { }

        // Event to be assigned in the inspector or code
        public SelectionMoveEvent OnSelectionMoved;

        private void OnEnable()
        {
            DestinationButton(StartingButton);
            lastJoystickVector = Vector2.zero; // Initialize with no input
        }

        private void Update()
        {
            // Only move if enough time has passed since the last input
            if (Time.time - lastInputTime < inputDelay)
                return;

            // Get the current UI selection
            GameObject currentSelected = eventSystem.currentSelectedGameObject;

            if (currentSelected != null)
            {
                // Use the LeverVector from JoystickControl
                Vector2 joystickVector = joystick.LeverVector;

                if (singleSelectMode)
                {
                    // Trigger selection only when crossing the threshold, not continuously
                    if (ShouldSelect(joystickVector, lastJoystickVector))
                    {
                        HandleSelection(currentSelected, joystickVector);
                        lastInputTime = Time.time;
                    }
                }
                else
                {
                    // Navigate continuously
                    HandleSelection(currentSelected, joystickVector);
                    lastInputTime = Time.time;
                }

                // Update the lastJoystickVector for the next comparison
                lastJoystickVector = joystickVector;
            }
        }

        // Check if joystick has crossed the movement threshold
        private bool ShouldSelect(Vector2 currentVector, Vector2 lastVector)
        {
            // If the joystick has crossed the threshold in any direction
            return (currentVector.y > 0.5f && lastVector.y <= 0.5f) ||
                   (currentVector.y < -0.5f && lastVector.y >= -0.5f) ||
                   (currentVector.x > 0.5f && lastVector.x <= 0.5f) ||
                   (currentVector.x < -0.5f && lastVector.x >= -0.5f);
        }

        // Handles the actual selection based on the joystick vector
        private void HandleSelection(GameObject currentSelected, Vector2 joystickVector)
        {
            if (joystickVector.y > 0.5f)
            {
                // Move Up
                SelectButton(currentSelected, MoveDirection.Up);
            }
            else if (joystickVector.y < -0.5f)
            {
                // Move Down
                SelectButton(currentSelected, MoveDirection.Down);
            }

            if (joystickVector.x > 0.5f)
            {
                // Move Right
                SelectButton(currentSelected, MoveDirection.Right);
            }
            else if (joystickVector.x < -0.5f)
            {
                // Move Left
                SelectButton(currentSelected, MoveDirection.Left);
            }
        }

        // Handles moving the selection based on the direction
        private void SelectButton(GameObject currentButton, MoveDirection direction)
        {
            // Get the Selectable component of the currently selected button
            Selectable selectable = currentButton.GetComponent<Selectable>();

            // Find the next selectable UI element in the specified direction
            if (selectable != null)
            {
                Selectable nextSelectable = null;

                switch (direction)
                {
                    case MoveDirection.Up:
                        nextSelectable = selectable.FindSelectableOnUp();
                        break;
                    case MoveDirection.Down:
                        nextSelectable = selectable.FindSelectableOnDown();
                        break;
                    case MoveDirection.Left:
                        nextSelectable = selectable.FindSelectableOnLeft();
                        break;
                    case MoveDirection.Right:
                        nextSelectable = selectable.FindSelectableOnRight();
                        break;
                }

                // If there is a next selectable UI element, select it
                if (nextSelectable != null)
                {
                    eventSystem.SetSelectedGameObject(nextSelectable.gameObject);
                    // Invoke the event, passing the new selected GameObject and direction
                    OnSelectionMoved?.Invoke(nextSelectable.gameObject, direction);
                }
            }
        }

        public void DestinationButton(UnityEngine.UI.Button destinationButton)
        {
            destinationButton.Select();
        }
    }

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
