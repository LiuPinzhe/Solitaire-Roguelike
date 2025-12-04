using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private Sprite cardBackSprite;
    
    void Start()
    {
        Debug.Log("Deck Start called");
        InitializeDeck();
        ShuffleDeck();
        Debug.Log($"Deck initialized with {cards.Count} cards");
    }
    
    void InitializeDeck()
    {
        cards.Clear();
        Debug.Log("Initializing deck...");
        
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Classic/ClassicCards");
        Debug.Log("Found " + allSprites.Length + " sprites in Classic folder");
        
        // 显示前几个sprite的名称
        for (int j = 0; j < Mathf.Min(8, allSprites.Length); j++)
        {
            Debug.Log($"Sprite {j}: {allSprites[j].name}");
        }
        
        if (allSprites.Length == 0)
        {
            Debug.LogError("No sprites found in Resources/ForestCards! Make sure the sprites are in the correct folder.");
            // 创建测试卡牌
            CreateTestCards();
            return;
        }
        
        // 按sprite名称排序确保正确顺序
        System.Array.Sort(allSprites, (x, y) => {
            int xNum = int.Parse(x.name.Replace("ClassicCards_", ""));
            int yNum = int.Parse(y.name.Replace("ClassicCards_", ""));
            return xNum.CompareTo(yNum);
        });
        
        for (int i = 0; i < 52 && i < allSprites.Length; i++)
        {
            Sprite cardSprite = allSprites[i];
            
            // 根据sprite名称中的数字确定卡牌
            string spriteName = cardSprite.name;
            int spriteIndex = int.Parse(spriteName.Replace("ClassicCards_", ""));
            
            // 特殊排列：0-25是方片和草花，26-51是黑桃和红心
            int rank = (spriteIndex / 2) % 13 + 1;  // 点数：每2张一个点数
            int suit;
            
            if (spriteIndex < 26) {
                // 0-25: 方片和草花
                suit = (spriteIndex % 2 == 0) ? 1 : 2; // 0=方片(Diamonds), 1=草花(Clubs)
            } else {
                // 26-51: 黑桃和红心
                suit = ((spriteIndex - 26) % 2 == 0) ? 3 : 0; // 0=黑桃(Spades), 1=红心(Hearts)
            }
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite);
            cards.Add(newCard);
            
            // 特别检查几个关键的sprite
            if (i < 16 || spriteIndex == 44 || spriteIndex == 40 || spriteIndex == 36)
                Debug.Log($"Sprite {spriteIndex} ({cardSprite.name}) -> {newCard.GetCardName()} (rank={rank}, suit={suit})");
        }
        
        Debug.Log("Deck initialized with " + cards.Count + " cards");
    }
    
    void CreateTestCards()
    {
        Debug.Log("Creating test cards without sprites...");
        
        for (int suit = 0; suit < 4; suit++)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, null);
                cards.Add(newCard);
            }
        }
        
        Debug.Log($"Created {cards.Count} test cards");
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