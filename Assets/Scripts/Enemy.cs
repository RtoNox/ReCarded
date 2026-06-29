using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Pathing")]
    public Transform[] waypoints;
    public float speed = 2f;

    [Header("Stats")]
    public int maxHealth = 10;
    public int health = 10;
    public int cashReward = 10;

    [Header("UI")]
    public HealthBarUI healthBarUI;

    public Action<Enemy> OnEnemyDeath;

    private int currentWaypoint = 0;
    private bool hasBeenRemoved = false;

    void Start()
    {
        health = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (currentWaypoint >= waypoints.Length)
        {
            ReachBase();
            return;
        }

        Transform target = waypoints[currentWaypoint];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            currentWaypoint++;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0)
        {
            health = 0;
        }

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(health, maxHealth);
        }
    }

    public int GetCurrentWaypoint()
    {
        return currentWaypoint;
    }

    public float GetPathProgress()
    {
        if (waypoints == null || waypoints.Length == 0)
            return 0f;

        if (currentWaypoint >= waypoints.Length)
            return float.MaxValue;

        Transform targetWaypoint = waypoints[currentWaypoint];

        float distanceToWaypoint =
            Vector2.Distance(transform.position, targetWaypoint.position);

        return currentWaypoint + (1f / (distanceToWaypoint + 1f));
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    void Die()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCash(cashReward);
        }

        RemoveEnemy();
        Destroy(gameObject);
    }

    void ReachBase()
    {
        BaseHealth baseHealth = FindObjectOfType<BaseHealth>();

        if (baseHealth != null)
        {
            baseHealth.TakeDamage(health);
        }

        Debug.Log("Enemy reached the base and dealt " + health + " damage!");

        RemoveEnemy();
        Destroy(gameObject);
    }

    void RemoveEnemy()
    {
        if (hasBeenRemoved)
            return;

        hasBeenRemoved = true;

        OnEnemyDeath?.Invoke(this);
    }
}