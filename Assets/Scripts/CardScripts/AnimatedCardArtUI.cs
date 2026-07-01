using UnityEngine;
using UnityEngine.UI;

public class AnimatedCardArtUI : MonoBehaviour
{
    [Header("UI")]
    public Image cardImage;

    [Header("Animation")]
    public Sprite[] frames;
    public float frameRate = 10f;
    public bool playOnStart = true;

    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private bool isPlaying = false;

    void Awake()
    {
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
        }
    }

    void OnEnable()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    void Update()
    {
        if (!isPlaying)
            return;

        if (frames == null || frames.Length == 0)
            return;

        frameTimer += Time.unscaledDeltaTime;

        float secondsPerFrame = 1f / frameRate;

        if (frameTimer >= secondsPerFrame)
        {
            frameTimer -= secondsPerFrame;
            currentFrameIndex++;

            if (currentFrameIndex >= frames.Length)
            {
                currentFrameIndex = 0;
            }

            UpdateFrame();
        }
    }

    public void SetAnimation(Sprite[] newFrames, float newFrameRate)
    {
        frames = newFrames;
        frameRate = newFrameRate;

        currentFrameIndex = 0;
        frameTimer = 0f;

        UpdateFrame();
        Play();
    }

    public void Play()
    {
        isPlaying = true;
        UpdateFrame();
    }

    public void Stop()
    {
        isPlaying = false;
    }

    void UpdateFrame()
    {
        if (cardImage == null)
            return;

        if (frames == null || frames.Length == 0)
        {
            cardImage.enabled = false;
            return;
        }

        cardImage.enabled = true;
        cardImage.sprite = frames[currentFrameIndex];
    }
}