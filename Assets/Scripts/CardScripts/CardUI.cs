using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Data")]
    public CardData cardData;

    [Header("Animated Art")]
    public AnimatedCardArtUI animatedCardArt;

    private CardPreviewUI cardPreviewUI;

    public void Setup(CardData newCardData, CardPreviewUI previewUI)
    {
        cardData = newCardData;
        cardPreviewUI = previewUI;

        RefreshUI();
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
}