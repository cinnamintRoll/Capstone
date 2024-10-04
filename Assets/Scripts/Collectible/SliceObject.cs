using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.Assertions.Must;  // Make sure to include this for slicing functionality

public class SliceObject : MonoBehaviour
{
    // Reference to the start and end points of the blade (assign in the inspector or dynamically)
    public Transform bladeStart;
    public Transform bladeEnd;
    public Collector collector;
    // Reference to a material used for the cross-section (cut faces)
    public Material crossSectionMaterial;
    public float cutForce = 2000f; // Adjust this value as needed for the force of the cut
    // Layer mask to filter sliceable objects
    public LayerMask sliceableLayer;

    // Thickness to extend the ray slightly to ensure collision is detected
    public float rayExtension = 0.01f;

    void Update()
    {
        float distance = Vector3.Distance(bladeStart.position, bladeEnd.position) + rayExtension;
        Debug.DrawLine(bladeStart.position, bladeEnd.position, Color.red);
        if (Physics.Raycast(bladeStart.position, bladeEnd.position, out RaycastHit hit, distance, sliceableLayer))
        {
            GameObject sliceableObject = hit.collider.gameObject;

            // Perform the slice at the point of impact
            Vector3 slicePosition = hit.point;
            Vector3 sliceNormal = bladeEnd.position - bladeStart.position;  // Slice plane along the blade direction

            // Slice the object
            Slice(sliceableObject, slicePosition, sliceNormal);
        }
    }

    public void Slice(GameObject sliceableObject, Vector3 slicePosition, Vector3 sliceNormal)
    {
        // Make sure the object has a MeshFilter and MeshRenderer
        MeshFilter meshFilter = sliceableObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = sliceableObject.GetComponent<MeshRenderer>();
        Vector3 planeNormal = Vector3.Cross(slicePosition, collector.CollectorVelocity);
        planeNormal.Normalize();

        if (meshFilter != null && meshRenderer != null)
        {
            // Use EzySlice to perform the cut
            SlicedHull slicedObject = sliceableObject.Slice(slicePosition, planeNormal, crossSectionMaterial);

            if (slicedObject != null)
            {
                // Create the top sliced part
                GameObject upperHull = slicedObject.CreateUpperHull(sliceableObject, crossSectionMaterial);
                GameObject lowerHull = slicedObject.CreateLowerHull(sliceableObject, crossSectionMaterial);
                upperHull.AddComponent<DespawnAfterSlice>();
                lowerHull.AddComponent<DespawnAfterSlice>();
                // Set the transform for the sliced parts
                SetupHullObject(upperHull, sliceableObject);
                SetupHullObject(lowerHull, sliceableObject);

                // Optionally, destroy the original object
                Destroy(sliceableObject);
            }
        }
    }

    // Helper function to configure the newly created slice objects
    private void SetupHullObject(GameObject hull, GameObject originalObject)
    {
        // Set parent and position to match original object
        hull.transform.SetPositionAndRotation(originalObject.transform.position, originalObject.transform.rotation);
        hull.transform.localScale = originalObject.transform.localScale;
        // Add necessary components to the sliced object
        hull.AddComponent<MeshCollider>().convex = true;  // Add collider
        Rigidbody rb = hull.AddComponent<Rigidbody>();                   // Add rigidbody for physics
        hull.layer = 12;
        // Apply explosion force to the rigidbody
        rb.AddExplosionForce(cutForce, originalObject.transform.position, 1f);
    }
}
