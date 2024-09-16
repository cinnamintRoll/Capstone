using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace BNG
{

    public class GraphicsSettingsControl : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        public JoystickControl joystickControl;

        // A list of graphics quality levels
        private List<string> qualityLevels;
        private int currentQualityIndex = 0;

        public TMP_Text qualityText;

        public float changeThreshold = 0.5f;
        private float previousXInput = 0;

        // To keep track of whether the button is selected
        private bool isButtonSelected = false;

        void Start()
        {
            // Initialize quality levels
            qualityLevels = new List<string>(QualitySettings.names);
            currentQualityIndex = QualitySettings.GetQualityLevel();
            UpdateQualityText();
        }

        void Update()
        {
            if (isButtonSelected)
            {
                Vector2 leverVector = joystickControl.LeverVector;

                if (Mathf.Abs(leverVector.x - previousXInput) > changeThreshold)
                {
                    if (leverVector.x > 0)
                    {
                        currentQualityIndex = Mathf.Min(currentQualityIndex + 1, qualityLevels.Count - 1);
                        Debug.Log("Quality Setting moved to rigth");
                    }
                    else if (leverVector.x < 0)
                    {
                        currentQualityIndex = Mathf.Max(currentQualityIndex - 1, 0);
                        Debug.Log("Quality Setting moved left");
                    }
                    
                    QualitySettings.SetQualityLevel(currentQualityIndex);
                    UpdateQualityText();

                    previousXInput = leverVector.x;
                }
            }
        }

        void UpdateQualityText()
        {
            if (qualityText != null)
            {
                qualityText.text = $"Graphics Quality: {qualityLevels[currentQualityIndex]}";
            }
        }

        // Event triggered when the button is selected
        public void OnSelect(BaseEventData eventData)
        {
            isButtonSelected = true;
        }

        // Event triggered when the button is deselected
        public void OnDeselect(BaseEventData eventData)
        {
            isButtonSelected = false;
        }
    }
}
