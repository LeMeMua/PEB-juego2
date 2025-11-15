using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance { get; private set; }
    public int Puntos_Totales { get { return puntosTotales; }}
    private int puntosTotales;

    public HUD hud;
    public GameObject[] vidas;
    private int vidasRestantes = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Más de un manager");
        }
    }

    private void Start()
    {
        // Reiniciar vidas al iniciar
        vidasRestantes = vidas.Length;
        for (int i = 0; i < vidas.Length; i++)
        {
            if (vidas[i] != null)
                vidas[i].SetActive(true);
        }
        puntosTotales = 0;
    }

    public void puntos_totales (int puntos_a_sumar)
    {
        puntosTotales += puntos_a_sumar;
        if (hud != null)
            hud.actualizar_puntos(puntosTotales);
    }

    public void desactivar_vida(int indice)
    {
        if (indice >= 0 && indice < vidas.Length && vidas[indice] != null)
        {
            vidas[indice].SetActive(false);
            vidasRestantes--;
            
            if (vidasRestantes <= 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        // Guardar la puntuación final
        PlayerPrefs.SetInt("LastScore", puntosTotales);
        PlayerPrefs.Save();
        
        // Cargar escena de game over
        SceneManager.LoadScene("GameOverScene");
    }
}

