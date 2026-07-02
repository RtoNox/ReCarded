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
    public float passiveRegenPercentPerTick = 0f;
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

        if (passiveRegenPercentPerTick > 0f)
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

    public void HealFlat(int amount)
    {
        if (amount <= 0)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    public void HealPercent(float percent)
    {
        if (percent <= 0f)
            return;

        int healAmount = Mathf.CeilToInt(maxHealth * percent);
        HealFlat(healAmount);
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void AddShieldFlat(int amount)
    {
        if (amount <= 0)
            return;

        shieldAmount += amount;
        UpdateHealthBar();
    }

    public void AddShieldPercent(float percent)
    {
        if (percent <= 0f)
            return;

        int shieldToAdd = Mathf.CeilToInt(maxHealth * percent);
        AddShieldFlat(shieldToAdd);
    }

    public void IncreaseMaxHealthFlat(int amount, bool alsoHealByAmount = true)
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

    public void IncreaseMaxHealthPercent(float percent, bool alsoHealByAmount = true)
    {
        if (percent <= 0f)
            return;

        int bonusHealth = Mathf.CeilToInt(maxHealth * percent);
        IncreaseMaxHealthFlat(bonusHealth, alsoHealByAmount);
    }

    public void AddPassiveRegenPercent(float percentPerTick, float tickRate)
    {
        if (percentPerTick <= 0f)
            return;

        passiveRegenPercentPerTick += percentPerTick;

        if (tickRate > 0f)
        {
            passiveRegenTickRate = tickRate;
        }

        if (passiveRegenCoroutine == null)
        {
            StartPassiveRegen();
        }
    }

    public void StartTimedRegenPercent(float percentPerTick, float duration, float tickRate)
    {
        if (percentPerTick <= 0f)
            return;

        if (duration <= 0f)
            return;

        if (tickRate <= 0f)
            tickRate = 1f;

        StartCoroutine(TimedRegenRoutine(percentPerTick, duration, tickRate));
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

            if (passiveRegenPercentPerTick > 0f)
            {
                HealPercent(passiveRegenPercentPerTick);
            }
        }
    }

    IEnumerator TimedRegenRoutine(float percentPerTick, float duration, float tickRate)
    {
        float timer = 0f;

        while (timer < duration)
        {
            HealPercent(percentPerTick);

            timer += tickRate;

            yield return new WaitForSeconds(tickRate);
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

        TowerCardTarget cardTarget = GetComponent<TowerCardTarget>();

        if (cardTarget != null)
        {
            cardTarget.DestroyTower();
            return;
        }

        Destroy(gameObject);
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

    public float GetPassiveRegenPercentPerTick()
    {
        return passiveRegenPercentPerTick;
    }

    public bool IsDamaged()
    {
        return currentHealth < maxHealth;
    }
}