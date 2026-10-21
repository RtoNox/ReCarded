using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    protected Enemy target;
    protected int damage;
    protected float speed;
    protected float lifetime;
    protected Color projectileColor;

    public SpriteRenderer spriteRenderer;
    public TrailRenderer trailRenderer;
    public ParticleSystem projectileParticles;

    protected bool initialized = false;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        if (projectileParticles == null)
        {
            projectileParticles = GetComponent<ParticleSystem>();
        }
    }

    public virtual void Initialize(Enemy newTarget, int newDamage, float newSpeed, float newLifetime, Color newColor)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        lifetime = newLifetime;
        projectileColor = newColor;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = projectileColor;
        }

        if (trailRenderer != null)
        {
            trailRenderer.startColor = projectileColor;
            trailRenderer.endColor = new Color(projectileColor.r, projectileColor.g, projectileColor.b, 0f);
        }

        if (projectileParticles != null)
        {
            var main = projectileParticles.main;
            main.startColor = projectileColor;
        }

        initialized = true;

        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        if (!initialized)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector3 targetPosition = target.transform.position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        Vector2 direction = targetPosition - transform.position;

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (distanceToTarget <= 0.05f)
        {
            HitTarget();
        }
    }

    protected virtual void HitTarget()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        if (projectileParticles != null)
        {
            projectileParticles.transform.parent = null;
            projectileParticles.Stop();
        }

        Destroy(gameObject);
    }
}