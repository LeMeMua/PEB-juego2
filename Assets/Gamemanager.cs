using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance { get; private set; }
    public int Puntos_Totales { get { return puntosTotales; }}
    private int puntosTotales = 100;

    public HUD hud;
    public GameObject[] vidas;
    private int vidasRestantes = 0;

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

    public void Start()
    {
        for (int i = 0; i < vidas.Length; i++)
        {
            vidas[i].gameObject.SetActive(true);
            vidasRestantes++;
        }
    }
    public void setear_puntosfijos(int puntosfijos)
    {
        puntosTotales = puntosfijos;
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

