using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button retryButton;
    public Button mainMenuButton;

    void Start()
    {
        // Mostrar puntuación final
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        if (scoreText != null)
            scoreText.text = "Puntuación: " + lastScore;

        // Configurar botones
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        // Permitir volver al menú con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }
}

