using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private Sprite cardBackSprite;
    
    void Start()
    {
        InitializeDeck();
        ShuffleDeck();
    }
    
    void InitializeDeck()
    {
        cards.Clear();
        Debug.Log("Initializing deck...");
        
        Sprite[] allSprites = Resources.LoadAll<Sprite>("ForestCards");
        Debug.Log("Found " + allSprites.Length + " sprites");
        
        for (int i = 0; i < 52 && i < allSprites.Length; i++)
        {
            Sprite cardSprite = allSprites[i];
            
            int suit = i / 13;
            int rank = (i % 13) + 1;
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite);
            cards.Add(newCard);
        }
        
        Debug.Log("Deck initialized with " + cards.Count + " cards");
    }
    
    public void ShuffleDeck()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
    
    public Card DrawCard()
    {
        if (cards.Count > 0)
        {
            Card drawnCard = cards[0];
            cards.RemoveAt(0);
            return drawnCard;
        }
        return null;
    }
    
    public int GetRemainingCards()
    {
        return cards.Count;
    }
}