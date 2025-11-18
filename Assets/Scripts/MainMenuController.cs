using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Update()
    {
        // Cualquier tecla para empezar
        if (Input.anyKeyDown)
        {
            PlayGame();
        }
        
        // Opcional: ESC para créditos (si quieres mantener esa opción)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowCredits();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("PrologoScene");
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}