using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SolitaireGameManager : MonoBehaviour
{
    [Header("Game Areas")]
    [SerializeField] private Transform[] tableauColumns = new Transform[7];
    [SerializeField] private Transform[] foundationPiles = new Transform[4];
    [SerializeField] private Transform stockPile;
    [SerializeField] private Transform wastePile;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cardPrefab;
    
    private Deck deck;
    private List<List<CardDisplay>> tableau = new List<List<CardDisplay>>();
    private List<List<CardDisplay>> foundations = new List<List<CardDisplay>>();
    private List<CardDisplay> stock = new List<CardDisplay>();
    private List<CardDisplay> waste = new List<CardDisplay>();
    
    void Start()
    {
        Debug.Log("SolitaireGameManager Start called");
        deck = FindFirstObjectByType<Deck>();
        if (deck == null)
        {
            Debug.LogError("Deck not found! Make sure there's a Deck component in the scene.");
            return;
        }
        Debug.Log("Deck found, starting game automatically");
        
        // 等待一帧让Deck初始化完成
        Invoke("InitializeGame", 0.1f);
    }
    
    void InitializeGame()
    {
        Debug.Log("InitializeGame called");
        
        // 检查必要组件
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is null!");
            return;
        }
        
        if (tableauColumns == null || tableauColumns.Length != 7)
        {
            Debug.LogError("Tableau columns not properly set!");
            return;
        }
        
        // 初始化数据结构
        tableau.Clear();
        foundations.Clear();
        for (int i = 0; i < 7; i++)
            tableau.Add(new List<CardDisplay>());
        for (int i = 0; i < 4; i++)
            foundations.Add(new List<CardDisplay>());
        
        Debug.Log("Starting to deal cards to tableau");
        
        // 发牌到7列
        for (int col = 0; col < 7; col++)
        {
            if (tableauColumns[col] == null)
            {
                Debug.LogError($"Tableau column {col} is null!");
                continue;
            }
            
            Debug.Log($"Dealing to column {col}, cards needed: {col + 1}");
            
            for (int row = 0; row <= col; row++)
            {
                Card card = deck.DrawCard();
                if (card != null)
                {
                    GameObject cardObj = Instantiate(cardPrefab, tableauColumns[col]);
                    CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
                    if (cardDisplay == null)
                    {
                        Debug.LogError("CardDisplay component not found on card prefab!");
                        continue;
                    }
                    
                    // 设置卡牌位置 - 从顶部开始垂直叠放
                    RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                    RectTransform columnRect = tableauColumns[col].GetComponent<RectTransform>();
                    
                    float cardHeight = 30f; // 卡牌间距
                    
                    // 设置锚点为顶部中心
                    cardRect.anchorMin = new Vector2(0.5f, 1f);
                    cardRect.anchorMax = new Vector2(0.5f, 1f);
                    cardRect.pivot = new Vector2(0.5f, 1f);
                    
                    // 从顶部开始向下排列
                    cardRect.anchoredPosition = new Vector2(0, -row * cardHeight);
                    
                    cardDisplay.SetCard(card);
                    
                    // 只有最后一张牌翻开
                    if (row == col)
                    {
                        cardDisplay.RevealCard();
                        Debug.Log($"Revealed card: {card.GetCardName()} in column {col}");
                    }
                    else
                    {
                        cardDisplay.HideCard();
                    }
                    
                    tableau[col].Add(cardDisplay);
                }
                else
                {
                    Debug.LogWarning($"No card available for column {col}, row {row}");
                }
            }
        }
        
        Debug.Log($"Remaining cards in deck: {deck.GetRemainingCards()}");
        
        // 剩余牌放入库存堆
        if (stockPile != null)
        {
            while (deck.GetRemainingCards() > 0)
            {
                Card card = deck.DrawCard();
                if (card != null)
                {
                    GameObject cardObj = Instantiate(cardPrefab, stockPile);
                    CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
                    
                    // 库存堆中的牌重叠在一起
                    RectTransform stockCardRect = cardObj.GetComponent<RectTransform>();
                    stockCardRect.anchoredPosition = Vector2.zero;
                    
                    cardDisplay.SetCard(card);
                    cardDisplay.HideCard();
                    stock.Add(cardDisplay);
                }
            }
            Debug.Log($"Added {stock.Count} cards to stock pile");
        }
        else
        {
            Debug.LogError("Stock pile is null!");
        }
        
        Debug.Log("Game initialization complete");
    }
    
    public int GetTableauCardCount(int columnIndex)
    {
        if (columnIndex >= 0 && columnIndex < tableau.Count)
            return tableau[columnIndex].Count;
        return 0;
    }
    
    public List<CardDisplay> GetTableauColumn(int columnIndex)
    {
        if (columnIndex >= 0 && columnIndex < tableau.Count)
            return tableau[columnIndex];
        return new List<CardDisplay>();
    }
    
    public void DrawFromStock()
    {
        Debug.Log($"DrawFromStock called. Stock has {stock.Count} cards");
        
        // 检查是否所有列都有牌
        for (int i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].Count == 0)
            {
                Debug.LogWarning($"Column {i} is empty! Cannot deal from stock.");
                return;
            }
        }
        
        // 检查stock是否有足够的牌（至少10张）
        if (stock.Count < 10)
        {
            Debug.LogWarning($"Not enough cards in stock! Only {stock.Count} cards remaining.");
            return;
        }
        
        // 给每列发一张牌
        for (int col = 0; col < 7; col++)
        {
            if (stock.Count > 0)
            {
                CardDisplay card = stock[stock.Count - 1];
                stock.RemoveAt(stock.Count - 1);
                
                // 移动到对应列的顶部
                card.transform.SetParent(tableauColumns[col], false);
                
                // 设置位置
                RectTransform cardRect = card.GetComponent<RectTransform>();
                float cardHeight = 30f;
                int cardCount = tableau[col].Count;
                
                cardRect.anchorMin = new Vector2(0.5f, 1f);
                cardRect.anchorMax = new Vector2(0.5f, 1f);
                cardRect.pivot = new Vector2(0.5f, 1f);
                cardRect.anchoredPosition = new Vector2(0, -cardCount * cardHeight);
                
                // 翻开卡牌并添加到列中
                card.RevealCard();
                tableau[col].Add(card);
                
                Debug.Log($"Dealt {card.GetCard().GetCardName()} to column {col}");
            }
        }
        
        Debug.Log($"Stock now has {stock.Count} cards remaining");
    }
    
    public void MoveCardSequence(List<CardDisplay> sequence, int targetColumnIndex)
    {
        if (sequence.Count == 0) return;
        
        Debug.Log($"Moving sequence of {sequence.Count} cards to column {targetColumnIndex}");
        
        // 获取目标列的当前卡牌数量（在移除之前）
        int startPosition = tableau[targetColumnIndex].Count;
        
        // 从原位置移除所有牌
        foreach (CardDisplay card in sequence)
        {
            RemoveCardFromCurrentLocation(card);
        }
        
        // 添加到目标列并设置位置
        float cardHeight = 30f;
        
        for (int i = 0; i < sequence.Count; i++)
        {
            CardDisplay card = sequence[i];
            
            // 添加到列数据先，再设置位置
            tableau[targetColumnIndex].Add(card);
            
            // 设置父对象
            card.transform.SetParent(tableauColumns[targetColumnIndex], false);
            
            // 计算正确的位置（基于当前列的总数）
            int currentRow = startPosition + i;
            
            // 设置位置
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 1f);
            cardRect.anchorMax = new Vector2(0.5f, 1f);
            cardRect.pivot = new Vector2(0.5f, 1f);
            cardRect.anchoredPosition = new Vector2(0, -currentRow * cardHeight);
            
            Debug.Log($"Positioned {card.GetCard().GetCardName()} at row {currentRow}, position Y: {-currentRow * cardHeight}");
        }
        
        // 检查是否需要翻开新的顶牌
        CheckAndRevealTopCards();
    }
    
    void CheckAndRevealTopCards()
    {
        for (int i = 0; i < tableau.Count; i++)
        {
            CheckAndRevealTopCard(i);
        }
    }
    
    public bool CanMoveToFoundation(CardDisplay cardDisplay, int foundationIndex)
    {
        Card card = cardDisplay.GetCard();
        List<CardDisplay> foundation = foundations[foundationIndex];
        
        if (foundation.Count == 0)
            return card.rank == Card.Rank.Ace;
        
        Card topCard = foundation[foundation.Count - 1].GetCard();
        return card.suit == topCard.suit && (int)card.rank == (int)topCard.rank + 1;
    }
    
    public bool CanMoveToTableau(CardDisplay cardDisplay, int columnIndex)
    {
        Card card = cardDisplay.GetCard();
        List<CardDisplay> column = tableau[columnIndex];
        
        if (column.Count == 0)
        {
            Debug.Log($"Empty column - can place K: {card.rank == Card.Rank.King}");
            return card.rank == Card.Rank.King;
        }
        
        Card topCard = column[column.Count - 1].GetCard();
        bool correctRank = (int)card.rank == (int)topCard.rank - 1;
        
        Debug.Log($"Trying to move {card.GetCardName()} to {topCard.GetCardName()}: correctRank={correctRank}");
        
        return correctRank;
    }
    
    public bool IsGameWon()
    {
        return foundations.All(foundation => foundation.Count == 13);
    }
    
    public void MoveCard(CardDisplay cardDisplay, DropZone.ZoneType zoneType, int zoneIndex)
    {
        // 从原位置移除
        RemoveCardFromCurrentLocation(cardDisplay);
        
        // 添加到新位置
        switch (zoneType)
        {
            case DropZone.ZoneType.Foundation:
                foundations[zoneIndex].Add(cardDisplay);
                break;
            case DropZone.ZoneType.Tableau:
                tableau[zoneIndex].Add(cardDisplay);
                CheckAndRevealTopCard(zoneIndex);
                break;
        }
    }
    
    private void RemoveCardFromCurrentLocation(CardDisplay cardDisplay)
    {
        // 从tableau中移除
        for (int i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].Remove(cardDisplay))
            {
                Debug.Log($"Removed {cardDisplay.GetCard().GetCardName()} from column {i}");
                // 注意：不要在这里立即检查翻牌，等所有牌移动完成后再统一检查
                return;
            }
        }
        
        // 从foundations中移除
        for (int i = 0; i < foundations.Count; i++)
        {
            if (foundations[i].Remove(cardDisplay))
                return;
        }
        
        // 从waste中移除
        waste.Remove(cardDisplay);
    }
    
    private void CheckAndRevealTopCard(int columnIndex)
    {
        if (tableau[columnIndex].Count > 0)
        {
            CardDisplay topCard = tableau[columnIndex][tableau[columnIndex].Count - 1];
            topCard.RevealCard();
        }
    }
}