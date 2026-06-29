using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    [Header("Preview Panel")]
    public GameObject previewPanel;

    [Header("Card Display")]
    public Image cardSleeveImage;
    public Image iconImage;

    public TMP_Text nameText;
    public TMP_Text typeText;
    public TMP_Text playTypeText;
    public TMP_Text descriptionText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        HidePreview();
    }

    public void ShowPreview(CardData cardData)
    {
        if (cardData == null)
            return;

        if (previewPanel != null)
            previewPanel.SetActive(true);

        if (cardSleeveImage != null)
        {
            cardSleeveImage.sprite = cardData.cardSleeve;
            cardSleeveImage.enabled = cardData.cardSleeve != null;
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

        if (playTypeText != null)
            playTypeText.text = "Play: " + cardData.playType.ToString();

        if (descriptionText != null)
            descriptionText.text = cardData.description;
    }

    public void HidePreview()
    {
        if (previewPanel != null)
            previewPanel.SetActive(false);
    }
}