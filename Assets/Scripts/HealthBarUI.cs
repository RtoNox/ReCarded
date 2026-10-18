using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public Slider ghostSlider;

    [Header("Main Bar Settings")]
    [SerializeField] private float mainShrinkSpeed = 10f;

    [Header("Ghost Bar Settings")]
    [SerializeField] private float ghostShrinkSpeed = 2f;
    [SerializeField] private float ghostDelay = 0.5f;

    private float targetValue = 1f;
    private Coroutine ghostCoroutine;

    [Header("Optional Text")]
    public TMP_Text healthText;

    [Header("Display Settings")]
    public bool hideWhenFull = false;
    public bool alwaysFaceCamera = true;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (!alwaysFaceCamera)
            return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    void Update()
    {
        // Smoothly move the main slider to the target value
        healthSlider.value = Mathf.MoveTowards(healthSlider.value, targetValue, Time.deltaTime * mainShrinkSpeed);
    }

    private IEnumerator ShrinkGhostSlider()
    {
        // Wait for the specified delay before the ghost bar starts moving
        yield return new WaitForSeconds(ghostDelay);

        // Smoothly drain the ghost slider until it catches up to the target value
        while (ghostSlider.value > targetValue)
        {
            ghostSlider.value = Mathf.MoveTowards(ghostSlider.value, targetValue, Time.deltaTime * ghostShrinkSpeed);
            yield return null;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0)
            return;

        // Calculate percentages (0.0 to 1.0) if your sliders use a Max Value of 1.
        // If your sliders use your actual max health as their Max Value, 
        // you can just do: targetValue = currentHealth;
        targetValue = Mathf.Clamp01((float)currentHealth / maxHealth);

        // If taking damage, start the trailing ghost slider effect
        if (targetValue < ghostSlider.value)
        {
            if (ghostCoroutine != null)
            {
                StopCoroutine(ghostCoroutine);
            }
            ghostCoroutine = StartCoroutine(ShrinkGhostSlider());
        }
        else
        {
            // If healing, make the ghost slider instantly match the higher health
            ghostSlider.value = targetValue;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }

        if (hideWhenFull)
        {
            gameObject.SetActive(currentHealth < maxHealth);
        }
    }
}