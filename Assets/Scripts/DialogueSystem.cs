using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 5)]
    public string text;
    public float displayTime = 3f; // Tiempo que se muestra cada diálogo
}

public class DialogueSystem : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image dialogueBackground;

    [Header("Dialogue Content")]
    public List<Dialogue> dialogues = new List<Dialogue>();

    [Header("Settings")]
    public float fadeInSpeed = 0.5f;
    public float fadeOutSpeed = 0.5f;
    public bool skipFade = false; // Para debugging: mostrar texto inmediatamente

    private IntroSceneController introController;
    private int currentDialogueIndex = 0;
    private bool isShowingDialogue = false;
    private Coroutine currentDialogueCoroutine;
    private bool autoContinue = false; // Si debe continuar automáticamente al siguiente diálogo

    void Start()
    {
        // Buscar el controlador de la escena intro
        introController = FindObjectOfType<IntroSceneController>();

        // Ocultar panel inicialmente
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Configurar transparencia inicial
        if (dialogueBackground != null)
        {
            Color bgColor = dialogueBackground.color;
            bgColor.a = 0f;
            dialogueBackground.color = bgColor;
        }

        if (dialogueText != null)
        {
            Color textColor = dialogueText.color;
            textColor.a = 0f;
            dialogueText.color = textColor;
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Count == 0)
        {
            Debug.LogWarning("DialogueSystem: No hay diálogos configurados. Agrega diálogos en el Inspector.");
            return;
        }

        // Verificar que los componentes UI estén asignados
        if (dialoguePanel == null)
        {
            Debug.LogError("DialogueSystem: Dialogue Panel no está asignado!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("DialogueSystem: Dialogue Text no está asignado!");
            return;
        }

        currentDialogueIndex = 0;
        isShowingDialogue = true;
        autoContinue = true; // SÍ continuar automáticamente cuando se inicia desde StartDialogue

        Debug.Log($"DialogueSystem: Iniciando diálogos. Total: {dialogues.Count}");
        ShowDialogue(0);
    }

    // Método para mostrar un diálogo específico por índice (llamado desde DialogueTrigger)
    public void ShowDialogueByIndex(int index)
    {
        if (index < 0 || index >= dialogues.Count)
        {
            Debug.LogWarning($"DialogueSystem: Índice {index} fuera de rango. Total diálogos: {dialogues.Count}");
            return;
        }
        
        // Verificar que los componentes UI estén asignados
        if (dialoguePanel == null)
        {
            Debug.LogError("DialogueSystem: Dialogue Panel no está asignado!");
            return;
        }
        
        if (dialogueText == null)
        {
            Debug.LogError("DialogueSystem: Dialogue Text no está asignado!");
            return;
        }
        
        currentDialogueIndex = index;
        isShowingDialogue = true;
        autoContinue = false; // NO continuar automáticamente cuando se activa desde trigger
        
        Debug.Log($"DialogueSystem: Mostrando diálogo {index + 1}/{dialogues.Count} desde trigger");
        ShowDialogue(index);
    }

    void ShowDialogue(int index)
    {
        if (index >= dialogues.Count)
        {
            EndDialogue();
            return;
        }

        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }

        currentDialogueCoroutine = StartCoroutine(DisplayDialogueCoroutine(dialogues[index]));
    }

    IEnumerator DisplayDialogueCoroutine(Dialogue dialogue)
    {
        // Mostrar texto primero (aunque esté invisible)
        if (dialogueText != null)
        {
            dialogueText.text = dialogue.text;
            // Asegurar que el texto esté visible después del fade
            Color textColor = dialogueText.color;
            textColor.a = 0f; // Iniciar invisible
            dialogueText.color = textColor;
            Debug.Log($"DialogueSystem: Texto asignado. Alpha inicial: {textColor.a}");
        }
        else
        {
            Debug.LogError("DialogueSystem: dialogueText es null!");
        }

        // Mostrar panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log($"DialogueSystem: Panel activado. Activo: {dialoguePanel.activeSelf}, Visible: {dialoguePanel.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("DialogueSystem: dialoguePanel es null!");
        }

        // Verificar Canvas
        if (dialoguePanel != null)
        {
            Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"DialogueSystem: Canvas encontrado. Activo: {canvas.gameObject.activeSelf}, RenderMode: {canvas.renderMode}");
            }
            else
            {
                Debug.LogWarning("DialogueSystem: No se encontró Canvas padre del DialoguePanel!");
            }
        }

        Debug.Log($"DialogueSystem: Mostrando diálogo {currentDialogueIndex + 1}/{dialogues.Count}: {dialogue.text}");

        // Fade in o mostrar inmediatamente
        if (skipFade)
        {
            // Mostrar inmediatamente para debugging
            if (dialogueText != null)
            {
                Color textColor = dialogueText.color;
                textColor.a = 1f;
                dialogueText.color = textColor;
            }
            if (dialogueBackground != null)
            {
                Color bgColor = dialogueBackground.color;
                bgColor.a = 0.8f;
                dialogueBackground.color = bgColor;
            }
            Debug.Log("DialogueSystem: Modo skipFade activado - texto visible inmediatamente");
        }
        else
        {
            yield return StartCoroutine(FadeIn());
        }
        
        Debug.Log($"DialogueSystem: Fade in completado. Alpha texto: {dialogueText?.color.a}, Alpha fondo: {dialogueBackground?.color.a}");

        // Esperar tiempo de visualización
        yield return new WaitForSeconds(dialogue.displayTime);

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Ocultar panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Solo continuar automáticamente si autoContinue está activado
        if (autoContinue)
        {
            // Siguiente diálogo
            currentDialogueIndex++;
            if (currentDialogueIndex < dialogues.Count)
            {
                ShowDialogue(currentDialogueIndex);
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            // No continuar automáticamente, solo terminar este diálogo
            EndDialogue();
        }
    }

    IEnumerator FadeIn()
    {
        float alpha = 0f;
        float elapsed = 0f;
        
        while (alpha < 1f)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Clamp01(elapsed / fadeInSpeed);
            
            if (dialogueBackground != null)
            {
                Color bgColor = dialogueBackground.color;
                bgColor.a = alpha * 0.8f; // Fondo semi-transparente
                dialogueBackground.color = bgColor;
            }
            else
            {
                Debug.LogWarning("DialogueSystem: dialogueBackground es null durante FadeIn!");
            }

            if (dialogueText != null)
            {
                Color textColor = dialogueText.color;
                textColor.a = alpha;
                dialogueText.color = textColor;
            }
            else
            {
                Debug.LogWarning("DialogueSystem: dialogueText es null durante FadeIn!");
            }

            yield return null;
        }
        
        // Asegurar que al final esté completamente visible
        if (dialogueText != null)
        {
            Color finalTextColor = dialogueText.color;
            finalTextColor.a = 1f;
            dialogueText.color = finalTextColor;
        }
        
        if (dialogueBackground != null)
        {
            Color finalBgColor = dialogueBackground.color;
            finalBgColor.a = 0.8f;
            dialogueBackground.color = finalBgColor;
        }
    }

    IEnumerator FadeOut()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeOutSpeed;
            
            if (dialogueBackground != null)
            {
                Color bgColor = dialogueBackground.color;
                bgColor.a = alpha * 0.8f;
                dialogueBackground.color = bgColor;
            }

            if (dialogueText != null)
            {
                Color textColor = dialogueText.color;
                textColor.a = alpha;
                dialogueText.color = textColor;
            }

            yield return null;
        }
    }

    void EndDialogue()
    {
        isShowingDialogue = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    // Método para avanzar diálogo manualmente con click/tecla
    public void NextDialogue()
    {
        if (isShowingDialogue && currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            currentDialogueIndex++;
            if (currentDialogueIndex < dialogues.Count)
            {
                ShowDialogue(currentDialogueIndex);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    void Update()
    {
        // Permitir avanzar diálogo con click o espacio
        if (isShowingDialogue && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            NextDialogue();
        }
    }
}

