using BNG;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Assign your pause menu UI object
    public float distanceFromPlayer = 2f;  // Distance the menu should appear in front of the player

    public AudioClip pauseClip;     // Audio clip for pausing
    public AudioClip resumeClip;    // Audio clip for resuming

    private Transform playerHead;  // Reference to the player's head (e.g., VR camera)
    private bool isPaused = false; // Tracks if the game is paused
    private AudioSource audioSource;
    private float originalFixedDelta;
    [SerializeField] private float heightOffset;

    [Tooltip("If true, will set Time.fixedDeltaTime to the device refresh rate")]
    public bool SetFixedDelta = false;

    private void Start()
    {
        if (SetFixedDelta)
        {
            Time.fixedDeltaTime = (Time.timeScale / UnityEngine.XR.XRDevice.refreshRate);
        }

        // Assuming the main camera represents the player's head
        playerHead = Camera.main.transform;

        // Ensure the pause menu is initially disabled
        pauseMenuUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        originalFixedDelta = Time.fixedDeltaTime;
    }

    void Update()
    {
        // Check if the pause button (custom input) is pressed
        if (InputBridge.Instance.AButtonDown)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Enable and position the pause menu
        pauseMenuUI.SetActive(true);
        MoveMenuToPlayer();

        // Pause the game by setting time scale to 0
        Time.timeScale = 0;
        Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;

        // Play pause audio
        if (pauseClip != null)
        {
            audioSource.clip = pauseClip;
            audioSource.Play();
        }

        isPaused = true;
    }

    public void ResumeGame()
    {
        // Disable the pause menu
        pauseMenuUI.SetActive(false);

        // Resume the game by setting time scale back to normal
        Time.timeScale = 1;
        Time.fixedDeltaTime = originalFixedDelta;

        // Play resume audio
        if (resumeClip != null)
        {
            audioSource.clip = resumeClip;
            audioSource.Play();
        }

        isPaused = false;
    }

    void MoveMenuToPlayer()
    {
        // Move the menu to a position in front of the player's head
        Vector3 targetPosition = playerHead.position + playerHead.forward * distanceFromPlayer;

        // Ignore the player's pitch and roll rotation
        // We only want to follow the player's yaw (rotation around the Y-axis)
        Quaternion headRotation = Quaternion.Euler(0, playerHead.eulerAngles.y, 0);

        pauseMenuUI.transform.position = new Vector3(targetPosition.x, playerHead.position.y + heightOffset, targetPosition.z);
        pauseMenuUI.transform.rotation = headRotation;
    }

    // Method to close the pause menu
    public void CloseMenu()
    {
        ResumeGame();
    }
}
