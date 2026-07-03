using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RocketProjectile : TowerProjectile
{
    [Header("Explosion")]
    public LayerMask enemyLayer;

    private float explosionRadius = 1.5f;

    public void SetExplosionRadius(float newExplosionRadius)
    {
        explosionRadius = newExplosionRadius;
    }

    protected override void HitTarget()
    {
        Explode();
    }

    protected override void Update()
    {
        if (!initialized)
            return;

        if (target == null)
        {
            Explode();
            return;
        }

        MoveTowardsTarget();
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}