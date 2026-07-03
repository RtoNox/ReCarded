using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public CardHandUI cardHandUI;
    public CardEffectResolver cardEffectResolver;

    [Header("Target Layers")]
    public LayerMask towerLayer;

    [Header("Drag Preview")]
    public CardAreaPreview cardAreaPreview;

    private CardUI currentlyDraggedCard;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void BeginCardDrag(CardUI cardUI, Vector2 screenPosition)
    {
        currentlyDraggedCard = cardUI;

        if (cardUI == null || cardUI.cardData == null)
            return;

        ShowDragPreview(cardUI.cardData, screenPosition);
    }

    public void UpdateCardDrag(CardUI cardUI, Vector2 screenPosition)
    {
        if (cardUI == null || cardUI.cardData == null)
            return;

        UpdateDragPreview(cardUI.cardData, screenPosition);
    }

    public void EndCardDrag()
    {
        currentlyDraggedCard = null;

        HideAreaPreview();
        ClearTowerHighlights();
    }

    public bool TryPlayCard(CardUI cardUI, Vector2 screenPosition)
    {
        if (cardUI == null || cardUI.cardData == null)
            return false;

        if (cardEffectResolver == null)
        {
            Debug.LogWarning("No CardEffectResolver assigned!");
            return false;
        }

        CardData cardData = cardUI.cardData;

        Vector3 worldPosition = GetWorldPosition(screenPosition);

        bool success = false;

        switch (cardData.playType)
        {
            case CardPlayType.TargetTower:
                success = TryPlayOnTower(cardData, worldPosition);
                break;

            case CardPlayType.TargetArea:
                success = cardEffectResolver.ApplyCardToArea(cardData, worldPosition);
                break;

            case CardPlayType.FullScreen:
                success = cardEffectResolver.ApplyFullScreenCard(cardData);
                break;

            case CardPlayType.Instant:
                success = cardEffectResolver.ApplyInstantCard(cardData);
                break;
        }

        if (success)
        {
            if (cardHandUI != null)
            {
                cardHandUI.RemoveCardFromHand(cardUI);
            }
            else
            {
                Destroy(cardUI.gameObject);
            }
        }

        return success;
    }

    void ShowDragPreview(CardData cardData, Vector2 screenPosition)
    {
        if (cardData.playType == CardPlayType.TargetArea)
        {
            ShowAreaPreview(cardData, screenPosition);
            return;
        }

        if (cardData.playType == CardPlayType.TargetTower)
        {
            HighlightValidTowers(cardData);
            return;
        }

        HideAreaPreview();
        ClearTowerHighlights();
    }

    void UpdateDragPreview(CardData cardData, Vector2 screenPosition)
    {
        if (cardData.playType == CardPlayType.TargetArea)
        {
            ShowAreaPreview(cardData, screenPosition);
        }
    }

    void ShowAreaPreview(CardData cardData, Vector2 screenPosition)
    {
        if (cardAreaPreview == null)
            return;

        if (cardData.cardType != CardType.Offensive)
        {
            HideAreaPreview();
            return;
        }

        if (cardData.offensiveEffect != OffensiveCardEffect.InstantAreaDamage &&
            cardData.offensiveEffect != OffensiveCardEffect.PoisonAreaDamage)
        {
            HideAreaPreview();
            return;
        }

        Vector3 worldPosition = GetWorldPosition(screenPosition);

        cardAreaPreview.SetWorldPosition(worldPosition);
        cardAreaPreview.Show(cardData.areaRadius);
    }

    void HideAreaPreview()
    {
        if (cardAreaPreview != null)
        {
            cardAreaPreview.Hide();
        }
    }

    void HighlightValidTowers(CardData cardData)
    {
        ClearTowerHighlights();

        if (cardEffectResolver == null)
            return;

        TowerCardTarget[] towers = FindObjectsOfType<TowerCardTarget>();

        foreach (TowerCardTarget tower in towers)
        {
            if (tower == null)
                continue;

            bool isValidTarget =
                cardEffectResolver.CanCardBePlayedOnTower(cardData, tower);

            tower.SetCardTargetHighlight(isValidTarget);
        }
    }

    void ClearTowerHighlights()
    {
        TowerCardTarget[] towers = FindObjectsOfType<TowerCardTarget>();

        foreach (TowerCardTarget tower in towers)
        {
            if (tower != null)
            {
                tower.SetCardTargetHighlight(false);
            }
        }
    }

    bool TryPlayOnTower(CardData cardData, Vector3 worldPosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, towerLayer);

        if (hit == null)
            return false;

        TowerCardTarget towerCardTarget =
            hit.GetComponentInParent<TowerCardTarget>();

        if (towerCardTarget == null)
            return false;

        return cardEffectResolver.ApplyCardToTower(cardData, towerCardTarget);
    }

    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
}