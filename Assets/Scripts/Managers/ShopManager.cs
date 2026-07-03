using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Core References")]
    public CardHandUI cardHandUI;
    public TowerPlacementManager towerPlacementManager;

    [Header("Shop Panels")]
    public GameObject shopPanel;
    public GameObject mainShopPanel;
    public GameObject specificCardShopPanel;

    [Header("Card Shop List")]
    public Transform cardShopItemsParent;
    public CardShopItemUI cardShopItemPrefab;

    [Header("Buttons")]
    public Button toggleShopButton;
    public Button towerLimitUpgradeButton;
    public Button cardGachaButton;
    public Button openSpecificCardShopButton;
    public Button backToMainShopButton;

    [Header("Text")]
    public TMP_Text towerLimitUpgradeText;
    public TMP_Text cardGachaText;
    public TMP_Text shopMessageText;

    [Header("Tower Limit Upgrade")]
    public int towerLimitIncreaseAmount = 1;
    public int baseTowerLimitUpgradeCost = 150;
    public int towerLimitUpgradeCostIncrease = 100;
    public int maxTowerLimit = 10;

    [Header("Card Gacha")]
    public int cardGachaCost = 75;
    public List<CardData> gachaCards = new List<CardData>();

    [Header("Specific Card Shop")]
    public List<CardData> specificShopCards = new List<CardData>();

    [Header("Message Settings")]
    public float messageDisplayTime = 2.5f;

    private readonly List<CardShopItemUI> spawnedCardShopItems = new List<CardShopItemUI>();
    private int towerLimitUpgradePurchaseCount = 0;
    private Coroutine messageCoroutine;

    void Start()
    {
        HookButtons();
        SubscribeToCurrencyChanges();
        BuildSpecificCardShop();
        CloseShop();
        RefreshShopUI();
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCashChanged -= RefreshShopUI;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopPanel == null)
                return;

            if (shopPanel.activeSelf)
            {
                CloseShop();
            }
        }
    }

    void HookButtons()
    {
        if (toggleShopButton != null)
            toggleShopButton.onClick.AddListener(ToggleShop);

        if (towerLimitUpgradeButton != null)
            towerLimitUpgradeButton.onClick.AddListener(BuyTowerLimitUpgrade);

        if (cardGachaButton != null)
            cardGachaButton.onClick.AddListener(BuyRandomGachaCard);

        if (openSpecificCardShopButton != null)
            openSpecificCardShopButton.onClick.AddListener(ShowSpecificCardShop);

        if (backToMainShopButton != null)
            backToMainShopButton.onClick.AddListener(ShowMainShop);
    }

    void SubscribeToCurrencyChanges()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCashChanged += RefreshShopUI;
        }
    }

    public void ToggleShop()
    {
        if (shopPanel == null)
            return;

        if (shopPanel.activeSelf)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

        ShowMainShop();
        RefreshShopUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        ClearMessage();
    }

    public void ShowMainShop()
    {
        if (mainShopPanel != null)
            mainShopPanel.SetActive(true);

        if (specificCardShopPanel != null)
            specificCardShopPanel.SetActive(false);

        RefreshShopUI();
    }

    public void ShowSpecificCardShop()
    {
        if (mainShopPanel != null)
            mainShopPanel.SetActive(false);

        if (specificCardShopPanel != null)
            specificCardShopPanel.SetActive(true);

        RefreshShopUI();
    }

    public void BuyTowerLimitUpgrade()
    {
        if (towerPlacementManager == null)
        {
            ShowMessage("No tower placement manager assigned!");
            return;
        }

        if (IsTowerLimitMaxed())
        {
            ShowMessage("Tower limit already maxed!");
            RefreshShopUI();
            return;
        }

        int cost = GetCurrentTowerLimitUpgradeCost();

        if (!TrySpend(cost))
        {
            ShowMessage("Not enough cash!");
            return;
        }

        towerPlacementManager.IncreaseTowerLimit(towerLimitIncreaseAmount);
        towerLimitUpgradePurchaseCount++;

        ShowMessage("Tower limit upgraded!");
        RefreshShopUI();
    }

    public void BuyRandomGachaCard()
    {
        if (cardHandUI == null)
        {
            ShowMessage("No card hand assigned!");
            return;
        }

        CardData rolledCard = RollRandomGachaCard();

        if (rolledCard == null)
        {
            ShowMessage("No valid gacha cards available!");
            return;
        }

        if (!TrySpend(cardGachaCost))
        {
            ShowMessage("Not enough cash!");
            return;
        }

        cardHandUI.AddCardToHand(rolledCard);
        ShowMessage("Rolled: " + GetCardDisplayName(rolledCard));
        RefreshShopUI();
    }

    public void BuySpecificCard(CardData cardData)
    {
        if (cardData == null)
            return;

        if (cardHandUI == null)
        {
            ShowMessage("No card hand assigned!");
            return;
        }

        int price = Mathf.Max(0, cardData.shopPrice);

        if (!TrySpend(price))
        {
            ShowMessage("Not enough cash!");
            return;
        }

        cardHandUI.AddCardToHand(cardData);
        ShowMessage("Bought: " + GetCardDisplayName(cardData));
        RefreshShopUI();
    }

    bool TrySpend(int amount)
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning("No CurrencyManager found in scene!");
            return false;
        }

        return CurrencyManager.Instance.SpendCash(amount);
    }

    CardData RollRandomGachaCard()
    {
        float totalWeight = GetTotalGachaWeight();

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);
        float runningWeight = 0f;

        foreach (CardData cardData in gachaCards)
        {
            if (!IsValidGachaCard(cardData))
                continue;

            runningWeight += cardData.gachaWeight;

            if (roll <= runningWeight)
                return cardData;
        }

        // Floating point safety fallback.
        for (int i = gachaCards.Count - 1; i >= 0; i--)
        {
            if (IsValidGachaCard(gachaCards[i]))
                return gachaCards[i];
        }

        return null;
    }

    float GetTotalGachaWeight()
    {
        float totalWeight = 0f;

        foreach (CardData cardData in gachaCards)
        {
            if (IsValidGachaCard(cardData))
            {
                totalWeight += cardData.gachaWeight;
            }
        }

        return totalWeight;
    }

    bool IsValidGachaCard(CardData cardData)
    {
        return cardData != null &&
               cardData.canAppearInGacha &&
               cardData.gachaWeight > 0f;
    }

    void BuildSpecificCardShop()
    {
        ClearSpecificCardShop();

        if (cardShopItemsParent == null || cardShopItemPrefab == null)
            return;

        foreach (CardData cardData in specificShopCards)
        {
            if (cardData == null || !cardData.canAppearInSpecificShop)
                continue;

            CardShopItemUI item = Instantiate(cardShopItemPrefab, cardShopItemsParent);
            item.Setup(cardData, this);
            spawnedCardShopItems.Add(item);
        }
    }

    void ClearSpecificCardShop()
    {
        for (int i = spawnedCardShopItems.Count - 1; i >= 0; i--)
        {
            if (spawnedCardShopItems[i] != null)
            {
                Destroy(spawnedCardShopItems[i].gameObject);
            }
        }

        spawnedCardShopItems.Clear();
    }

    void RefreshShopUI()
    {
        RefreshTowerLimitUpgradeUI();
        RefreshGachaUI();
        RefreshSpecificCardItems();
    }

    void RefreshTowerLimitUpgradeUI()
    {
        int cost = GetCurrentTowerLimitUpgradeCost();
        bool maxed = IsTowerLimitMaxed();
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(cost);

        if (towerLimitUpgradeText != null)
        {
            if (maxed)
            {
                towerLimitUpgradeText.text = "Tower Limit: MAX";
            }
            else
            {
                towerLimitUpgradeText.text = "+" + towerLimitIncreaseAmount + " Tower Limit - $" + cost;
            }
        }

        if (towerLimitUpgradeButton != null)
        {
            towerLimitUpgradeButton.interactable = !maxed && canAfford;
        }
    }

    void RefreshGachaUI()
    {
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(cardGachaCost);
        bool hasValidGachaPool = GetTotalGachaWeight() > 0f;

        if (cardGachaText != null)
        {
            cardGachaText.text = "Card Gacha - $" + cardGachaCost;
        }

        if (cardGachaButton != null)
        {
            cardGachaButton.interactable = canAfford && hasValidGachaPool;
        }
    }

    void RefreshSpecificCardItems()
    {
        foreach (CardShopItemUI item in spawnedCardShopItems)
        {
            if (item != null)
            {
                item.RefreshAffordability();
            }
        }
    }

    int GetCurrentTowerLimitUpgradeCost()
    {
        return baseTowerLimitUpgradeCost +
               (towerLimitUpgradePurchaseCount * towerLimitUpgradeCostIncrease);
    }

    bool IsTowerLimitMaxed()
    {
        if (towerPlacementManager == null)
            return false;

        if (maxTowerLimit <= 0)
            return false;

        return towerPlacementManager.GetTowerLimit() >= maxTowerLimit;
    }

    string GetCardDisplayName(CardData cardData)
    {
        if (cardData == null)
            return "Unknown Card";

        if (!string.IsNullOrEmpty(cardData.cardName))
            return cardData.cardName;

        return cardData.name;
    }

    void ShowMessage(string message)
    {
        if (shopMessageText == null)
        {
            Debug.Log(message);
            return;
        }

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        shopMessageText.text = message;
        Debug.Log(message);

        messageCoroutine = StartCoroutine(ClearMessageAfterDelay());
    }

    IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        ClearMessage();
    }

    void ClearMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        if (shopMessageText != null)
        {
            shopMessageText.text = "";
        }
    }
}
