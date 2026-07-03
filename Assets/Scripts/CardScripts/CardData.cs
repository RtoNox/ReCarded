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
    SingleTargetHeal,
    TimedRegen,
    Shield,
    MaxHPBoost,
    PassiveRegen,
    HealEverythingToFull
}

public enum ModificationCardEffect
{
    None,
    ModifyToMinigunner,
    ModifyToShotgunner,
    ModifyToSniper,
    ModifyToRocketLauncher,
    ModifyToMedic,
    ModifyToTrapper,
    CopyCard,
    RemoveAllAttachedCards
}

[CreateAssetMenu(fileName = "New Card", menuName = "ReCarded/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;

    [TextArea(3, 6)]
    public string effectDescription;

    [Header("Card Identity")]
    public CardType cardType;
    public CardPlayType playType;

    [Header("Shop Values")]
    [Tooltip("The guaranteed-buy price in the specific card shop.")]
    public int shopPrice = 100;

    [Tooltip("If false, this card will not appear in the specific card shop list.")]
    public bool canAppearInSpecificShop = true;

    [Tooltip("If false, this card cannot be rolled by the gacha.")]
    public bool canAppearInGacha = true;

    [Tooltip("Weighted gacha chance. Higher weight = more common. 0 = never rolls.")]
    [Min(0f)]
    public float gachaWeight = 1f;

    [Header("Offensive Effect")]
    public OffensiveCardEffect offensiveEffect;

    [Header("Sustain Effect")]
    public SustainCardEffect sustainEffect;

    [Header("Modification Effect")]
    public ModificationCardEffect modificationEffect;

    [Header("Card Animation")]
    public Sprite[] cardAnimationFrames;
    public float animationFrameRate = 10f;

    [Header("Tower Buff Values")]
    public int damageBonus;
    public float attackSpeedMultiplier = 1f;

    [Header("Stacking Rules")]
    [Tooltip("If true, this card can be attached multiple times to the same tower.")]
    public bool canStack = false;

    [Tooltip("Cards with the same stack key count as the same attached effect. Leave empty to use the card name.")]
    public string stackKey;

    [Header("Spell Values")]
    public int instantDamage;
    public float areaRadius = 2f;

    [Header("Poison Values")]
    public int poisonDamagePerTick;
    public float poisonDuration = 5f;
    public float poisonTickRate = 1f;

    [Header("Sustain Percentage Values")]
    [Tooltip("0.25 = heals 25% of the tower's max HP.")]
    public float healPercent = 0.25f;

    [Tooltip("0.05 = heals 5% of the tower's max HP per tick.")]
    public float timedRegenPercentPerTick = 0.05f;

    [Tooltip("How long the timed regen lasts.")]
    public float timedRegenDuration = 5f;

    [Tooltip("How often timed regen heals.")]
    public float timedRegenTickRate = 1f;

    [Tooltip("0.001 = heals 0.1% of the tower's max HP per tick permanently.")]
    public float passiveRegenPercentPerTick = 0.001f;

    [Tooltip("How often passive regen heals.")]
    public float passiveRegenTickRate = 1f;

    [Tooltip("0.25 = shield equal to 25% of the tower's max HP.")]
    public float shieldPercent = 0.25f;

    [Tooltip("0.25 = increases max HP by 25%.")]
    public float maxHealthBonusPercent = 0.25f;

    [Header("Upgrade Prefab")]
    public GameObject modifiedTowerPrefab;
}
