using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private List<Card> allCards = new List<Card>(); // Master list of all cards
    [SerializeField] private Sprite cardBackSprite;
    
    void Start()
    {
        InitializeDeck();
        ShuffleDeck();
    }
    
    public void InitializeDeck()
    {
        cards.Clear();
        
        LoadClassicCards();
        LoadForestCards();
        // LoadSpaceCards();
        LoadFireCards();
    }
    
    void LoadClassicCards()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Classic/ClassicCards");
        
        if (allSprites.Length == 0)
        {
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
            
            // 特殊排列：0-25是红心黑桃，26-51是草花方片
            int rank = (spriteIndex / 2) % 13 + 1;
            int suit;
            
            if (spriteIndex < 26) {
                suit = (spriteIndex % 2 == 0) ? 0 : 3; // 0=红心, 3=黑桃
            } else {
                suit = ((spriteIndex - 26) % 2 == 0) ? 2 : 1; // 2=草花, 1=方片
            }
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite, "Classic");
            cards.Add(newCard);
            allCards.Add(newCard);
        }
    }
    
    void LoadForestCards()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Forest/ForestCards");
        Debug.Log("Found " + allSprites.Length + " sprites in Forest folder");
        
        if (allSprites.Length == 0)
        {
            Debug.LogError("No sprites found in Resources/Forest/ForestCards!");
            CreateTestCards();
            return;
        }
        
        // 按sprite名称排序确保正确顺序
        System.Array.Sort(allSprites, (x, y) => {
            int xNum = int.Parse(x.name.Replace("ForestCards_", ""));
            int yNum = int.Parse(y.name.Replace("ForestCards_", ""));
            return xNum.CompareTo(yNum);
        });
        
        for (int i = 0; i < 52 && i < allSprites.Length; i++)
        {
            Sprite cardSprite = allSprites[i];
            
            // 根据sprite名称中的数字确定卡牌
            string spriteName = cardSprite.name;
            int spriteIndex = int.Parse(spriteName.Replace("ForestCards_", ""));
            
            // 使用相同的映射逻辑
            int rank = (spriteIndex / 2) % 13 + 1;
            int suit;
            
            if (spriteIndex < 26) {
                suit = (spriteIndex % 2 == 0) ? 0 : 3; // 0=红心, 3=黑桃
            } else {
                suit = ((spriteIndex - 26) % 2 == 0) ? 2 : 1; // 2=草花, 1=方片
            }
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite, "Forest/Backsides/Classic");
            cards.Add(newCard);
        }
    }
    
    void LoadSpaceCards()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Space/SpaceCards");
        Debug.Log("Found " + allSprites.Length + " sprites in Space folder");
        
        if (allSprites.Length == 0)
        {
            Debug.LogError("No sprites found in Resources/Space/SpaceCards!");
            return;
        }
        
        // 按sprite名称排序确保正确顺序
        System.Array.Sort(allSprites, (x, y) => {
            int xNum = int.Parse(x.name.Replace("SpaceCards_", ""));
            int yNum = int.Parse(y.name.Replace("SpaceCards_", ""));
            return xNum.CompareTo(yNum);
        });
        
        for (int i = 0; i < 52 && i < allSprites.Length; i++)
        {
            Sprite cardSprite = allSprites[i];
            
            // 根据sprite名称中的数字确定卡牌
            string spriteName = cardSprite.name;
            int spriteIndex = int.Parse(spriteName.Replace("SpaceCards_", ""));
            
            // 使用相同的映射逻辑
            int rank = (spriteIndex / 2) % 13 + 1;
            int suit;
            
            if (spriteIndex < 26) {
                suit = (spriteIndex % 2 == 0) ? 0 : 3; // 0=红心, 3=黑桃
            } else {
                suit = ((spriteIndex - 26) % 2 == 0) ? 2 : 1; // 2=草花, 1=方片
            }
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite, "Space/Backsides/Classic");
            cards.Add(newCard);
        }
    }
    
    void LoadFireCards()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Fire/FireCards");
        Debug.Log("Found " + allSprites.Length + " sprites in Fire folder");
        
        if (allSprites.Length == 0)
        {
            Debug.LogError("No sprites found in Resources/Fire/FireCards!");
            return;
        }
        
        // 按sprite名称排序确保正确顺序
        System.Array.Sort(allSprites, (x, y) => {
            int xNum = int.Parse(x.name.Replace("FireCards_", ""));
            int yNum = int.Parse(y.name.Replace("FireCards_", ""));
            return xNum.CompareTo(yNum);
        });
        
        for (int i = 0; i < 52 && i < allSprites.Length; i++)
        {
            Sprite cardSprite = allSprites[i];
            
            // 根据sprite名称中的数字确定卡牌
            string spriteName = cardSprite.name;
            int spriteIndex = int.Parse(spriteName.Replace("FireCards_", ""));
            
            // 使用相同的映射逻辑
            int rank = (spriteIndex / 2) % 13 + 1;
            int suit;
            
            if (spriteIndex < 26) {
                suit = (spriteIndex % 2 == 0) ? 0 : 3; // 0=红心, 3=黑桃
            } else {
                suit = ((spriteIndex - 26) % 2 == 0) ? 2 : 1; // 2=草花, 1=方片
            }
            
            Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, cardSprite, "Fire/Backsides/Classic");
            cards.Add(newCard);
        }
    }
    
    void CreateTestCards()
    {
        for (int suit = 0; suit < 4; suit++)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank, null);
                cards.Add(newCard);
                allCards.Add(newCard);
            }
        }
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
    
    public void AddCard(Card card)
    {
        allCards.Add(card);
    }
    
    public void ResetDeckForNewGame()
    {
        cards.Clear();
        cards.AddRange(allCards);
    }
}