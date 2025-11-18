using UnityEngine;

public class BoundaryWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public bool isLeftWall = true; // true = pared izquierda, false = pared derecha
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Detener el movimiento horizontal del jugador
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(0, playerRb.linearVelocity.y);
            }
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Mantener detenido mientras esté en contacto
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(0, playerRb.linearVelocity.y);
            }
        }
    }
}

