using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private Enemy target;
    private int damage;
    private float speed;
    private float lifetime;

    private bool initialized = false;

    public void Initialize(Enemy newTarget, int newDamage, float newSpeed, float newLifetime)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        lifetime = newLifetime;

        initialized = true;

        Destroy(gameObject, lifetime);
    }

    void Update()
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

    void MoveTowardsTarget()
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

    void HitTarget()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}