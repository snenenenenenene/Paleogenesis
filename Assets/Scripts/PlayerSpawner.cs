using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;     // Reference to the player prefab
    public Transform spawnPoint;        // The position where the player will spawn

    private GameObject spawnedPlayer;   // Holds reference to the spawned player

    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("PlayerSpawner is missing a playerPrefab or spawnPoint!");
            return;
        }

        if (spawnedPlayer == null) // Check if player has already been spawned
        {
            spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Player spawned at: " + spawnPoint.position);
        }
    }
}
