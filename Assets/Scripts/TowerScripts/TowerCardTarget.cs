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

    public virtual void DestroyTower()
    {
        if (placementManager != null)
        {
            placementManager.RemovePlacedTower();
        }

        Destroy(gameObject);
    }
}