using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveDisplayUI : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;
    public TowerPlacementManager towerPlacementManager;

    [Header("Wave Display")]
    public TMP_Text waveDisplayText;

    [Header("Wave Call UI")]
    public GameObject waveCallPanel;
    public Button waveCallButton;
    public TMP_Text waveCallText;
    public TMP_Text waveTimerText;

    private bool isPlacingTower = false;

    void OnEnable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStateChanged += UpdateUI;
        }

        if (towerPlacementManager != null)
        {
            towerPlacementManager.OnPlacementStateChanged += HandlePlacementStateChanged;
        }
    }

    void OnDisable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStateChanged -= UpdateUI;
        }

        if (towerPlacementManager != null)
        {
            towerPlacementManager.OnPlacementStateChanged -= HandlePlacementStateChanged;
        }
    }

    void Start()
    {
        if (waveCallButton != null)
        {
            waveCallButton.onClick.AddListener(CallWave);
        }

        if (towerPlacementManager != null)
        {
            isPlacingTower = towerPlacementManager.IsPlacingTower();
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

    void HandlePlacementStateChanged(bool placingTower)
    {
        isPlacingTower = placingTower;
        UpdateUI();
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

        if (!enemySpawner.HasGameStarted())
        {
            waveCallText.text = "Start Wave";
        }
        else
        {
            waveCallText.text = "Call Next Wave";
        }
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