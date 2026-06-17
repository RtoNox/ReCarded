using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int amount = 5;
    public float spawnDelay = 1f;
}

[System.Serializable]
public class Wave
{
    [Header("Enemy Groups")]
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

    [Header("Wave Timing")]
    public float delayBeforeWaveStarts = 0f;
    public float waveTimer = 20f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public BaseHealth baseHealth;

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    public event Action OnWaveStateChanged;
    public event Action OnVictory;

    private int nextWaveIndex = 0;
    private int activeWaveNumber = 0;

    private bool gameStarted = false;
    private bool waveIsSpawning = false;
    private bool waveTimerIsRunning = false;
    private bool allWavesFinished = false;
    private bool victoryTriggered = false;

    private Coroutine waveTimerCoroutine;

    private List<Enemy> activeEnemies = new List<Enemy>();

    private float currentWaveTimer = 0f;

    void Start()
    {
        NotifyWaveStateChanged();
    }

    public void StartNextWave()
    {
        if (victoryTriggered)
            return;

        if (baseHealth != null && baseHealth.IsGameOver())
            return;

        if (waveIsSpawning)
        {
            Debug.Log("Cannot start next wave. A wave is still spawning.");
            return;
        }

        if (nextWaveIndex >= waves.Count)
        {
            Debug.Log("No more waves left.");
            allWavesFinished = true;
            NotifyWaveStateChanged();
            CheckForVictory();
            return;
        }

        StopWaveTimer();

        gameStarted = true;

        int waveIndexToSpawn = nextWaveIndex;
        Wave wave = waves[waveIndexToSpawn];

        activeWaveNumber = waveIndexToSpawn + 1;
        nextWaveIndex++;

        NotifyWaveStateChanged();

        StartCoroutine(StartWaveRoutine(wave));
    }

    IEnumerator StartWaveRoutine(Wave wave)
    {
        waveIsSpawning = true;
        waveTimerIsRunning = false;

        NotifyWaveStateChanged();

        Debug.Log("Preparing Wave " + nextWaveIndex);

        if (wave.delayBeforeWaveStarts > 0f)
        {
            yield return new WaitForSeconds(wave.delayBeforeWaveStarts);
        }

        Debug.Log("Starting Wave: " + nextWaveIndex);

        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.amount; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnDelay);
            }
        }

        Debug.Log("Finished spawning enemies for Wave " + nextWaveIndex);

        waveIsSpawning = false;

        NotifyWaveStateChanged();

        if (activeEnemies.Count <= 0)
        {
            Debug.Log("No enemies left after spawning.");
            CheckForVictory();

            if (!victoryTriggered)
            {
                StartNextWave();
            }

            yield break;
        }

        if (HasNextWave())
        {
            StartWaveTimer(wave.waveTimer);
        }
        else
        {
            Debug.Log("Final wave spawned. Waiting for remaining enemies to be defeated.");
            StopWaveTimer();
            NotifyWaveStateChanged();
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject enemyObj = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.waypoints = waypoints;
            enemy.OnEnemyDeath += HandleEnemyRemoved;

            activeEnemies.Add(enemy);
        }
        else
        {
            Debug.LogWarning(enemyPrefab.name + " does not have an Enemy script!");
        }
    }

    void HandleEnemyRemoved(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        Debug.Log("Enemy removed. Remaining enemies: " + activeEnemies.Count);

        if (!waveIsSpawning && activeEnemies.Count <= 0)
        {
            CheckForVictory();

            if (!victoryTriggered)
            {
                Debug.Log("All enemies defeated or escaped. Starting next wave automatically.");
                StartNextWave();
            }
        }

        NotifyWaveStateChanged();
    }

    void StartWaveTimer(float duration)
    {
        StopWaveTimer();

        waveTimerCoroutine = StartCoroutine(WaveTimerRoutine(duration));
    }

    IEnumerator WaveTimerRoutine(float duration)
    {
        waveTimerIsRunning = true;
        currentWaveTimer = duration;

        NotifyWaveStateChanged();

        while (currentWaveTimer > 0f)
        {
            currentWaveTimer -= Time.deltaTime;
            yield return null;
        }

        currentWaveTimer = 0f;
        waveTimerIsRunning = false;
        waveTimerCoroutine = null;

        NotifyWaveStateChanged();

        Debug.Log("Wave timer ended. Starting next wave automatically.");

        StartNextWave();
    }

    void StopWaveTimer()
    {
        if (waveTimerCoroutine != null)
        {
            StopCoroutine(waveTimerCoroutine);
            waveTimerCoroutine = null;
        }

        waveTimerIsRunning = false;
        currentWaveTimer = 0f;

        NotifyWaveStateChanged();
    }

    void CheckForVictory()
    {
        if (victoryTriggered)
            return;

        if (baseHealth != null && baseHealth.IsGameOver())
            return;

        bool noMoreWaves = nextWaveIndex >= waves.Count;
        bool noEnemiesLeft = activeEnemies.Count <= 0;
        bool notSpawning = !waveIsSpawning;

        if (gameStarted && noMoreWaves && noEnemiesLeft && notSpawning)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        victoryTriggered = true;
        allWavesFinished = true;

        StopWaveTimer();

        Debug.Log("Victory! All waves cleared!");

        NotifyWaveStateChanged();
        OnVictory?.Invoke();
    }

    public void CallNextWave()
    {
        if (victoryTriggered)
            return;

        if (baseHealth != null && baseHealth.IsGameOver())
            return;

        if (!gameStarted)
        {
            Debug.Log("Game has not started yet. Starting first wave.");
            StartNextWave();
            return;
        }

        if (waveIsSpawning)
        {
            Debug.Log("Cannot call next wave while enemies are still spawning.");
            return;
        }

        if (!waveTimerIsRunning)
        {
            Debug.Log("Cannot call next wave right now. Wave timer is not active.");
            return;
        }

        if (!HasNextWave())
        {
            Debug.Log("Cannot call next wave. There are no more waves.");
            return;
        }

        Debug.Log("Player called the next wave early!");

        StartNextWave();
    }

    public bool CanCallNextWave()
    {
        if (victoryTriggered)
            return false;

        if (allWavesFinished)
            return false;

        if (baseHealth != null && baseHealth.IsGameOver())
            return false;

        if (!gameStarted)
            return waves.Count > 0;

        return waveTimerIsRunning && !waveIsSpawning && HasNextWave();
    }

    public bool HasNextWave()
    {
        return nextWaveIndex < waves.Count;
    }

    public int GetCurrentWaveNumber()
    {
        if (!gameStarted)
            return 0;

        return activeWaveNumber;
    }

    public int GetNextWaveNumber()
    {
        return nextWaveIndex + 1;
    }

    public int GetTotalWaveCount()
    {
        return waves.Count;
    }

    public bool HasGameStarted()
    {
        return gameStarted;
    }

    public bool AreAllWavesFinished()
    {
        return allWavesFinished;
    }

    public bool HasVictoryTriggered()
    {
        return victoryTriggered;
    }

    public int GetRemainingEnemyCount()
    {
        return activeEnemies.Count;
    }

    public float GetCurrentWaveTimer()
    {
        return currentWaveTimer;
    }

    public bool IsWaveSpawning()
    {
        return waveIsSpawning;
    }

    public bool IsWaveTimerRunning()
    {
        return waveTimerIsRunning;
    }

    void NotifyWaveStateChanged()
    {
        OnWaveStateChanged?.Invoke();
    }
}