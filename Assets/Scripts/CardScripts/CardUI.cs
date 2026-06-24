using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Data")]
    public CardData cardData;

    [Header("Card Visuals")]
    public Image sleeveImage;
    public Image iconImage;

    public TMP_Text nameText;
    public TMP_Text typeText;

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

        if (sleeveImage != null)
        {
            sleeveImage.sprite = cardData.cardSleeve;
            sleeveImage.enabled = cardData.cardSleeve != null;
        }

        if (iconImage != null)
        {
            iconImage.sprite = cardData.cardIcon;
            iconImage.enabled = cardData.cardIcon != null;
        }

        if (nameText != null)
            nameText.text = cardData.cardName;

        if (typeText != null)
            typeText.text = cardData.cardType.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardPreviewUI != null)
            cardPreviewUI.ShowPreview(cardData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardPreviewUI != null)
            cardPreviewUI.HidePreview();
    }
}