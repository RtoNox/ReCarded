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

    private bool gameOver = false;

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
            baseHealthText.text = "Base HP: " + currentHealth;
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

        Time.timeScale = 0f;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}