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

    private float fireCooldown = 0f;
    private TowerPlacementManager placementManager;

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
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Enemy target = FindTarget();

            if (target != null)
            {
                Shoot(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    public void InitializeTower(TowerPlacementManager newPlacementManager, int newPurchaseCost)
    {
        placementManager = newPlacementManager;
        purchaseCost = newPurchaseCost;
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

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}