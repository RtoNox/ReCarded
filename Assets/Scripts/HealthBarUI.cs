using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;

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

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0)
            return;

        float healthPercent = (float)currentHealth / maxHealth;

        if (healthSlider != null)
        {
            healthSlider.value = healthPercent;
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