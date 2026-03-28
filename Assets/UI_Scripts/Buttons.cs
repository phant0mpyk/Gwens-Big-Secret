using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class Buttons : MonoBehaviour
{

    [Header("Menu Panels")]
    // These create slots in the Inspector where you can drag your panels
    public GameObject optionsPanel;
    public GameObject infoPanel;
    // Call this to load your first level
    public void PlayGame()
    {
        // Loads the scene at index 1 in your Build Profiles
        SceneManager.LoadScene(0); 
    }

    // Call this to exit the game
    public void QuitGame()
    {
        Debug.Log("Game is quitting!"); // Just to confirm it works in the editor
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true); // Turns the panel ON
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false); // Turns the panel OFF
    }

    // Info Menu Logic
    public void OpenInfo()
    {
        infoPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }
}