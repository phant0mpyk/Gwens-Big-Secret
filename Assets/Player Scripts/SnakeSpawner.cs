using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    public GameObject snakePrefab;
    public Transform player;
    
    [Header("Spawn Settings")]
    public int amountOfSnakes = 3;
    public float dropHeight = 10f;
    public float spawnSpread = 5f; // How far apart they drop

    private bool trapSprung = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player walks in, and the trap hasn't been used yet
        if (!trapSprung && collision.CompareTag("Player"))
        {
            trapSprung = true;

            // Spawn multiple snakes in the air AHEAD of the player
            for (int i = 0; i < amountOfSnakes; i++)
            {
                // Calculate a random position in the air, slightly ahead of the donkey
                Vector3 spawnPos = new Vector3(
                    player.position.x + Random.Range(2f, spawnSpread), 
                    player.position.y + dropHeight, 
                    0f
                );

                Instantiate(snakePrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}