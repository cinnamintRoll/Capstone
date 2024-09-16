using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventButton : MonoBehaviour
{
    GameObject targetSelectButton;
    UnityEvent EventTriggers;
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
    public void TriggerButton()
    {
        EventTriggers.Invoke();
    }
}
