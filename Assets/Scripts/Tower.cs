using UnityEngine;

public enum TargetingMode
{
    First,
    Last,
    Strong,
    Weak,
    Closest
}

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    public int damage = 2;
    public float range = 3f;
    public float fireRate = 1f;
    public float projectileSpeed = 8f;
    public float projectileLifetime = 3f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public LayerMask enemyLayer;

    [Header("Targeting")]
    public TargetingMode targetingMode = TargetingMode.First;

    private float fireCooldown = 0f;

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

            case TargetingMode.Strong:
                return FindStrongestTarget(hits);

            case TargetingMode.Weak:
                return FindWeakestTarget(hits);

            case TargetingMode.Closest:
                return FindClosestTarget(hits);

            default:
                return FindFirstTarget(hits);
        }
    }

    // Targeting mode methods
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

    // Targeting mode switching
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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}