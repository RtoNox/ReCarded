using System.Collections.Generic;
using UnityEngine;

public class CardHandUI : MonoBehaviour
{
    [Header("Hand References")]
    public Transform handContainer;
    public CardUI cardPrefab;
    public CardPreviewUI cardPreviewUI;
    public CardPlayManager cardPlayManager;

    [Header("Random Starting Hand")]
    [Min(0)]
    [Tooltip("The exact total card value the starting hand must reach for this level.")]
    public int maxStartingHandValue = 10;

    [Tooltip("All CardData assets that can be drawn in a starting hand. Duplicate draws are allowed.")]
    public List<CardData> availableCards = new List<CardData>();

    private readonly List<CardUI> cardsInHand = new List<CardUI>();

    void Start()
    {
        CreateStartingHand();
    }

    void CreateStartingHand()
    {
        if (maxStartingHandValue < 0)
        {
            Debug.LogError("CardHandUI: Starting-hand value cannot be negative.", this);
            return;
        }

        if (maxStartingHandValue == 0)
        {
            ClearHand();
            return;
        }

        // Validate the pool before changing the visible hand. A value-1 card guarantees
        // that every positive budget can be completed exactly.
        bool hasValueOneCard = false;
        foreach (CardData card in availableCards)
        {
            if (card == null)
                continue;

            if (card.cardValue < 1)
            {
                Debug.LogError($"CardHandUI: Card '{card.cardName}' has cardValue below 1. Every card must have a positive value.", card);
                return;
            }

            if (card.cardValue == 1)
                hasValueOneCard = true;
        }

        if (!hasValueOneCard)
        {
            Debug.LogError("CardHandUI: Add at least one card with cardValue = 1 to Available Cards so the starting hand can always reach its exact value.", this);
            return;
        }

        List<CardData> generatedHand = new List<CardData>();
        int remainingValue = maxStartingHandValue;

        while (remainingValue > 0)
        {
            List<CardData> eligibleCards = new List<CardData>();

            foreach (CardData card in availableCards)
            {
                if (card != null && card.cardValue <= remainingValue)
                    eligibleCards.Add(card);
            }

            // The value-1 card should make this unreachable after validation, but
            // retain a guard so a bad configuration can never cause an infinite loop.
            if (eligibleCards.Count == 0)
            {
                Debug.LogError($"CardHandUI: No card can fill the remaining value of {remainingValue}. Check the Available Cards list.", this);
                return;
            }

            CardData chosenCard = eligibleCards[Random.Range(0, eligibleCards.Count)];
            generatedHand.Add(chosenCard);
            remainingValue -= chosenCard.cardValue;
        }

        ClearHand();

        foreach (CardData card in generatedHand)
            AddCardToHand(card);

        Debug.Log($"CardHandUI: Generated starting hand worth exactly {maxStartingHandValue} from {generatedHand.Count} cards.", this);
    }

    public void AddCardToHand(CardData cardData)
    {
        if (cardData == null)
            return;

        if (handContainer == null)
        {
            Debug.LogWarning("No hand container assigned!");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogWarning("No card prefab assigned!");
            return;
        }

        CardUI newCard = Instantiate(cardPrefab, handContainer);

        newCard.Setup(
            cardData,
            cardPreviewUI,
            cardPlayManager
        );

        cardsInHand.Add(newCard);
    }

    public void RemoveCardFromHand(CardUI cardUI)
    {
        if (cardUI == null)
            return;

        if (cardsInHand.Contains(cardUI))
        {
            cardsInHand.Remove(cardUI);
        }

        Destroy(cardUI.gameObject);
    }

    public void ClearHand()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            if (cardsInHand[i] != null)
            {
                Destroy(cardsInHand[i].gameObject);
            }
        }

        cardsInHand.Clear();
    }
}
