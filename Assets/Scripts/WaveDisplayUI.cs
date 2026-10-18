using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveDisplayUI : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;

    [Header("Wave Display")]
    public TMP_Text waveDisplayText;

    [Header("Wave Call UI")]
    public GameObject waveCallPanel;
    public Button waveCallButton;
    public TMP_Text waveTimerText;

    private bool isPlacingTower = false;

    void OnEnable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStateChanged += UpdateUI;
        }
    }

    void OnDisable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStateChanged -= UpdateUI;
        }
    }

    void Start()
    {
        if (waveCallButton != null)
        {
            waveCallButton.onClick.AddListener(CallWave);
        }

        UpdateUI();
    }

    void Update()
    {
        if (enemySpawner == null)
            return;

        if (enemySpawner.IsWaveTimerRunning())
        {
            UpdateWaveTimerText();
        }
    }

    void CallWave()
    {
        if (enemySpawner == null)
            return;

        if (isPlacingTower)
            return;

        enemySpawner.CallNextWave();
        UpdateUI();
    }

    void UpdateUI()
    {
        UpdateWaveDisplay();
        UpdateWaveCallPanel();
        UpdateWaveTimerText();
    }

    void UpdateWaveDisplay()
    {
        if (waveDisplayText == null || enemySpawner == null)
            return;

        if (enemySpawner.AreAllWavesFinished())
        {
            waveDisplayText.text = "All Waves Cleared!";
            return;
        }

        int totalWaves = enemySpawner.GetTotalWaveCount();

        if (!enemySpawner.HasGameStarted())
        {
            waveDisplayText.text = "";
            return;
        }

        int currentWave = enemySpawner.GetCurrentWaveNumber();

        waveDisplayText.text = "Wave " + currentWave + " / " + totalWaves;
    }

    void UpdateWaveCallPanel()
    {
        if (waveCallPanel == null || enemySpawner == null)
            return;

        if (isPlacingTower)
        {
            waveCallPanel.SetActive(false);
            return;
        }

        bool canCallWave = enemySpawner.CanCallNextWave();

        waveCallPanel.SetActive(canCallWave);

        if (!canCallWave)
            return;
    }

    void UpdateWaveTimerText()
    {
        if (waveTimerText == null || enemySpawner == null)
            return;

        if (enemySpawner.IsWaveTimerRunning())
        {
            float timer = enemySpawner.GetCurrentWaveTimer();
            waveTimerText.text = "Next wave in: " + Mathf.CeilToInt(timer);
        }
        else
        {
            waveTimerText.text = "";
        }
    }
}