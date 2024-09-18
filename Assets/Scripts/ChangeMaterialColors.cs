using UnityEngine;

public class ChangeMaterialColors : MonoBehaviour
{
    void Start()
    {
        // Iterate through each child of the GameObject this script is attached to
        foreach (Transform child in transform)
        {
            // Iterate through each child of the child (grandchildren)
            foreach (Transform grandchild in child)
            {
                Renderer renderer = grandchild.GetComponent<Renderer>();

                // Check if the grandchild has a Renderer component
                if (renderer != null)
                {
                    Material[] materials = renderer.materials;

                    // Generate random colors
                    Color randomColorForSecondMaterial = Random.ColorHSV();
                    Color randomColorForThirdMaterial = Random.ColorHSV();

                    // Change the color of the 2nd material if it exists
                    if (materials.Length > 1)
                    {
                        materials[1].color = randomColorForSecondMaterial;
                    }

                    // Change the color of the 3rd material if it exists
                    if (materials.Length > 2)
                    {
                        materials[2].color = randomColorForThirdMaterial;
                    }

                    // Apply the modified materials back to the Renderer
                    renderer.materials = materials;
                }
            }
        }
    }
}
