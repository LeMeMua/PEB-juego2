using UnityEngine;

public class recargua : MonoBehaviour
{
    int cantidad_agua = 0;
    public void Recharge()
    { 
        cantidad_agua = 100;
        Gamemanager.instance.setear_puntosfijos(0);
        Gamemanager.instance.puntos_totales(cantidad_agua);   

    }
}
