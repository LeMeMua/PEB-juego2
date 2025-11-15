using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;
    public Button mainMenuButton;
    private bool isPaused = false;

    void Update()
    {
        // Toggle pausa con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
        
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        TogglePause();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Restaurar tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        // Asegurar que el tiempo se restaure si el objeto se destruye
        Time.timeScale = 1f;
    }
}

