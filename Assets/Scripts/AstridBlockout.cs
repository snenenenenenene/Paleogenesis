using UnityEngine;

public class CityPopulator : MonoBehaviour
{
    [Header("Building Settings")]
    public GameObject[] buildingPrefabs; // Array of building prefabs
    public Transform cityParent; // Parent transform to organize city objects
    public LayerMask buildingLayer; // LayerMask for buildings

    [Header("City Settings")]
    public Vector2 gridDimensions = new Vector2(200, 200); // City grid size (x, z)
    public float gridSpacing = 5f; // Spacing between buildings

    void Start()
    {
        Debug.Log("Populating city...");
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            Debug.LogError("Building prefabs array is empty! Please assign building prefabs.");
            return;
        }

        PopulateCity();
    }

    void PopulateCity()
    {
        // Half dimensions for positioning relative to center
        float halfWidth = gridDimensions.x / 2;
        float halfHeight = gridDimensions.y / 2;

        for (float x = -halfWidth; x <= halfWidth; x += gridSpacing)
        {
            for (float z = -halfHeight; z <= halfHeight; z += gridSpacing)
            {
                Vector3 position = new Vector3(x, 0, z);

                // Try placing the building
                if (!PlaceBuilding(position))
                {
                    Debug.Log($"Skipping position {position} due to overlap.");
                }
            }
        }
    }

   private bool PlaceBuilding(Vector3 position)
{
    // Define the size of the overlap check box (match this to your prefab size)
    Vector3 halfExtents = new Vector3(2.5f, 1f, 2.5f); // Adjust as needed

    // Use a layer mask for buildings
    int buildingLayerMask = LayerMask.GetMask("Building");

    // Check for overlap
    Collider[] colliders = Physics.OverlapBox(position, halfExtents, Quaternion.identity, buildingLayerMask);
    
    if (colliders.Length > 0)
    {
        Debug.Log($"Skipping position {position} due to overlap with {colliders[0].name}");
        return false;
    }

    // Randomly select a prefab
    GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

    // Instantiate the building
    Instantiate(buildingPrefab, position, Quaternion.identity);
    Debug.Log($"Building placed at {position}");

    return true;
}

}
