using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class Buttons : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject optionsPanel;
    public GameObject infoPanel;

    [Header("Audio")]
    public AudioSource menuAudioSource; // Drag your AudioSource here
    public AudioClip startSound;       // Drag your "Mario Start" clip here

    public void PlayGame()
    {
        // Instead of loading immediately, we start the "Play Sequence"
        StartCoroutine(PlaySequence());
    }

    // This handles the sound delay
    IEnumerator PlaySequence()
    {
        if (menuAudioSource != null && startSound != null)
        {
            menuAudioSource.PlayOneShot(startSound);

            // Wait for the length of the sound clip before switching scenes
            yield return new WaitForSeconds(startSound.length);
        }

        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting!");
        Application.Quit();
    }

    // --- Panel Logic ---
    public void OpenOptions() => optionsPanel.SetActive(true);
    public void CloseOptions() => optionsPanel.SetActive(false);
    public void OpenInfo() => infoPanel.SetActive(true);
    public void CloseInfo() => infoPanel.SetActive(false);
}