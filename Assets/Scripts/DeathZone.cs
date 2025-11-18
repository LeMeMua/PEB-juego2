using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Punto donde reaparecerá el jugador
    public float respawnDelay = 0.5f; // Tiempo antes de respawnear
    
    private Vector3 defaultRespawnPosition;
    private bool isRespawning = false;
    private GameObject playerReference; // Guardar referencia al jugador
    
    void Start()
    {
        Debug.Log("DeathZone: Inicializado");
        // Si no hay respawnPoint asignado, usar la posición inicial del jugador
        if (respawnPoint == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                defaultRespawnPosition = player.transform.position;
                Debug.Log($"DeathZone: Posición inicial guardada: {defaultRespawnPosition}");
            }
            else
            {
                Debug.LogWarning("DeathZone: No se encontró el jugador en Start()");
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"DeathZone: OnTriggerEnter2D detectado. Objeto: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DeathZone: Jugador detectado!");
            
            if (isRespawning)
            {
                Debug.Log("DeathZone: Ya está respawneando, ignorando trigger");
                return;
            }
            
            isRespawning = true;
            Debug.Log("DeathZone: Iniciando proceso de muerte y respawn");
            
            // Guardar referencia al jugador ANTES de desactivarlo
            playerReference = collision.gameObject;
            
            // Quitar una vida
            if (Gamemanager.instance != null)
            {
                Debug.Log("DeathZone: GameManager encontrado, quitando vida");
                // Contar cuántas vidas quedan activas
                int vidasActivas = 0;
                for (int i = 0; i < Gamemanager.instance.vidas.Length; i++)
                {
                    if (Gamemanager.instance.vidas[i] != null && Gamemanager.instance.vidas[i].activeSelf)
                    {
                        vidasActivas++;
                    }
                }
                
                Debug.Log($"DeathZone: Vidas activas: {vidasActivas}");
                
                // Quitar una vida si hay vidas disponibles
                if (vidasActivas > 0)
                {
                    int indiceVida = Gamemanager.instance.vidas.Length - vidasActivas;
                    Gamemanager.instance.desactivar_vida(indiceVida);
                    Debug.Log($"DeathZone: Vida quitada. Índice: {indiceVida}");
                }
                else
                {
                    Debug.LogWarning("DeathZone: No hay vidas disponibles para quitar");
                }
            }
            else
            {
                Debug.LogError("DeathZone: GameManager.instance es null!");
            }
            
            // Respawnear al jugador usando coroutine
            StartCoroutine(RespawnPlayerCoroutine());
        }
        else
        {
            Debug.Log($"DeathZone: Objeto detectado pero no es Player. Tag: {collision.gameObject.tag}");
        }
    }
    
    IEnumerator RespawnPlayerCoroutine()
    {
        if (playerReference == null)
        {
            Debug.LogError("DeathZone: playerReference es null en coroutine!");
            isRespawning = false;
            yield break;
        }
        
        Debug.Log("DeathZone: Iniciando coroutine de respawn");
        
        // Detener movimiento
        Rigidbody2D rb = playerReference.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Desactivar temporalmente el jugador
        Debug.Log("DeathZone: Desactivando jugador");
        playerReference.SetActive(false);
        
        // Esperar antes de respawnear
        Debug.Log($"DeathZone: Esperando {respawnDelay} segundos antes de respawnear");
        yield return new WaitForSeconds(respawnDelay);
        
        // Respawnear
        DoRespawn();
    }
    
    void DoRespawn()
    {
        Debug.Log("DeathZone: DoRespawn() llamado");
        
        if (playerReference == null)
        {
            Debug.LogWarning("DeathZone: playerReference es null, intentando encontrar jugador");
            playerReference = GameObject.FindGameObjectWithTag("Player");
            if (playerReference == null)
            {
                Debug.LogError("DeathZone: No se pudo encontrar el jugador para respawnear!");
                isRespawning = false;
                return;
            }
        }
        
        PlayerMovement playerMovement = playerReference.GetComponent<PlayerMovement>();
        
        // Determinar posición de respawn
        Vector3 respawnPos;
        if (respawnPoint != null)
        {
            respawnPos = respawnPoint.position;
            Debug.Log($"DeathZone: Respawneando en RespawnPoint: {respawnPos}");
        }
        else if (playerMovement != null)
        {
            respawnPos = playerMovement.GetLastSafePosition();
            Debug.Log($"DeathZone: Respawneando en checkpoint: {respawnPos}");
        }
        else
        {
            respawnPos = defaultRespawnPosition;
            Debug.Log($"DeathZone: Respawneando en posición inicial: {respawnPos}");
        }
        
        // Activar jugador
        playerReference.SetActive(true);
        Debug.Log("DeathZone: Jugador reactivado");
        
        // Mover jugador
        playerReference.transform.position = respawnPos;
        Debug.Log($"DeathZone: Jugador movido a: {respawnPos}");
        
        // Resetear velocidad
        Rigidbody2D rb = playerReference.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Resetear estado de salto
        if (playerMovement != null)
        {
            playerMovement.ResetGroundedState();
        }
        
        isRespawning = false;
        Debug.Log("DeathZone: Respawn completado!");
    }
}