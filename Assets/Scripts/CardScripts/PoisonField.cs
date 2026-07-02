using System.Collections;
using UnityEngine;

public class PoisonField : MonoBehaviour
{
    private float radius;
    private int damagePerTick;
    private float duration;
    private float tickRate;
    private LayerMask enemyLayer;

    public void Initialize(
        float newRadius,
        int newDamagePerTick,
        float newDuration,
        float newTickRate,
        LayerMask newEnemyLayer
    )
    {
        radius = newRadius;
        damagePerTick = newDamagePerTick;
        duration = newDuration;
        tickRate = newTickRate;
        enemyLayer = newEnemyLayer;

        StartCoroutine(PoisonRoutine());
    }

    IEnumerator PoisonRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            DamageEnemiesInField();

            timer += tickRate;

            yield return new WaitForSeconds(tickRate);
        }

        Destroy(gameObject);
    }

    void DamageEnemiesInField()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damagePerTick);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}