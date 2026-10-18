using UnityEngine;

public class RocketProjectile : TowerProjectile
{
    [Header("Explosion")]
    public LayerMask enemyLayer;
    [Min(0f)] public float explosionRadius = 1.5f;

    [Header("Explosion Visual")]
    public GameObject areaEffectVisualPrefab;
    public Color explosionVisualColor = new Color(1f, 0.35f, 0.05f, 0.45f);

    public void SetExplosionRadius(float newExplosionRadius)
    {
        explosionRadius = Mathf.Max(0f, newExplosionRadius);
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
        // Visual only: the actual damage is still handled by the overlap check below.
        AreaEffectVisual.Spawn(
            areaEffectVisualPrefab,
            transform.position,
            explosionRadius,
            explosionVisualColor
        );

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
