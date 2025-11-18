using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Settings")]
    public bool showVisual = true; // Mostrar sprite visual del checkpoint
    
    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && !showVisual)
        {
            spriteRenderer.enabled = false;
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isActivated)
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.SetCheckpoint(transform.position);
                isActivated = true;
                
                // Cambiar color visual si hay sprite
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.green; // Indicar que está activado
                }
                
                Debug.Log("Checkpoint guardado en: " + transform.position);
            }
        }
    }
}

