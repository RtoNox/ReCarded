using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";

    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;

    public float MasterVolume { get; private set; } = 1f;
    public AudioClip buttonClickSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SetMasterVolume(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = MasterVolume;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null AudioClip.");
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(clip);
    }

    public void PlayClickSound()
    {
        if (buttonClickSound == null)
        {
            Debug.LogWarning("AudioManager: Button click sound has not been assigned.");
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(buttonClickSound);
    }
}
