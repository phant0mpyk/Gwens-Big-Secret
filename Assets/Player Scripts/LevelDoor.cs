using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    [Header("Level Settings")]
    public int sceneBuildIndex; // Which scene this door loads

    private bool isPlayerHere = false;

    void Update()
    {
        // If the player is in the doorway AND presses the "W" key
        if (isPlayerHere && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Loading Level " + sceneBuildIndex);
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }

    // When the player enters the door's trigger zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerHere = true;
        }
    }

    // When the player walks away from the door
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerHere = false;
        }
    }
}