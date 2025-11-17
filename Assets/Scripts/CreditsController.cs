using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        // Permitir volver al menú con ESC o clic
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            GoToMainMenu();
        }
    }
}

