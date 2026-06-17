using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Pathing")]
    public Transform[] waypoints;
    public float speed = 2f;

    [Header("Stats")]
    public int health = 10;

    public Action<Enemy> OnEnemyDeath;

    private int currentWaypoint = 0;
    private bool hasBeenRemoved = false;

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

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
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