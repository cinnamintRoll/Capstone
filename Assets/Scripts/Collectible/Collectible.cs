using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    [Tooltip("This is the value of the item when collected.")]
    public int CoinValue = 1;

    [Tooltip("Set to true if you want to destroy this object when it is collected.")]
    public bool DestroyOnCollect = true;

    [Tooltip("If true, the item will be reactivated after RespawnTime.")]
    public bool Respawn = false;

    [Tooltip("If Respawn is true, this GameObject will reactivate after RespawnTime. In seconds.")]
    public float RespawnTime = 10f;

    [Header("Events")]
    [Tooltip("Optional event to be called when the item is collected.")]
    public UnityEvent onCollected;

    private bool isCollected = false; // Track if the item has been collected

    private void Start()
    {
        // Set the collider to trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only process if not already collected
        if (isCollected) return;

        Collector collector = other.GetComponent<Collector>();
        if (collector != null)
        {
            Collect(collector); // Call the Collect method if it's a valid collector
        }
    }

    public void Collect(Collector collector)
    {
        if (isCollected) return; // Prevent double collection

        isCollected = true; // Mark as collected

        // Invoke the collected event
        onCollected?.Invoke();
        Debug.Log("Collected Item!");

        // Award points to the collector
        collector.CollectItem(CoinValue);

        // Destroy or respawn logic
        if (DestroyOnCollect)
        {
            Destroy(gameObject);
        }
        else if (Respawn)
        {
            StartCoroutine(RespawnCoin(RespawnTime));
        }
    }

    private IEnumerator RespawnCoin(float seconds)
    {
        // Hide the coin for a while
        gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);

        // Reset state and reactivate
        isCollected = false;
        gameObject.SetActive(true);
    }
}
