using System.Collections.Generic;
using UnityEngine;

public class CardHandUI : MonoBehaviour
{
    [Header("Hand References")]
    public Transform handContainer;
    public CardUI cardPrefab;
    public CardPreviewUI cardPreviewUI;

    [Header("Starting Cards")]
    public List<CardData> startingCards = new List<CardData>();

    private List<CardUI> cardsInHand = new List<CardUI>();

    void Start()
    {
        CreateStartingHand();
    }

    void CreateStartingHand()
    {
        ClearHand();

        for (int i = 0; i < startingCards.Count; i++)
        {
            AddCardToHand(startingCards[i]);
        }
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
        newCard.Setup(cardData, cardPreviewUI);

        cardsInHand.Add(newCard);
    }

    public void RemoveCardFromHand(CardUI cardUI)
    {
        if (cardUI == null)
            return;

        if (cardsInHand.Contains(cardUI))
            cardsInHand.Remove(cardUI);

        Destroy(cardUI.gameObject);
    }

    public void ClearHand()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            if (cardsInHand[i] != null)
                Destroy(cardsInHand[i].gameObject);
        }

        cardsInHand.Clear();
    }
}