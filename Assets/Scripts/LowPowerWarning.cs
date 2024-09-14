using UnityEngine;
using UnityEngine.UI;

public class LowPowerWarning : MonoBehaviour
{
    public Image warningImage;     // The image to fade in and out
    public float fadeSpeed = 2.0f; // Speed of the fade in/out
    private bool isFading = false; // Control if fading is active
    private bool isFadingIn = true; // Control fade direction (true = fading in, false = fading out)

    void Start()
    {
        if (warningImage == null)
        {
            Debug.LogError("Image has not been set. Please assign the Image component.");
            return; // Exit if no image is set to avoid errors
        }
        // Start the fade effect
        TriggerWarning(true);
    }

    void Update()
    {
        if (isFading)
        {
            // Get current color and alpha
            Color color = warningImage.color;
            float targetAlpha = isFadingIn ? 1f : 0f; // Target alpha: 1 for fade in, 0 for fade out

            // Gradually change alpha towards target alpha
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            warningImage.color = color;

            // If we reach the target alpha, switch the fade direction
            if (Mathf.Approximately(color.a, targetAlpha))
            {
                isFadingIn = !isFadingIn; // Switch direction when target alpha is reached
            }
        }
    }

    // Method to start or stop the warning effect
    public void TriggerWarning(bool trigger)
    {
        if (trigger)
        {
            isFading = true;  // Activate the fading effect
        }
        else
        {
            isFading = false; // Stop fading
            // Optionally, reset the image alpha when stopping
            Color color = warningImage.color;
            color.a = 0f;
            warningImage.color = color;
            Debug.Log("Warning image reset to transparent.");
        }
    }
}
