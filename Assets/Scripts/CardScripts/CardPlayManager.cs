using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public CardHandUI cardHandUI;
    public CardEffectResolver cardEffectResolver;
    public LayerMask towerLayer;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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