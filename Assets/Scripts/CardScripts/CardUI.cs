using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Card Data")]
    public CardData cardData;

    [Header("Animated Art")]
    public AnimatedCardArtUI animatedCardArt;

    private CardPreviewUI cardPreviewUI;
    private CardPlayManager cardPlayManager;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalAnchoredPosition;
    private int originalIndex;

    public void Setup(
        CardData newCardData,
        CardPreviewUI previewUI,
        CardPlayManager playManager
    )
    {
        cardData = newCardData;
        cardPreviewUI = previewUI;
        cardPlayManager = playManager;

        RefreshUI();
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void RefreshUI()
    {
        if (cardData == null)
            return;

        if (animatedCardArt != null)
        {
            animatedCardArt.SetAnimation(
                cardData.cardAnimationFrames,
                cardData.animationFrameRate
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardPreviewUI != null)
        {
            cardPreviewUI.ShowPreview(cardData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardPreviewUI != null)
        {
            cardPreviewUI.HidePreview();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalIndex = transform.GetSiblingIndex();

        if (cardPreviewUI != null)
        {
            cardPreviewUI.HidePreview();
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
        animatedCardArt.cardImage.color = new Color(1f, 1f, 1f, 0.5f); // Make the card semi-transparent while dragging

        if (cardPlayManager != null)
        {
            cardPlayManager.BeginCardDrag(this, eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;

        if (cardPlayManager != null)
        {
            cardPlayManager.UpdateCardDrag(this, eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        animatedCardArt.cardImage.color = Color.white; // Restore the card's opacity

        bool cardWasPlayed = false;

        if (cardPlayManager != null)
        {
            cardWasPlayed = cardPlayManager.TryPlayCard(this, eventData.position);
            cardPlayManager.EndCardDrag();
        }

        if (!cardWasPlayed)
        {
            ReturnToHand();
        }
    }

    public void ReturnToHand()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        transform.SetSiblingIndex(originalIndex);
    }
}