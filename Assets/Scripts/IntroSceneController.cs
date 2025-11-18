using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;

    [Header("Scene Boundaries")]
    public Transform endPoint; // Punto donde termina la escena intro
    public float endPointX = 20f; // Si no usas Transform, usa coordenada X

    [Header("Dialogue System")]
    public DialogueSystem dialogueSystem;

    [Header("Transition Settings")]
    public float transitionDelay = 1f; // Tiempo de espera antes de cambiar de escena
    public float fadeDuration = 1f; // Duración del fade de transición

    private bool hasReachedEnd = false;
    private SceneTransition sceneTransition;

    void Start()
    {
        // Buscar el jugador si no está asignado
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Configurar transición de escena
        GameObject transitionObj = new GameObject("SceneTransition");
        sceneTransition = transitionObj.AddComponent<SceneTransition>();
        sceneTransition.transitionDuration = fadeDuration;
        sceneTransition.useFade = true;

        // REMOVIDO: Ya no inicia diálogos automáticamente
        // Los diálogos se activarán mediante DialogueTrigger cuando el jugador pase por zonas
        // if (dialogueSystem != null)
        // {
        //     dialogueSystem.StartDialogue();
        // }
    }

    void Update()
    {
        if (player == null || hasReachedEnd) return;

        // Verificar si llegó al final
        float endX = endPoint != null ? endPoint.position.x : endPointX;
        
        if (player.transform.position.x >= endX)
        {
            ReachedEnd();
        }
    }

    void ReachedEnd()
    {
        if (hasReachedEnd) return; // Evitar múltiples llamadas
        
        hasReachedEnd = true;

        // Esperar un momento antes de la transición
        Invoke("TransitionToGameScene", transitionDelay);
    }

    void TransitionToGameScene()
    {
        if (sceneTransition != null)
        {
            sceneTransition.TransitionToScene("GameScene");
        }
        else
        {
            // Fallback si no hay transición
            SceneManager.LoadScene("GameScene");
        }
    }
}

