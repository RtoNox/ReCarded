using UnityEngine;
using System.Collections;

public class TowerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Shield")]
    public int shieldAmount = 0;

    [Header("Passive Regen")]
    public int passiveRegenPerTick = 0;
    public float passiveRegenTickRate = 1f;

    [Header("UI")]
    public HealthBarUI healthBarUI;

    private Coroutine passiveRegenCoroutine;
    private Tower tower;

    void Awake()
    {
        tower = GetComponent<Tower>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (passiveRegenPerTick > 0)
        {
            StartPassiveRegen();
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        int remainingDamage = damage;

        if (shieldAmount > 0)
        {
            int blockedDamage = Mathf.Min(shieldAmount, remainingDamage);
            shieldAmount -= blockedDamage;
            remainingDamage -= blockedDamage;
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void AddShield(int amount)
    {
        if (amount <= 0)
            return;

        shieldAmount += amount;
        UpdateHealthBar();
    }

    public void IncreaseMaxHealth(int amount, bool alsoHealByAmount = true)
    {
        if (amount <= 0)
            return;

        maxHealth += amount;

        if (alsoHealByAmount)
        {
            currentHealth += amount;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    public void AddPassiveRegen(int amount)
    {
        if (amount <= 0)
            return;

        passiveRegenPerTick += amount;

        if (passiveRegenCoroutine == null)
        {
            StartPassiveRegen();
        }
    }

    void StartPassiveRegen()
    {
        if (passiveRegenCoroutine != null)
        {
            StopCoroutine(passiveRegenCoroutine);
        }

        passiveRegenCoroutine = StartCoroutine(PassiveRegenRoutine());
    }

    IEnumerator PassiveRegenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveRegenTickRate);

            if (passiveRegenPerTick > 0)
            {
                Heal(passiveRegenPerTick);
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        Debug.Log(name + " was destroyed!");

        if (tower != null)
        {
            tower.DestroyTower();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetShieldAmount()
    {
        return shieldAmount;
    }

    public int GetPassiveRegenPerTick()
    {
        return passiveRegenPerTick;
    }

    public bool IsDamaged()
    {
        return currentHealth < maxHealth;
    }
}