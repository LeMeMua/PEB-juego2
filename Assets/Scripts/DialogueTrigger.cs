using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueSystem dialogueSystem; // Referencia al sistema de diálogos
    public int dialogueIndex = 0; // Índice del diálogo a mostrar (0 = primero, 1 = segundo, etc.)
    public bool triggerOnce = true; // Si solo se activa una vez
    
    [Header("Trigger Settings")]
    public bool useTrigger = true; // Usar OnTriggerEnter2D
    public bool useCollision = false; // Usar OnCollisionEnter2D
    
    private bool hasTriggered = false;
    
    void Start()
    {
        // Buscar el DialogueSystem si no está asignado
        if (dialogueSystem == null)
        {
            dialogueSystem = FindObjectOfType<DialogueSystem>();
            if (dialogueSystem == null)
            {
                Debug.LogError("DialogueTrigger: No se encontró DialogueSystem!");
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!useTrigger) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!useCollision) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
    
    void TriggerDialogue()
    {
        // Si solo se activa una vez y ya se activó, no hacer nada
        if (triggerOnce && hasTriggered)
        {
            return;
        }
        
        if (dialogueSystem != null)
        {
            dialogueSystem.ShowDialogueByIndex(dialogueIndex);
            hasTriggered = true;
        }
        else
        {
            Debug.LogWarning("DialogueTrigger: DialogueSystem no está asignado!");
        }
    }
    
    // Método público para activar desde otro script si es necesario
    public void ActivateDialogue()
    {
        TriggerDialogue();
    }
}

