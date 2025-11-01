using UnityEngine;
using TMPro;
public class HUD : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void actualizar_puntos(int puntostotales)
    {
        textMeshPro.text = puntostotales.ToString();
    }
}
