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

        if (cardPreviewUI != null)
        {
            cardPreviewUI.HidePreview();
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        bool cardWasPlayed = false;

        if (cardPlayManager != null)
        {
            cardWasPlayed = cardPlayManager.TryPlayCard(this, eventData.position);
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
    }
}