using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public float transitionDuration = 1f;
    public bool useFade = true;

    private CanvasGroup fadeCanvasGroup;
    private GameObject fadeObject;

    void Start()
    {
        if (useFade)
        {
            CreateFadeCanvas();
        }
    }

    void CreateFadeCanvas()
    {
        // Crear Canvas para fade
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // Por encima de todo

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();

        // Crear imagen de fade
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        
        UnityEngine.UI.Image fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = Color.black;

        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        fadeCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        fadeObject = canvasObj;
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade out
        if (useFade && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / transitionDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(transitionDuration);
        }

        // Cargar escena
        SceneManager.LoadScene(sceneName);
    }

    void OnDestroy()
    {
        if (fadeObject != null)
        {
            Destroy(fadeObject);
        }
    }
}

