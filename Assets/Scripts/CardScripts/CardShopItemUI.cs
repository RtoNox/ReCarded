using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardShopItemUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text cardNameText;
    public TMP_Text cardDescriptionText;
    public TMP_Text priceText;
    public Button buyButton;

    [Header("Card Art")]
    public AnimatedCardArtUI animatedCardArt;
    public Image fallbackCardImage;

    private CardData cardData;
    private ShopManager shopManager;

    public void Setup(CardData newCardData, ShopManager newShopManager)
    {
        cardData = newCardData;
        shopManager = newShopManager;

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(BuyCard);
            buyButton.onClick.AddListener(BuyCard);
        }

        RefreshUI();
        RefreshAffordability();
    }

    void RefreshUI()
    {
        if (cardData == null)
            return;

        if (cardNameText != null)
        {
            cardNameText.text = GetCardDisplayName();
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = cardData.effectDescription;
        }

        if (priceText != null)
        {
            priceText.text = "$" + Mathf.Max(0, cardData.shopPrice);
        }

        if (animatedCardArt != null)
        {
            animatedCardArt.SetAnimation(
                cardData.cardAnimationFrames,
                cardData.animationFrameRate
            );
        }
        else if (fallbackCardImage != null)
        {
            Sprite icon = GetCardIcon();
            fallbackCardImage.sprite = icon;
            fallbackCardImage.enabled = icon != null;
        }
    }

    public void RefreshAffordability()
    {
        if (buyButton == null || cardData == null)
            return;

        bool canAfford = CurrencyManager.Instance != null &&
                         CurrencyManager.Instance.CanAfford(Mathf.Max(0, cardData.shopPrice));

        buyButton.interactable = canAfford;
    }

    void BuyCard()
    {
        if (shopManager != null && cardData != null)
        {
            shopManager.BuySpecificCard(cardData);
        }
    }

    string GetCardDisplayName()
    {
        if (cardData == null)
            return "Unknown Card";

        if (!string.IsNullOrEmpty(cardData.cardName))
            return cardData.cardName;

        return cardData.name;
    }

    Sprite GetCardIcon()
    {
        if (cardData == null)
            return null;

        if (cardData.cardAnimationFrames != null && cardData.cardAnimationFrames.Length > 0)
            return cardData.cardAnimationFrames[0];

        return null;
    }
}
