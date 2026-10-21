using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stuns nearby towers when this enemy is killed.
/// Attach this alongside the existing Enemy component on the enemy prefab.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class DeathStunEnemy : MonoBehaviour
{
    private const float StunDuration = 3f;

    [Header("Death Stun")]
    [Min(0f)]
    public float stunRadius = 2f;

    [Tooltip("Set this to the layer used by tower GameObjects.")]
    public LayerMask towerLayer;

    [Header("Visual Effect")]
    [Tooltip("Assign the AreaEffectVisual prefab created for the area-effect feature.")]
    public GameObject areaEffectVisualPrefab;

    public Color stunVisualColor = new Color(0.2f, 0.65f, 1f, 0.45f);

    private Enemy enemy;
    private bool hasTriggered;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.OnEnemyDeath += HandleEnemyRemoved;
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnEnemyDeath -= HandleEnemyRemoved;
        }
    }

    private void HandleEnemyRemoved(Enemy removedEnemy)
    {
        // Enemy.OnEnemyDeath is also invoked when an enemy reaches the base.
        // Only trigger this ability when the enemy was actually killed.
        if (hasTriggered || removedEnemy == null || removedEnemy.GetHealth() > 0)
        {
            return;
        }

        hasTriggered = true;
        TriggerDeathStun();
    }

    private void TriggerDeathStun()
    {
        Vector3 deathPosition = transform.position;

        // This is a short visual pulse; the gameplay stun itself lasts 3 seconds.
        AreaEffectVisual.Spawn(
            areaEffectVisualPrefab,
            deathPosition,
            stunRadius,
            stunVisualColor
        );

        ScreenFlasher.Instance.FlashScreen(Color.white, 1f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            deathPosition,
            stunRadius,
            towerLayer
        );

        // A tower may have multiple colliders, so only stun each tower once.
        HashSet<Tower> affectedTowers = new HashSet<Tower>();

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            Tower tower = hit.GetComponentInParent<Tower>();

            if (tower != null && affectedTowers.Add(tower))
            {
                tower.Stun(StunDuration);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.65f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}
