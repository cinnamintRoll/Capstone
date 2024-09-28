using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be added to any object that is capable of collecting items.
/// </summary>
public class Collector : MonoBehaviour
{
    public Vector3 CollectorVelocity;
    public int TotalItemsCollected = 0; // Total number of collected items
    private Vector3 PreviousPosition;
    public void CollectItem(int itemValue)
    {
        TotalItemsCollected += itemValue;
        Debug.Log("Collected Item! Total Items: " + TotalItemsCollected);
    }
    private void Start()
    {
        PreviousPosition = transform.position;
    }

    private void Update()
    {
        CollectorVelocity = (transform.position - PreviousPosition) /Time.deltaTime;
        PreviousPosition = transform.position;
    }
}
