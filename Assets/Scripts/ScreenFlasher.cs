using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFlasher : MonoBehaviour
{
    public static ScreenFlasher Instance { get; private set; }

    public CanvasGroup canvasGroup;
    public Image screenFlashOverlay;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (screenFlashOverlay == null)
            screenFlashOverlay = GetComponentInChildren<Image>();
    }

    public void FlashScreen(Color color, float duration)
    {
        screenFlashOverlay.color = color;
        FadeOut(0f, duration);
    }

    private void FadeOut(float targetAlpha, float duration)
    {
        // Stop any currently running fade to avoid conflicts
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = 1f;
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            // Smoothly interpolate alpha over time
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed / duration);
            yield return null;
        }

        // Ensure final value snaps perfectly to target
        canvasGroup.alpha = targetAlpha;
    }
}