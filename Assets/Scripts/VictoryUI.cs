using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;
    public GameObject victoryPanel;
    public AudioClip victorySound;

    void OnEnable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnVictory += ShowVictory;
        }
    }

    void OnDisable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnVictory -= ShowVictory;
        }
    }

    void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    void ShowVictory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        AudioManager.Instance.PlaySound(victorySound);
        Time.timeScale = 0f;
    }
}