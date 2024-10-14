using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public EnemyAI enemyPrefab; // The enemy prefab to spawn
    public Transform spawnPoint; // The location where the enemy will spawn
    public Transform player; // Reference to the player
    public float spawnDelay = 2f; // Delay between spawns
    private bool canSpawn = true; // Flag to control spawning


    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        if(gameManager)
            player = gameManager.PlayerTransform;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming the player has the tag "Player"
        {
            
            if (canSpawn)
            SpawnEnemy();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = false; // Stop spawning when the player exits the trigger
        }
    }

    void SpawnEnemy()
    {

        if (enemyPrefab != null && player != null)
            {
            enemyPrefab.player = player; // Assign the player transform to the enemy's AI script
            }

        enemyPrefab.transform.position = spawnPoint.position;
        enemyPrefab.gameObject.SetActive(true);
    }
}
