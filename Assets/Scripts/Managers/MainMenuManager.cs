using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject mainMenuUI;
    public GameObject settingsMenuUI;
    public GameObject levelSelectUI;
    public AudioClip startSound;
    public AudioClip quitSound;

    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }

    public void ShowSettingsMenu()
    {
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        levelSelectUI.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }

    public void ShowLevelSelectMenu()
    {
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        levelSelectUI.SetActive(true);
        AudioManager.Instance.PlayClickSound();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        AudioManager.Instance.PlaySound(startSound);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySound(quitSound);
        Task.Delay(800).ContinueWith(_ => Application.Quit());
    }
}
