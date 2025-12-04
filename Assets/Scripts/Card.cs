using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }
    
    public Suit suit;
    public Rank rank;
    public Sprite cardSprite;
    public string set;
    
    public Card(Suit suit, Rank rank, Sprite sprite, string cardSet = "Classic")
    {
        this.suit = suit;
        this.rank = rank;
        this.cardSprite = sprite;
        this.set = cardSet;
    }
    
    public string GetCardName()
    {
        return $"{rank} of {suit}";
    }
    
    public bool IsRed()
    {
        return suit == Suit.Hearts || suit == Suit.Diamonds;
    }
    
    public bool IsBlack()
    {
        return suit == Suit.Clubs || suit == Suit.Spades;
    }
}