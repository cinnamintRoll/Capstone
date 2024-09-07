using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, ISelectHandler
{
    public AudioClip hoverSound; // The sound to play when a button is hovered/selected
    private AudioSource audioSource;
    [SerializeField] private EventSystem eventSystem;

    void Start()
    {
        // Get the AudioSource from the object (make sure the script is attached to the same object as the AudioSource)
        audioSource = GetComponent<AudioSource>();
    }

    // This method is triggered when the button is selected (e.g., by keyboard/gamepad/VR controller)
    public void OnSelect(BaseEventData eventData)
    {
        PlaySound();
    }

    // Plays the hover sound
    public void PlaySound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
