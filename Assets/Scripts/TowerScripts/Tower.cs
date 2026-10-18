using UnityEngine;
using System.Collections.Generic;

public enum TargetingMode
{
    First,
    Last,
    Strongest,
    Weakest,
    Closest
}

public class Tower : MonoBehaviour
{
    [Header("Basic Info")]
    public string towerName = "Basic Tower";

    [Header("UI Display")]
    public Sprite towerIcon;
    public List<Sprite> attachedCardIcons = new List<Sprite>();

    [Header("Tower Stats")]
    public int damage = 2;
    public float range = 3f;
    public float fireRate = 1f;
    public float projectileSpeed = 8f;
    public float projectileLifetime = 3f;

    [Header("AoE Stats")]
    [Tooltip("For the Rocketeer Tower")]
    public float explosionRadius = 1.5f;

    [Header("Economy")]
    public int purchaseCost = 50;
    [Range(0f, 1f)]
    public float sellRefundPercent = 0.5f;
    public int extraSellValue = 0;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public LayerMask enemyLayer;

    [Header("Targeting")]
    public TargetingMode targetingMode = TargetingMode.First;

    [Header("Selection Visuals")]
    public SpriteRenderer towerSpriteRenderer;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public LineRenderer selectedRangeLine;
    public int rangeCircleSegments = 80;
    public float rangeLineWidth = 0.05f;

    [Tooltip("Status Effects")]
    public bool IsStunned { get; private set; }
    public bool IsAttackSpeedDebuffed { get; private set; }

    private float stunTimer = 0f;
    private float attackSpeedDebuffTimer = 0f;
    private float attackSpeedIntervalMultiplier = 1f;
    private float fireCooldown = 0f;
    private TowerPlacementManager placementManager;

    [Header("Sound Effects")]
    public AudioClip sellSound;

    void Awake()
    {
        if (towerSpriteRenderer == null)
        {
            towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        SetupRangeLine();

        SetSelected(false);
    }

    void Update()
    {
        UpdateAttackSpeedDebuff();

        // A stun pauses all tower behavior handled by this Update method.
        // The firing cooldown is deliberately paused rather than reset.
        if (IsStunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
            {
                stunTimer = 0f;
                IsStunned = false;
            }

            return;
        }

        // While debuffed, the cooldown advances more slowly. Because the
        // cooldown itself remains based on the normal fireRate, the slowdown
        // ends cleanly when the debuff expires.
        float cooldownDelta = IsAttackSpeedDebuffed
            ? Time.deltaTime / attackSpeedIntervalMultiplier
            : Time.deltaTime;
        fireCooldown -= cooldownDelta;

        if (fireCooldown <= 0f && fireRate > 0f)
        {
            Enemy target = FindTarget();

            if (target != null)
            {
                Shoot(target);
                fireCooldown = fireRate;
            }
        }
    }

    public void InitializeTower(TowerPlacementManager newPlacementManager, int newPurchaseCost)
    {
        placementManager = newPlacementManager;
        purchaseCost = newPurchaseCost;
    }

    /// <summary>
    /// Temporarily disables this tower. Reapplying a stun keeps whichever
    /// duration is longer, so a shorter stun cannot reduce the time remaining.
    /// </summary>
    public void Stun(float duration)
    {
        if (duration <= 0f)
            return;

        stunTimer = Mathf.Max(stunTimer, duration);
        IsStunned = true;
    }

    /// <summary>
    /// Temporarily increases the interval between shots. This is separate from
    /// stun and does not modify the tower's base fireRate stat.
    /// A multiplier of 2 means the tower takes twice as long between shots.
    /// </summary>
    public void ApplyAttackSpeedDebuff(float duration, float intervalMultiplier)
    {
        if (duration <= 0f || intervalMultiplier <= 1f)
            return;

        attackSpeedDebuffTimer = Mathf.Max(attackSpeedDebuffTimer, duration);
        attackSpeedIntervalMultiplier = Mathf.Max(
            attackSpeedIntervalMultiplier,
            intervalMultiplier
        );
        IsAttackSpeedDebuffed = true;
    }

    private void UpdateAttackSpeedDebuff()
    {
        if (!IsAttackSpeedDebuffed)
            return;

        attackSpeedDebuffTimer -= Time.deltaTime;

        if (attackSpeedDebuffTimer <= 0f)
        {
            attackSpeedDebuffTimer = 0f;
            attackSpeedIntervalMultiplier = 1f;
            IsAttackSpeedDebuffed = false;
        }
    }

    void SetupRangeLine()
    {
        if (selectedRangeLine == null)
            return;

        selectedRangeLine.useWorldSpace = false;
        selectedRangeLine.loop = true;
        selectedRangeLine.positionCount = rangeCircleSegments;
        selectedRangeLine.startWidth = rangeLineWidth;
        selectedRangeLine.endWidth = rangeLineWidth;

        DrawRangeCircle();
    }

    void DrawRangeCircle()
    {
        if (selectedRangeLine == null)
            return;

        selectedRangeLine.positionCount = rangeCircleSegments;

        for (int i = 0; i < rangeCircleSegments; i++)
        {
            float angle = ((float)i / rangeCircleSegments) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;

            selectedRangeLine.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    public void SetSelected(bool selected)
    {
        if (towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = selected ? selectedColor : normalColor;
        }

        if (selectedRangeLine != null)
        {
            selectedRangeLine.gameObject.SetActive(selected);

            if (selected)
            {
                DrawRangeCircle();
            }
        }
    }

    void Shoot(Enemy target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(name + " has no projectile prefab assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        TowerProjectile projectile = projectileObject.GetComponent<TowerProjectile>();

        if (projectile == null)
        {
            Debug.LogWarning(projectilePrefab.name + " does not have a TowerProjectile script!");
            Destroy(projectileObject);
            return;
        }

        projectile.Initialize(
            target,
            damage,
            projectileSpeed,
            projectileLifetime
        );

        RocketProjectile rocketProjectile = projectileObject.GetComponent<RocketProjectile>();

        if (rocketProjectile != null)
        {
            rocketProjectile.SetExplosionRadius(explosionRadius);
        }
    }

    Enemy FindTarget()
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                range,
                enemyLayer
            );

        if (hits.Length == 0)
            return null;

        switch (targetingMode)
        {
            case TargetingMode.First:
                return FindFirstTarget(hits);

            case TargetingMode.Last:
                return FindLastTarget(hits);

            case TargetingMode.Strongest:
                return FindStrongestTarget(hits);

            case TargetingMode.Weakest:
                return FindWeakestTarget(hits);

            case TargetingMode.Closest:
                return FindClosestTarget(hits);

            default:
                return FindFirstTarget(hits);
        }
    }

    Enemy FindFirstTarget(Collider2D[] hits)
    {
        Enemy bestTarget = null;
        float bestProgress = float.MinValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            float progress = enemy.GetPathProgress();

            if (progress > bestProgress)
            {
                bestProgress = progress;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    Enemy FindLastTarget(Collider2D[] hits)
    {
        Enemy bestTarget = null;
        float lowestProgress = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            float progress = enemy.GetPathProgress();

            if (progress < lowestProgress)
            {
                lowestProgress = progress;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    Enemy FindStrongestTarget(Collider2D[] hits)
    {
        Enemy bestTarget = null;
        int highestHealth = int.MinValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            if (enemy.GetHealth() > highestHealth)
            {
                highestHealth = enemy.GetHealth();
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    Enemy FindWeakestTarget(Collider2D[] hits)
    {
        Enemy bestTarget = null;
        int lowestHealth = int.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            if (enemy.GetHealth() < lowestHealth)
            {
                lowestHealth = enemy.GetHealth();
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    Enemy FindClosestTarget(Collider2D[] hits)
    {
        Enemy bestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    public void NextTargetingMode()
    {
        int modeCount =
            System.Enum.GetValues(typeof(TargetingMode)).Length;

        targetingMode =
            (TargetingMode)(((int)targetingMode + 1) % modeCount);
    }

    public void PreviousTargetingMode()
    {
        int modeCount =
            System.Enum.GetValues(typeof(TargetingMode)).Length;

        targetingMode =
            (TargetingMode)(((int)targetingMode - 1 + modeCount) % modeCount);
    }

    public string GetTargetingModeName()
    {
        return targetingMode.ToString();
    }

    public virtual List<TowerStatDisplayData> GetDisplayStats()
    {
        List<TowerStatDisplayData> stats = new List<TowerStatDisplayData>();
        TowerHealth towerHealth = GetComponent<TowerHealth>();

        if (towerHealth != null)
        {
            stats.Add(new TowerStatDisplayData(
                "HP",
                towerHealth.GetCurrentHealth() + " / " + towerHealth.GetMaxHealth()
            ));
        }

        stats.Add(new TowerStatDisplayData("Damage", damage.ToString()));
        stats.Add(new TowerStatDisplayData("Range", range.ToString("0.0")));
        stats.Add(new TowerStatDisplayData("Fire Rate", fireRate.ToString("0.0") + "s"));

        return stats;
    }

    public virtual int GetSellValue()
    {
        int baseRefund = Mathf.RoundToInt(purchaseCost * sellRefundPercent);
        return baseRefund + extraSellValue;
    }

    public virtual void Sell()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCash(GetSellValue());
        }

        if (placementManager != null)
        {
            placementManager.RemovePlacedTower();
        }

        AudioManager.Instance.PlaySound(sellSound);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}