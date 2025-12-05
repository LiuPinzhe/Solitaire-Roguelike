using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.Collections;

public class SolitaireGameManager : MonoBehaviour
{
    [Header("Game Areas")]
    [SerializeField] private Transform[] tableauColumns = new Transform[7];
    [SerializeField] private Transform[] foundationPiles = new Transform[4];
    [SerializeField] private Transform stockPile;
    [SerializeField] private Transform wastePile;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject batPrefab;
    [SerializeField] private GameObject jokerPrefab;
    [SerializeField] private GameObject joker2Prefab;
    
    [Header("Enemy System")]
    [SerializeField] private Transform enemySpawnPoint;
    
    [Header("Hand Area")]
    [SerializeField] private Transform handArea;
    
    private Deck deck;
    private List<List<CardDisplay>> tableau = new List<List<CardDisplay>>();
    private List<List<CardDisplay>> foundations = new List<List<CardDisplay>>();
    private List<CardDisplay> stock = new List<CardDisplay>();
    private List<CardDisplay> waste = new List<CardDisplay>();
    private bool cardSelectionMode = false;
    private bool cardSwapMode = false;
    private CardDisplay firstSelectedCard = null;
    private bool isDealing = false;
    
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
                    
                    // 设置卡牌位置和尺寸
                    RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                    
                    // 设置tableau中的卡牌尺寸为46*70
                    cardRect.sizeDelta = new Vector2(46f, 70f);
                    
                    float cardHeight = 30f; // 卡牌间距
                    
                    // 设置锚点为顶部中心
                    cardRect.anchorMin = new Vector2(0.5f, 1f);
                    cardRect.anchorMax = new Vector2(0.5f, 1f);
                    cardRect.pivot = new Vector2(0.5f, 1f);
                    
                    // 从顶部开始向下排列
                    cardRect.anchoredPosition = new Vector2(0, -row * cardHeight);
                    
                    // 根据卡牌set选择卡背图片
                    string backSpritePath = card.set == "Forest/Backsides/Classic" ? "Forest/Backsides/Classic" : "Classic/Backsides/LightClassic";
                    Sprite backSprite = Resources.Load<Sprite>(backSpritePath);
                    cardDisplay.SetCard(card, backSprite);
                    
                    // 只有最后一张牌翻开
                    if (row == col)
                    {
                        cardDisplay.RevealCard();
                        Debug.Log($"Revealed card: {card.GetCardName()} in column {col}");
                    }
                    else
                    {
                        cardDisplay.HideCard();
                        Debug.Log($"Hidden card in column {col}, row {row}");
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
                    
                    // 设置stock堆中的卡牌尺寸为46*70
                    stockCardRect.sizeDelta = new Vector2(46f, 70f);
                    
                    stockCardRect.anchoredPosition = Vector2.zero;
                    
                    // 根据卡牌set选择卡背图片
                    string backSpritePath = card.set == "Forest/Backsides/Classic" ? "Forest/Backsides/Classic" : "Classic/Backsides/LightClassic";
                    Sprite backSprite = Resources.Load<Sprite>(backSpritePath);
                    cardDisplay.SetCard(card, backSprite);
                    cardDisplay.HideCard();
                    
                    // 禁用Raycast Target避免阻挡点击
                    Image cardImage = cardDisplay.GetComponent<Image>();
                    if (cardImage != null)
                    {
                        cardImage.raycastTarget = false;
                    }
                    
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
        
        // 初始化HandArea
        InitializeHandArea();
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
        if (isDealing)
        {
            return; // 正在发牌，忽略点击
        }
        
        if (stock.Count < 7)
        {
            Debug.LogWarning($"Not enough cards in stock! Only {stock.Count} cards remaining.");
            return;
        }
        
        StartCoroutine(AnimateCardDealing());
    }
    
    IEnumerator AnimateCardDealing()
    {
        isDealing = true;
        
        for (int col = 0; col < 7; col++)
        {
            if (stock.Count > 0)
            {
                CardDisplay card = stock[stock.Count - 1];
                stock.RemoveAt(stock.Count - 1);
                
                // 记录原始位置
                Vector3 startPos = card.transform.position;
                
                // 局部变量避免闭包问题
                int targetColumn = col;
                
                // 设置目标位置
                card.transform.SetParent(tableauColumns[targetColumn], false);
                RectTransform cardRect = card.GetComponent<RectTransform>();
                cardRect.sizeDelta = new Vector2(46f, 70f);
                
                float cardHeight = 30f;
                int cardCount = tableau[targetColumn].Count;
                
                cardRect.anchorMin = new Vector2(0.5f, 1f);
                cardRect.anchorMax = new Vector2(0.5f, 1f);
                cardRect.pivot = new Vector2(0.5f, 1f);
                
                Vector2 targetPos = new Vector2(0, -cardCount * cardHeight);
                
                // 设置初始位置为stock位置
                cardRect.position = startPos;
                
                // 动画移动到目标位置
                cardRect.DOAnchorPos(targetPos, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        card.RevealCard();
                        tableau[targetColumn].Add(card);
                        
                        // 重新启用Raycast Target
                        Image cardImage = card.GetComponent<Image>();
                        if (cardImage != null)
                        {
                            cardImage.raycastTarget = true;
                        }
                    });
                
                // 等待一小段时间再发下一张牌
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // 等待所有动画完成
        yield return new WaitForSeconds(0.3f);
        isDealing = false;
    }
    
    public void RemoveCardFromGame(CardDisplay cardDisplay)
    {
        RemoveCardFromCurrentLocation(cardDisplay);
    }
    
    public void CheckAndRevealAllTopCards()
    {
        CheckAndRevealTopCards();
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
            
            // 设置位置和尺寸
            RectTransform cardRect = card.GetComponent<RectTransform>();
            
            // 设置卡牌尺寸为46*70
            cardRect.sizeDelta = new Vector2(46f, 70f);
            
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
            Debug.Log($"Empty column - can place any card: {card.GetCardName()}");
            return true; // 任何牌都可以放在空列
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
        // 记录来源列
        int sourceColumn = -1;
        for (int i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].Contains(cardDisplay))
            {
                sourceColumn = i;
                break;
            }
        }
        
        // 从原位置移除
        RemoveCardFromCurrentLocation(cardDisplay);
        
        // 添加到新位置
        switch (zoneType)
        {
            case DropZone.ZoneType.Foundation:
                foundations[zoneIndex].Add(cardDisplay);
                // 设置Foundation中卡牌的位置
                cardDisplay.transform.SetParent(foundationPiles[zoneIndex], false);
                RectTransform cardRect = cardDisplay.GetComponent<RectTransform>();
                cardRect.anchoredPosition = Vector2.zero;
                cardRect.sizeDelta = new Vector2(46f, 70f);
                break;
            case DropZone.ZoneType.Tableau:
                tableau[zoneIndex].Add(cardDisplay);
                CheckAndRevealTopCard(zoneIndex);
                break;
        }
        
        // 检查来源列是否需要翻开新的顶牌
        if (sourceColumn >= 0)
        {
            CheckAndRevealTopCard(sourceColumn);
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
    
    public void SpawnNextEnemy()
    {
        if (batPrefab != null && enemySpawnPoint != null)
        {
            GameObject newBat = Instantiate(batPrefab, enemySpawnPoint);
            newBat.SetActive(true);
        }
    }
    
    void InitializeHandArea()
    {
        if (handArea != null)
        {
            if (jokerPrefab != null)
            {
                GameObject joker = Instantiate(jokerPrefab, handArea);
                joker.SetActive(true);
            }
            
            if (joker2Prefab != null)
            {
                GameObject joker2 = Instantiate(joker2Prefab, handArea);
                joker2.SetActive(true);
            }
        }
    }
    
    public void EnterCardSelectionMode()
    {
        Debug.Log($"[GameManager] EnterCardSelectionMode called");
        cardSelectionMode = true;
        Debug.Log($"[GameManager] Card selection mode activated");
    }
    
    public void SelectCardForRemoval(CardDisplay cardDisplay)
    {
        Debug.Log($"[GameManager] SelectCardForRemoval called, cardSelectionMode: {cardSelectionMode}");
        if (cardSelectionMode)
        {
            Debug.Log($"[GameManager] Removing card: {cardDisplay.GetCard().GetCardName()}");
            // 移除选中的卡牌
            RemoveCardFromCurrentLocation(cardDisplay);
            Destroy(cardDisplay.gameObject);
            
            // 检查并翻开新的顶牌
            CheckAndRevealTopCards();
            
            // 重置Joker状态
            Joker joker = FindObjectOfType<Joker>();
            if (joker != null)
            {
                joker.ResetJoker();
            }
            
            // 退出选择模式
            cardSelectionMode = false;
            Debug.Log($"[GameManager] Card selection mode deactivated");
        }
    }
    
    public bool IsInCardSelectionMode()
    {
        return cardSelectionMode;
    }
    
    void Update()
    {
        // 在选择模式下检测点击
        if (cardSelectionMode && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForCancelClick();
        }
        
        // 在交换模式下检测点击
        if (cardSwapMode && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForCancelSwap();
        }
    }
    
    void CheckForCancelClick()
    {
        // 检查点击是否命中了卡牌
        var raycastResults = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, raycastResults);
        
        bool hitCard = false;
        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<CardDisplay>() != null)
            {
                hitCard = true;
                break;
            }
        }
        
        // 如果没有点击到卡牌，则取消选择
        if (!hitCard)
        {
            CancelCardSelection();
        }
    }
    
    public void CancelCardSelection()
    {
        Debug.Log($"[GameManager] CancelCardSelection called");
        if (cardSelectionMode)
        {
            // 重置Joker状态
            Joker joker = FindObjectOfType<Joker>();
            if (joker != null)
            {
                joker.ResetJoker();
            }
            
            // 退出选择模式
            cardSelectionMode = false;
            Debug.Log($"[GameManager] Card selection mode canceled");
        }
    }
    
    // Joker2 交换功能
    public void EnterCardSwapMode()
    {
        cardSwapMode = true;
        firstSelectedCard = null;
    }
    
    public bool IsInCardSwapMode()
    {
        return cardSwapMode;
    }
    
    public void SelectCardForSwap(CardDisplay cardDisplay)
    {
        if (!cardSwapMode) return;
        
        if (firstSelectedCard == null)
        {
            // 选择第一张牌
            firstSelectedCard = cardDisplay;
            cardDisplay.GetComponent<Image>().color = Color.yellow; // 高亮显示
        }
        else if (firstSelectedCard == cardDisplay)
        {
            // 取消选择
            cardDisplay.GetComponent<Image>().color = Color.white;
            firstSelectedCard = null;
        }
        else
        {
            // 选择第二张牌，执行交换
            SwapCards(firstSelectedCard, cardDisplay);
            
            // 重置状态
            firstSelectedCard.GetComponent<Image>().color = Color.white;
            firstSelectedCard = null;
            
            // 重置Joker2并退出交换模式
            Joker2 joker2 = FindObjectOfType<Joker2>();
            if (joker2 != null)
            {
                joker2.ResetJoker2();
            }
            cardSwapMode = false;
        }
    }
    
    void SwapCards(CardDisplay card1, CardDisplay card2)
    {
        // 交换卡牌数据
        Card tempCard = card1.GetCard();
        Sprite tempSprite = card1.GetComponent<Image>().sprite;
        
        card1.SetCard(card2.GetCard());
        card2.SetCard(tempCard);
    }
    
    void CheckForCancelSwap()
    {
        // 检查点击是否命中了卡牌
        var raycastResults = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, raycastResults);
        
        bool hitCard = false;
        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<CardDisplay>() != null)
            {
                hitCard = true;
                break;
            }
        }
        
        if (!hitCard)
        {
            CancelCardSwap();
        }
    }
    
    public void CancelCardSwap()
    {
        if (cardSwapMode)
        {
            // 重置选中的牌
            if (firstSelectedCard != null)
            {
                firstSelectedCard.GetComponent<Image>().color = Color.white;
                firstSelectedCard = null;
            }
            
            // 重置Joker2状态
            Joker2 joker2 = FindObjectOfType<Joker2>();
            if (joker2 != null)
            {
                joker2.ResetJoker2();
            }
            
            cardSwapMode = false;
        }
    }
}