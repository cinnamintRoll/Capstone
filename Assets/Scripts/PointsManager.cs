using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance { get; private set; }

    [SerializeField] private int points = 0;

    private void Awake()
    {
        // Singleton pattern to ensure only one PointsManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist between scenes
        }
        else
        {
            Destroy(gameObject); // If another instance is created, destroy it
        }
    }

    void OnEnable()
    {
        int savedPoints = PlayerPrefs.GetInt("PlayerPoints");
        if (savedPoints != 0)
        {
            points = savedPoints;
        }
    }

    public void ModifyPoints(int inputPoints)
    {
        points += inputPoints;
        Debug.Log("Current Points: " + points);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("PlayerPoints", points); // Save points on destruction
    }
}
