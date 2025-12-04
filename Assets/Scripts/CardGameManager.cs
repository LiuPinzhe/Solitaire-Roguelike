using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardGameManager : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] private Deck deck;
    [SerializeField] private Transform handArea;
    [SerializeField] private GameObject cardPrefab;
    
    [Header("UI Elements")]
    [SerializeField] private Button drawButton;
    [SerializeField] private Text deckCountText;
    [SerializeField] private Text handCountText;
    
    private List<CardDisplay> playerHand = new List<CardDisplay>();
    
    void Start()
    {
        if (drawButton != null)
        {
            drawButton.onClick.AddListener(() => {
                Debug.Log("Button clicked!");
                DrawCardToHand();
            });
            Debug.Log("Draw button connected successfully");
        }
        else
        {
            Debug.LogError("Draw button is not assigned!");
        }
        
        UpdateUI();
    }
    
    public void TestButtonClick()
    {
        Debug.Log("Button clicked - test method");
    }
    
    public void DrawCardToHand()
    {
        Debug.Log("DrawCardToHand called");
        
        if (deck == null)
        {
            Debug.LogError("Deck is null!");
            return;
        }
        
        Card drawnCard = deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.LogWarning("No more cards in deck!");
            return;
        }
        
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is null!");
            return;
        }
        
        if (handArea == null)
        {
            Debug.LogError("Hand area is null!");
            return;
        }
        
        GameObject cardObj = Instantiate(cardPrefab, handArea);
        CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
        if (cardDisplay == null)
        {
            cardDisplay = cardObj.AddComponent<CardDisplay>();
        }
        cardDisplay.SetCard(drawnCard);
        playerHand.Add(cardDisplay);
        
        UpdateUI();
        Debug.Log("Card drawn: " + drawnCard.GetCardName());
    }
    
    public void ShuffleDeck()
    {
        if (deck != null)
        {
            deck.ShuffleDeck();
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        if (deckCountText != null && deck != null)
            deckCountText.text = "牌组: " + deck.GetRemainingCards();
        
        if (handCountText != null)
            handCountText.text = "手牌: " + playerHand.Count;
    }
}