using UnityEngine;

public enum CardType
{
    Offensive,
    Sustain,
    Modification
}

public enum CardPlayType
{
    TargetTower,
    TargetEnemy,
    TargetArea,
    FullScreen,
    Instant
}

public enum OffensiveCardEffect
{
    None,
    DamageBoost,
    AttackSpeedBoost,
    InstantAreaDamage,
    PoisonAreaDamage,
    KillAllEnemiesOnScreen
}

public enum SustainCardEffect
{
    None,
    FlatAreaHeal,
    RegenField,
    Shield,
    MaxHPBoost,
    PassiveRegen,
    HealEverythingToFull
}

public enum ModificationCardEffect
{
    None,
    ModifyToMinigunner,
    ModifyToSniper,
    ModifyToRocketLauncher,
    ModifyToHealingTower,
    CopyCard,
    RemoveAllAttachedCards
}

[CreateAssetMenu(fileName = "New Card", menuName = "ReCarded/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;

    [TextArea(3, 6)]
    public string description;

    [Header("Card Identity")]
    public CardType cardType;
    public CardPlayType playType;

    [Header("Offensive Effect")]
    public OffensiveCardEffect offensiveEffect;

    [Header("Sustain Effect")]
    public SustainCardEffect sustainEffect;

    [Header("Modification Effect")]
    public ModificationCardEffect modificationEffect;

    [Header("Visuals")]
    public Sprite cardSleeve;
    public Sprite cardIcon;

    [Header("Tower Buff Values")]
    public int damageBonus;
    public float attackSpeedMultiplier = 1f;

    [Header("Spell Values")]
    public int instantDamage;
    public float areaRadius = 2f;

    [Header("Poison Values")]
    public int poisonDamagePerTick;
    public float poisonDuration = 5f;
    public float poisonTickRate = 1f;

    [Header("Sustain Values")]
    public int healAmount;
    public int shieldAmount;
    public int maxHealthBonus;
    public int passiveRegenAmount;

    [Header("Area Sustain Values")]
    public float sustainAreaRadius = 2.5f;
    public float sustainDuration = 5f;
    public float sustainTickRate = 1f;
    public int sustainHealPerTick;

    [Header("Upgrade Prefab")]
    public GameObject upgradedTowerPrefab;
}