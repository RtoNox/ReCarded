using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string FullscreenKey = "FullscreenEnabled";

    [Header("UI References")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private bool isInitializing;

    private void Start()
    {
        isInitializing = true;

        float savedVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, 0) == 1;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.SetValueWithoutNotify(savedVolume);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        else
        {
            Debug.LogWarning("SettingsManager: Master Volume Slider is not assigned.");
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
        else
        {
            Debug.LogWarning("SettingsManager: Fullscreen Toggle is not assigned.");
        }

        ApplyMasterVolume(savedVolume, false);
        ApplyFullscreen(savedFullscreen, false);

        isInitializing = false;
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
    }

    public void OnMasterVolumeChanged(float volume)
    {
        if (isInitializing)
            return;

        ApplyMasterVolume(volume, true);
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        if (isInitializing)
            return;

        ApplyFullscreen(isFullscreen, true);
    }

    private void ApplyMasterVolume(float volume, bool save)
    {
        volume = Mathf.Clamp01(volume);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(volume);
        else
            AudioListener.volume = volume;

        if (save)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, volume);
            PlayerPrefs.Save();
        }
    }

    private void ApplyFullscreen(bool isFullscreen, bool save)
    {
        // Checked = borderless fullscreen. Unchecked = maximized windowed mode.
        Screen.fullScreenMode = isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.MaximizedWindow;

        if (save)
        {
            PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
