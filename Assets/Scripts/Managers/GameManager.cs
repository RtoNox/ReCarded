using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    public AudioClip pauseSound;
    public AudioClip startSound;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
        else
            Debug.LogWarning("GameManager: Pause Panel is not assigned.");

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        else
            Debug.LogWarning("GameManager: Settings Panel is not assigned.");

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        else
            Debug.LogWarning("GameManager: Pause Menu is not assigned.");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            AudioManager.Instance.PlaySound(pauseSound);
        else
            CloseSettings();

        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        AudioManager.Instance.PlayClickSound();
    }

    public void CloseSettings()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioManager.Instance.PlaySound(startSound);
    }

    public void MainMenu(string mainMenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
        AudioManager.Instance.PlayClickSound();
    }
}
