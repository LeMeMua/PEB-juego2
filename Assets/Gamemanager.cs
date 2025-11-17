using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance { get; private set; }
    public int Puntos_Totales { get { return puntosTotales; }}
    private int puntosTotales = 100;

    public HUD hud;
    public GameObject[] vidas;
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
    public void setear_puntosfijos(int puntosfijos)
    {
        puntosTotales = puntosfijos;
    }
    public void puntos_totales (int puntos_a_sumar)
    {
        puntosTotales += puntos_a_sumar;
        hud.actualizar_puntos(puntosTotales);
    }

    public void desactivar_vida(int indice)
    {
        vidas[indice].SetActive(false);
    }
}
