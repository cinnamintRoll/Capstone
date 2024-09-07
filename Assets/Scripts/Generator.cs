using System.Collections;
using UnityEngine;
using UnityEngine.Events;  // Add this for UnityEvents

public class Generator : MonoBehaviour
{
    public AudioSource startSound;   // Assign the starting sound audio source
    public AudioSource loopSound;    // Assign the looping sound audio source
    public float startSoundDelay = 1.0f; // Time to delay before looping sound starts

    public UnityEvent onGeneratorStart;  // UnityEvent for starting the generator
    public UnityEvent onGeneratorStop;   // UnityEvent for stopping the generator

    private bool isGeneratorRunning = false;

    // Method to start the generator
    public void StartGenerator()
    {
        if (!isGeneratorRunning)
        {
            StartCoroutine(PlayGeneratorAudio());
            onGeneratorStart?.Invoke();  // Trigger the UnityEvent
        }
    }

    // Coroutine to handle the sound sequence
    private IEnumerator PlayGeneratorAudio()
    {
        isGeneratorRunning = true;

        // Play the generator starting sound
        if (startSound != null)
        {
            startSound.Play();
        }

        // Wait for the duration of the start sound before playing the loop
        yield return new WaitForSeconds(startSoundDelay);

        // Play the looping generator sound
        if (loopSound != null)
        {
            loopSound.Play();
            loopSound.loop = true;  // Ensure the sound loops
        }
    }

    // Optionally, stop the generator
    public void StopGenerator()
    {
        if (isGeneratorRunning)
        {
            // Stop both sounds
            if (startSound != null && startSound.isPlaying)
            {
                startSound.Stop();
            }

            if (loopSound != null && loopSound.isPlaying)
            {
                loopSound.Stop();
            }

            onGeneratorStop?.Invoke();  // Trigger the stop UnityEvent

            isGeneratorRunning = false;
        }
    }
}
