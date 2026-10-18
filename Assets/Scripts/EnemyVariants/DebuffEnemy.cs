using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// On the enemy's first successful damage event, it permanently speeds up
/// itself and briefly slows the attack rate of nearby towers.
/// Attach alongside the existing Enemy component.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class DebuffEnemy : MonoBehaviour
{
    [Header("Self Speed Boost")]
    [Tooltip("Movement speed multiplier applied when this enemy first takes damage. The boost lasts for the rest of its life.")]
    [Min(1f)]
    public float speedMultiplier = 1.5f;

    [Header("Tower Attack-Speed Debuff")]
    [Min(0f)]
    public float debuffRadius = 2.5f;

    [Min(0f)]
    public float debuffDuration = 3f;

    [Tooltip("Multiplier for the interval between tower shots. 2 means towers take twice as long between shots.")]
    [Min(1f)]
    public float attackIntervalMultiplier = 2f;

    [Tooltip("Set this to the layer used by tower colliders.")]
    public LayerMask towerLayer;

    [Header("Area Visual")]
    [Tooltip("Assign the AreaEffectVisual prefab created for the area-effect feature.")]
    public GameObject areaEffectVisualPrefab;

    public Color debuffVisualColor = new Color(0.65f, 0.25f, 1f, 0.4f);

    [Min(0f)]
    public float visualDuration = 0.5f;

    private Enemy enemy;
    private bool hasTriggered;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.OnEnemyDamaged += HandleEnemyDamaged;
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnEnemyDamaged -= HandleEnemyDamaged;
        }
    }

    private void HandleEnemyDamaged(Enemy damagedEnemy)
    {
        if (hasTriggered || damagedEnemy == null)
            return;

        hasTriggered = true;
        TriggerAbility();
    }

    private void TriggerAbility()
    {
        // The speed boost is permanent for this enemy's remaining lifetime.
        enemy.speed *= Mathf.Max(1f, speedMultiplier);

        Vector3 effectPosition = transform.position;

        AreaEffectVisual.Spawn(
            areaEffectVisualPrefab,
            effectPosition,
            debuffRadius,
            debuffVisualColor,
            visualDuration
        );

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            effectPosition,
            debuffRadius,
            towerLayer
        );

        // A tower may have multiple colliders, so only debuff each tower once.
        HashSet<Tower> affectedTowers = new HashSet<Tower>();

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            Tower tower = hit.GetComponentInParent<Tower>();

            if (tower != null && affectedTowers.Add(tower))
            {
                tower.ApplyAttackSpeedDebuff(debuffDuration, attackIntervalMultiplier);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.65f, 0.25f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, debuffRadius);
    }
#endif
}
