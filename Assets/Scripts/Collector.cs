using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be added to any object that is capable of collecting items.
/// </summary>
public class Collector : MonoBehaviour
{
    public int TotalItemsCollected = 0; // Total number of collected items

    public void CollectItem(int itemValue)
    {
        TotalItemsCollected += itemValue;
        Debug.Log("Collected Item! Total Items: " + TotalItemsCollected);
    }
}
