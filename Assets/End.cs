using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class End : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject optionsPanel;
    public GameObject player;




    // --- Panel Logic ---
    public void OpenOptions() => optionsPanel.SetActive(true);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Optional: You can add a sound effect or animation here before loading the menu
            player.SetActive(false);
            OpenOptions(); // Load the main menu scene (make sure your menu is at index 0 in Build Settings)
        }
    }
}