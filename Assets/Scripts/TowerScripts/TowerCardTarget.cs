using System.Collections.Generic;
using UnityEngine;

public class TowerCardTarget : MonoBehaviour
{
    [Header("Tower Slot Info")]
    public TowerPlacementManager placementManager;
    public int purchaseCost;
    public GameObject basicTowerPrefab;

    [Header("Attached Cards")]
    public List<CardData> attachedCards = new List<CardData>();
    public List<Sprite> attachedCardIcons = new List<Sprite>();

    [Header("Card Target Highlight")]
    public SpriteRenderer[] spriteRenderers;
    public Color validCardTargetColor = Color.yellow;

    private Color[] originalColors;
    private bool hasStoredOriginalColors = false;

    void Awake()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        StoreOriginalColors();
    }

    public void Initialize(
        TowerPlacementManager newPlacementManager,
        int newPurchaseCost,
        GameObject newBasicTowerPrefab
    )
    {
        placementManager = newPlacementManager;
        purchaseCost = newPurchaseCost;
        basicTowerPrefab = newBasicTowerPrefab;

        Tower tower = GetComponent<Tower>();

        if (tower != null)
        {
            tower.InitializeTower(newPlacementManager, newPurchaseCost);
            tower.attachedCardIcons = attachedCardIcons;
        }
    }

    void StoreOriginalColors()
    {
        if (spriteRenderers == null)
            return;

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                originalColors[i] = spriteRenderers[i].color;
            }
        }

        hasStoredOriginalColors = true;
    }

    public void SetCardTargetHighlight(bool highlighted)
    {
        if (spriteRenderers == null)
            return;

        if (!hasStoredOriginalColors)
        {
            StoreOriginalColors();
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            if (highlighted)
            {
                spriteRenderers[i].color = validCardTargetColor;
            }
            else
            {
                if (originalColors != null && i < originalColors.Length)
                {
                    spriteRenderers[i].color = originalColors[i];
                }
            }
        }
    }

    public bool CanAttachCard(CardData cardData)
    {
        if (cardData == null)
            return false;

        if (cardData.canStack)
            return true;

        string newCardStackKey = GetStackKey(cardData);

        foreach (CardData attachedCard in attachedCards)
        {
            if (attachedCard == null)
                continue;

            string attachedStackKey = GetStackKey(attachedCard);

            if (attachedStackKey == newCardStackKey)
            {
                return false;
            }
        }

        return true;
    }

    string GetStackKey(CardData cardData)
    {
        if (cardData == null)
            return "";

        if (!string.IsNullOrEmpty(cardData.stackKey))
            return cardData.stackKey;

        return cardData.cardName;
    }

    public void AttachCard(CardData cardData)
    {
        if (cardData == null)
            return;

        attachedCards.Add(cardData);

        Sprite icon = GetCardIcon(cardData);

        if (icon != null)
        {
            attachedCardIcons.Add(icon);
        }

        Tower tower = GetComponent<Tower>();

        if (tower != null)
        {
            tower.attachedCardIcons = attachedCardIcons;
        }
    }

    Sprite GetCardIcon(CardData cardData)
    {
        if (cardData.cardAnimationFrames != null && cardData.cardAnimationFrames.Length > 0)
        {
            return cardData.cardAnimationFrames[0];
        }

        return null;
    }

    public TowerCardTarget ReplaceWithPrefab(GameObject newTowerPrefab, CardData attachedCard)
    {
        if (newTowerPrefab == null)
            return null;

        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        GameObject newTowerObject = Instantiate(
            newTowerPrefab,
            oldPosition,
            oldRotation
        );

        TowerCardTarget newCardTarget =
            newTowerObject.GetComponent<TowerCardTarget>();

        if (newCardTarget != null)
        {
            newCardTarget.Initialize(
                placementManager,
                purchaseCost,
                basicTowerPrefab
            );

            newCardTarget.attachedCards = new List<CardData>(attachedCards);
            newCardTarget.attachedCardIcons = new List<Sprite>(attachedCardIcons);

            if (attachedCard != null)
            {
                newCardTarget.AttachCard(attachedCard);
            }
        }

        Destroy(gameObject);

        return newCardTarget;
    }

    public TowerCardTarget RevertToBasicTower()
    {
        if (basicTowerPrefab == null)
        {
            Debug.LogWarning(name + " cannot revert because no basic tower prefab was assigned.");
            return null;
        }

        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        GameObject newTowerObject = Instantiate(
            basicTowerPrefab,
            oldPosition,
            oldRotation
        );

        TowerCardTarget newCardTarget =
            newTowerObject.GetComponent<TowerCardTarget>();

        if (newCardTarget != null)
        {
            newCardTarget.Initialize(
                placementManager,
                purchaseCost,
                basicTowerPrefab
            );

            newCardTarget.attachedCards.Clear();
            newCardTarget.attachedCardIcons.Clear();
        }

        Destroy(gameObject);

        return newCardTarget;
    }

    public void DestroyTower()
    {
        if (placementManager != null)
        {
            placementManager.RemovePlacedTower();
        }

        Destroy(gameObject);
    }
}