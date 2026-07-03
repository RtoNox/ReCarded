using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Targeting")]
    public LayerMask enemyLayer;

    private int damage;
    private bool destroyAfterTrigger = true;
    private bool triggered = false;

    public void Initialize(int newDamage, bool newDestroyAfterTrigger)
    {
        damage = newDamage;
        destroyAfterTrigger = newDestroyAfterTrigger;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (((1 << other.gameObject.layer) & enemyLayer) == 0)
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        triggered = true;

        enemy.TakeDamage(damage);

        if (destroyAfterTrigger)
        {
            Destroy(gameObject);
        }
        else
        {
            triggered = false;
        }
    }
}