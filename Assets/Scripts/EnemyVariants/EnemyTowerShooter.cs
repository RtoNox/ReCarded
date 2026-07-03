using UnityEngine;

public class EnemyTowerShooter : MonoBehaviour
{
    [Header("Shooting Stats")]
    public int damage = 5;
    public float range = 4f;
    public float fireRate = 0.5f;
    public float projectileSpeed = 6f;
    public float projectileLifetime = 3f;

    [Header("References")]
    public GameObject enemyProjectilePrefab;
    public Transform firePoint;
    public LayerMask towerLayer;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            TowerHealth target = FindTargetTower();

            if (target != null)
            {
                Shoot(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    TowerHealth FindTargetTower()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            range,
            towerLayer
        );

        if (hits.Length == 0)
            return null;

        TowerHealth closestTower = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            TowerHealth towerHealth = hit.GetComponent<TowerHealth>();

            if (towerHealth == null)
                continue;

            float distance = Vector2.Distance(transform.position, towerHealth.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = towerHealth;
            }
        }

        return closestTower;
    }

    void Shoot(TowerHealth target)
    {
        if (enemyProjectilePrefab == null)
        {
            Debug.LogWarning(name + " has no enemy projectile prefab assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }

        GameObject projectileObject = Instantiate(
            enemyProjectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        EnemyProjectile projectile = projectileObject.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            Debug.LogWarning(enemyProjectilePrefab.name + " does not have an EnemyProjectile script!");
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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}