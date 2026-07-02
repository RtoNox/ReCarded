using UnityEngine;

public class CardEffectResolver : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask towerLayer;

    [Header("Area Effect Prefabs")]
    public GameObject poisonFieldPrefab;

    public bool ApplyCardToTower(CardData cardData, TowerCardTarget towerCardTarget)
    {
        if (cardData == null || towerCardTarget == null)
            return false;

        switch (cardData.cardType)
        {
            case CardType.Offensive:
                return ApplyOffensiveCardToTower(cardData, towerCardTarget);

            case CardType.Sustain:
                return ApplySustainCardToTower(cardData, towerCardTarget);

            case CardType.Modification:
                return ApplyModificationCardToTower(cardData, towerCardTarget);
        }

        return false;
    }

    public bool ApplyCardToArea(CardData cardData, Vector3 centerPosition)
    {
        if (cardData == null)
            return false;

        if (cardData.cardType == CardType.Offensive)
        {
            return ApplyOffensiveCardToArea(cardData, centerPosition);
        }

        return false;
    }

    public bool ApplyFullScreenCard(CardData cardData)
    {
        if (cardData == null)
            return false;

        if (cardData.cardType == CardType.Offensive)
        {
            if (cardData.offensiveEffect == OffensiveCardEffect.KillAllEnemiesOnScreen)
            {
                KillAllEnemiesOnScreen();
                return true;
            }
        }

        if (cardData.cardType == CardType.Sustain)
        {
            if (cardData.sustainEffect == SustainCardEffect.HealEverythingToFull)
            {
                HealAllTowersToFull();
                return true;
            }
        }

        return false;
    }

    public bool ApplyInstantCard(CardData cardData)
    {
        if (cardData == null)
            return false;

        if (cardData.cardType != CardType.Modification)
            return false;

        if (cardData.modificationEffect == ModificationCardEffect.CopyCard)
        {
            Debug.Log("Copy Card used. Copy behavior will be added later.");
            return true;
        }

        return false;
    }

    bool ApplyOffensiveCardToTower(CardData cardData, TowerCardTarget towerCardTarget)
    {
        Tower tower = towerCardTarget.GetComponent<Tower>();

        if (tower == null)
        {
            Debug.Log("This tower cannot receive offensive tower buffs.");
            return false;
        }

        switch (cardData.offensiveEffect)
        {
            case OffensiveCardEffect.DamageBoost:
                tower.damage += cardData.damageBonus;
                towerCardTarget.AttachCard(cardData);
                return true;

            case OffensiveCardEffect.AttackSpeedBoost:
                tower.fireRate *= cardData.attackSpeedMultiplier;
                towerCardTarget.AttachCard(cardData);
                return true;
        }

        return false;
    }

    bool ApplySustainCardToTower(CardData cardData, TowerCardTarget towerCardTarget)
    {
        TowerHealth towerHealth = towerCardTarget.GetComponent<TowerHealth>();

        if (towerHealth == null)
            return false;

        switch (cardData.sustainEffect)
        {
            case SustainCardEffect.SingleTargetHeal:
                towerHealth.HealPercent(cardData.healPercent);
                towerCardTarget.AttachCard(cardData);
                return true;

            case SustainCardEffect.TimedRegen:
                towerHealth.StartTimedRegenPercent(
                    cardData.timedRegenPercentPerTick,
                    cardData.timedRegenDuration,
                    cardData.timedRegenTickRate
                );

                towerCardTarget.AttachCard(cardData);
                return true;

            case SustainCardEffect.Shield:
                towerHealth.AddShieldPercent(cardData.shieldPercent);
                towerCardTarget.AttachCard(cardData);
                return true;

            case SustainCardEffect.MaxHPBoost:
                towerHealth.IncreaseMaxHealthPercent(cardData.maxHealthBonusPercent);
                towerCardTarget.AttachCard(cardData);
                return true;

            case SustainCardEffect.PassiveRegen:
                towerHealth.AddPassiveRegenPercent(
                    cardData.passiveRegenPercentPerTick,
                    cardData.passiveRegenTickRate
                );

                towerCardTarget.AttachCard(cardData);
                return true;
        }

        return false;
    }

    bool ApplyModificationCardToTower(CardData cardData, TowerCardTarget towerCardTarget)
    {
        switch (cardData.modificationEffect)
        {
            case ModificationCardEffect.ModifyToMinigunner:
            case ModificationCardEffect.ModifyToSniper:
            case ModificationCardEffect.ModifyToRocketLauncher:
            case ModificationCardEffect.ModifyToMedic:
            case ModificationCardEffect.ModifyToTrapper:
                if (cardData.modifiedTowerPrefab == null)
                {
                    Debug.LogWarning(cardData.cardName + " has no modified tower prefab assigned.");
                    return false;
                }

                towerCardTarget.ReplaceWithPrefab(cardData.modifiedTowerPrefab, cardData);
                return true;

            case ModificationCardEffect.RemoveAllAttachedCards:
                towerCardTarget.RevertToBasicTower();
                return true;
        }

        return false;
    }

    bool ApplyOffensiveCardToArea(CardData cardData, Vector3 centerPosition)
    {
        switch (cardData.offensiveEffect)
        {
            case OffensiveCardEffect.InstantAreaDamage:
                DamageEnemiesInArea(
                    centerPosition,
                    cardData.areaRadius,
                    cardData.instantDamage
                );
                return true;

            case OffensiveCardEffect.PoisonAreaDamage:
                SpawnPoisonField(cardData, centerPosition);
                return true;
        }

        return false;
    }

    void DamageEnemiesInArea(Vector3 centerPosition, float radius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            centerPosition,
            radius,
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
    }

    void SpawnPoisonField(CardData cardData, Vector3 centerPosition)
    {
        GameObject fieldObject;

        if (poisonFieldPrefab != null)
        {
            fieldObject = Instantiate(poisonFieldPrefab, centerPosition, Quaternion.identity);
        }
        else
        {
            fieldObject = new GameObject("Poison Field");
            fieldObject.transform.position = centerPosition;
        }

        PoisonField poisonField = fieldObject.GetComponent<PoisonField>();

        if (poisonField == null)
        {
            poisonField = fieldObject.AddComponent<PoisonField>();
        }

        poisonField.Initialize(
            cardData.areaRadius,
            cardData.poisonDamagePerTick,
            cardData.poisonDuration,
            cardData.poisonTickRate,
            enemyLayer
        );
    }

    void KillAllEnemiesOnScreen()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (IsVisibleOnScreen(enemy.transform.position))
            {
                enemy.TakeDamage(enemy.GetHealth());
            }
        }
    }

    bool IsVisibleOnScreen(Vector3 worldPosition)
    {
        Camera camera = Camera.main;

        if (camera == null)
            return true;

        Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);

        return viewportPosition.x >= 0f &&
               viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f &&
               viewportPosition.y <= 1f &&
               viewportPosition.z > 0f;
    }

    void HealAllTowersToFull()
    {
        TowerHealth[] towers = FindObjectsOfType<TowerHealth>();

        foreach (TowerHealth towerHealth in towers)
        {
            towerHealth.HealToFull();
        }
    }
}