using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PrologoController : MonoBehaviour
{
    [Header("Prologue Images")]
    public Sprite[] prologueImages; // Array de las 5 imágenes del prólogo
    public Image displayImage; // Componente Image que mostrará las imágenes
    
    [Header("Settings")]
    public float imageDisplayTime = 3f; // Tiempo que se muestra cada imagen (si no se presiona tecla)
    public float fadeInDuration = 0.5f; // Duración del fade in
    public float fadeOutDuration = 0.5f; // Duración del fade out
    public bool allowSkip = true; // Permitir saltar con tecla
    
    [Header("Transition Settings")]
    public float transitionDelay = 0.5f; // Tiempo antes de cambiar a IntroScene
    public float fadeDuration = 1f; // Duración del fade de transición
    
    private int currentImageIndex = 0;
    private bool isTransitioning = false;
    private SceneTransition sceneTransition;
    
    void Start()
    {
        // Verificar que hay imágenes asignadas
        if (prologueImages == null || prologueImages.Length == 0)
        {
            Debug.LogError("PrologoController: No hay imágenes asignadas!");
            // Si no hay imágenes, ir directo a IntroScene
            SceneManager.LoadScene("IntroScene");
            return;
        }
        
        // Verificar que hay un Image asignado
        if (displayImage == null)
        {
            Debug.LogError("PrologoController: No hay Image asignado!");
            SceneManager.LoadScene("IntroScene");
            return;
        }
        
        // Configurar transición de escena
        GameObject transitionObj = new GameObject("SceneTransition");
        sceneTransition = transitionObj.AddComponent<SceneTransition>();
        sceneTransition.transitionDuration = fadeDuration;
        sceneTransition.useFade = true;
        
        // Iniciar con la primera imagen
        StartCoroutine(ShowPrologueSequence());
    }
    
    void Update()
    {
        // Permitir avanzar con cualquier tecla
        if (allowSkip && !isTransitioning && Input.anyKeyDown)
        {
            // No procesar teclas durante fade out
            if (currentImageIndex < prologueImages.Length)
            {
                SkipToNext();
            }
        }
    }
    
    IEnumerator ShowPrologueSequence()
    {
        // Mostrar cada imagen secuencialmente
        for (int i = 0; i < prologueImages.Length; i++)
        {
            currentImageIndex = i;
            
            // Asignar la imagen
            displayImage.sprite = prologueImages[i];
            
            // Fade in
            yield return StartCoroutine(FadeIn());
            
            // Esperar tiempo de visualización o hasta que se presione una tecla
            float elapsed = 0f;
            while (elapsed < imageDisplayTime && !isTransitioning)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Si no se está saltando, hacer fade out
            if (!isTransitioning)
            {
                yield return StartCoroutine(FadeOut());
            }
        }
        
        // Todas las imágenes mostradas, transicionar a IntroScene
        yield return new WaitForSeconds(transitionDelay);
        TransitionToIntroScene();
    }
    
    void SkipToNext()
    {
        if (isTransitioning) return;
        
        StopAllCoroutines();
        StartCoroutine(SkipToNextCoroutine());
    }
    
    IEnumerator SkipToNextCoroutine()
    {
        isTransitioning = true;
        
        // Fade out rápido
        yield return StartCoroutine(FadeOut());
        
        currentImageIndex++;
        
        // Si hay más imágenes, mostrar la siguiente
        if (currentImageIndex < prologueImages.Length)
        {
            displayImage.sprite = prologueImages[currentImageIndex];
            yield return StartCoroutine(FadeIn());
            isTransitioning = false;
            
            // Continuar con la secuencia
            StartCoroutine(ContinueSequence());
        }
        else
        {
            // No hay más imágenes, transicionar
            yield return new WaitForSeconds(transitionDelay);
            TransitionToIntroScene();
        }
    }
    
    IEnumerator ContinueSequence()
    {
        // Esperar tiempo de visualización
        float elapsed = 0f;
        while (elapsed < imageDisplayTime && !isTransitioning)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (!isTransitioning)
        {
            yield return StartCoroutine(FadeOut());
            currentImageIndex++;
            
            if (currentImageIndex < prologueImages.Length)
            {
                displayImage.sprite = prologueImages[currentImageIndex];
                yield return StartCoroutine(FadeIn());
                StartCoroutine(ContinueSequence());
            }
            else
            {
                yield return new WaitForSeconds(transitionDelay);
                TransitionToIntroScene();
            }
        }
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = displayImage.color;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            color.a = alpha;
            displayImage.color = color;
            yield return null;
        }
        
        color.a = 1f;
        displayImage.color = color;
    }
    
    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = displayImage.color;
        float startAlpha = color.a;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            color.a = alpha;
            displayImage.color = color;
            yield return null;
        }
        
        color.a = 0f;
        displayImage.color = color;
    }
    
    void TransitionToIntroScene()
    {
        if (sceneTransition != null)
        {
            sceneTransition.TransitionToScene("IntroScene");
        }
        else
        {
            // Fallback si no hay transición
            SceneManager.LoadScene("IntroScene");
        }
    }
}

