using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }
    
    public Suit suit;
    public Rank rank;
    public Sprite cardSprite;
    
    public Card(Suit suit, Rank rank, Sprite sprite)
    {
        this.suit = suit;
        this.rank = rank;
        this.cardSprite = sprite;
    }
    
    public string GetCardName()
    {
        return $"{rank} of {suit}";
    }
}