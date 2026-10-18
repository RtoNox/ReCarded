using UnityEngine;
using TMPro;

public class BaseHealth : MonoBehaviour
{
    [Header("Base Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public TMP_Text baseHealthText;
    public GameObject gameOverPanel;

    [Header("Sound Effects")]
    public AudioClip damageSound;
    public AudioClip gameOverSound;
    public AudioClip criticalHealthSound;

    private bool gameOver = false;
    private bool isCriticalHealth = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (gameOver)
            return;

        currentHealth -= damage;

        AudioManager.Instance.PlaySound(damageSound);

        if (currentHealth < maxHealth * 0.25f && !isCriticalHealth)
        {
            isCriticalHealth = true;
            AudioManager.Instance.PlaySound(criticalHealthSound);
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (baseHealthText != null)
        {
            baseHealthText.text = currentHealth.ToString();
        }
    }

    void GameOver()
    {
        gameOver = true;

        Debug.Log("Game Over! The base has been destroyed!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        AudioManager.Instance.PlaySound(gameOverSound);

        Time.timeScale = 0f;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}