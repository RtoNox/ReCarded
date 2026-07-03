using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    protected Enemy target;
    protected int damage;
    protected float speed;
    protected float lifetime;

    protected bool initialized = false;

    public virtual void Initialize(Enemy newTarget, int newDamage, float newSpeed, float newLifetime)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        lifetime = newLifetime;

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

        Destroy(gameObject);
    }
}