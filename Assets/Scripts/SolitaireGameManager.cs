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
    [SerializeField] private GameObject s02Prefab;
    
    [Header("Enemy System")]
    [SerializeField] private Transform enemySpawnPoint;
    
    [Header("Hand Area")]
    [SerializeField] private Transform handArea;
    
    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Button resetButton;
    
    private Deck deck;
    private List<List<CardDisplay>> tableau = new List<List<CardDisplay>>();
    private List<List<CardDisplay>> foundations = new List<List<CardDisplay>>();
    private List<CardDisplay> stock = new List<CardDisplay>();
    private List<CardDisplay> waste = new List<CardDisplay>();
    private bool isDealing = false;
    private AbilityManager abilityManager;
    
    void Start()
    {
        Debug.Log("SolitaireGameManager Start called");
        deck = FindFirstObjectByType<Deck>();
        abilityManager = FindFirstObjectByType<AbilityManager>();
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
                    string backSpritePath = "Classic/Backsides/LightClassic";
                    if (card.set == "Forest/Backsides/Classic")
                        backSpritePath = "Forest/Backsides/Classic";
                    else if (card.set == "Space/Backsides/Classic")
                        backSpritePath = "Space/Backsides/Classic";
                    else if (card.set == "Fire/Backsides/Classic")
                        backSpritePath = "Fire/Backsides/Classic";
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
                    Debug.LogError($"No card available for column {col}, row {row}. Deck has {deck.GetRemainingCards()} cards left!");
                    break;
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
                    string backSpritePath = "Classic/Backsides/LightClassic";
                    if (card.set == "Forest/Backsides/Classic")
                        backSpritePath = "Forest/Backsides/Classic";
                    else if (card.set == "Space/Backsides/Classic")
                        backSpritePath = "Space/Backsides/Classic";
                    else if (card.set == "Fire/Backsides/Classic")
                        backSpritePath = "Fire/Backsides/Classic";
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
        
        // 设置重置按钮
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
        }
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
    
    public void RemoveCardFromCurrentLocation(CardDisplay cardDisplay)
    {
        // 从 tableau中移除
        for (int i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].Remove(cardDisplay))
            {
                Debug.Log($"Removed {cardDisplay.GetCard().GetCardName()} from column {i}");
                return;
            }
        }
        
        // 从 foundations中移除
        for (int i = 0; i < foundations.Count; i++)
        {
            if (foundations[i].Remove(cardDisplay))
                return;
        }
        
        // 从 waste中移除
        waste.Remove(cardDisplay);
    }
    
    public List<List<CardDisplay>> GetTableauData()
    {
        return tableau;
    }
    
    public void RearrangeColumn(int columnIndex)
    {
        float cardHeight = 30f;
        for (int i = 0; i < tableau[columnIndex].Count; i++)
        {
            CardDisplay card = tableau[columnIndex][i];
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(46f, 70f);
            cardRect.anchorMin = new Vector2(0.5f, 1f);
            cardRect.anchorMax = new Vector2(0.5f, 1f);
            cardRect.pivot = new Vector2(0.5f, 1f);
            cardRect.anchoredPosition = new Vector2(0, -i * cardHeight);
        }
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
            
            // 设置父对象
            card.transform.SetParent(tableauColumns[targetColumnIndex], false);
            
            // 添加到列数据
            tableau[targetColumnIndex].Add(card);
            
            // 计算正确的位置（基于添加后的列总数）
            int currentRow = tableau[targetColumnIndex].Count - 1;
            
            // 设置位置和尺寸
            RectTransform cardRect = card.GetComponent<RectTransform>();
            
            // 重置缩放和尺寸
            card.transform.localScale = Vector3.one;
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
        bool normalRule = (int)card.rank == (int)topCard.rank - 1; // 正常规则：小1
        
        // 森林卡牌双向规则：森林卡可以接在±1的卡牌下面，其他卡也可以接在森林卡的±1位置
        bool forestRule = (card.set == "Forest/Backsides/Classic" && (int)card.rank == (int)topCard.rank + 1) ||
                         (topCard.set == "Forest/Backsides/Classic" && (int)card.rank == (int)topCard.rank + 1);
        
        // 火焰卡牌双向规则：火焰卡可以接在同点数卡牌下面，其他卡也可以接在火焰卡同点数位置
        bool fireRule = (card.set == "Fire/Backsides/Classic" && (int)card.rank == (int)topCard.rank) ||
                       (topCard.set == "Fire/Backsides/Classic" && (int)card.rank == (int)topCard.rank);
        
        // S02万能卡规则：S02可以接在任何卡下面，任何卡也可以接在S02下面
        bool s02Rule = card.set == "S02" || topCard.set == "S02";
        
        bool canPlace = normalRule || forestRule || fireRule || s02Rule;
        
        Debug.Log($"Trying to move {card.GetCardName()} to {topCard.GetCardName()}: normalRule={normalRule}, forestRule={forestRule}, canPlace={canPlace}");
        
        return canPlace;
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
                cardDisplay.transform.localScale = Vector3.one;
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
    
    public void ShowRewardCanvas()
    {
        RewardManager rewardManager = FindFirstObjectByType<RewardManager>();
        if (rewardManager != null)
        {
            rewardManager.ShowRewards();
        }
    }
    
    public void StartNextLevel()
    {
        // Reset deck with all cards (including rewards) then shuffle
        deck.ResetDeckForNewGame();
        deck.ShuffleDeck();
        ResetGame();
        SpawnNextEnemy();
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
            
            if (s02Prefab != null)
            {
                GameObject s02 = Instantiate(s02Prefab, handArea);
                s02.SetActive(true);
            }
        }
    }
    

    
    // 委托给AbilityManager的方法
    public void EnterCardSelectionMode() => abilityManager?.EnterCardSelectionMode();
    public bool IsInCardSelectionMode() => abilityManager?.IsInCardSelectionMode() ?? false;
    public void SelectCardForRemoval(CardDisplay cardDisplay) => abilityManager?.SelectCardForRemoval(cardDisplay);
    public void CancelCardSelection() => abilityManager?.CancelCardSelection();
    
    public void EnterCardSwapMode() => abilityManager?.EnterCardSwapMode();
    public bool IsInCardSwapMode() => abilityManager?.IsInCardSwapMode() ?? false;
    public void SelectCardForSwap(CardDisplay cardDisplay) => abilityManager?.SelectCardForSwap(cardDisplay);
    public void CancelCardSwap() => abilityManager?.CancelCardSwap();
    
    public void EnterS02Mode() => abilityManager?.EnterS02Mode();
    public bool IsInS02Mode() => abilityManager?.IsInS02Mode() ?? false;
    public void SelectCardForS02(CardDisplay targetCard) => abilityManager?.SelectCardForS02(targetCard);
    public void CancelS02() => abilityManager?.CancelS02();
    

    
    public void ResetGame()
    {
        // 清理现有卡牌
        ClearAllCards();
        
        // 重置游戏状态
        isDealing = false;
        abilityManager?.ResetAllModes();
        
        // 重新初始化牌组和游戏
        deck.InitializeDeck();
        deck.ShuffleDeck();
        InitializeGame();
    }
    
    void ClearAllCards()
    {
        // 清理tableau
        for (int i = 0; i < tableauColumns.Length; i++)
        {
            foreach (Transform child in tableauColumns[i])
            {
                if (child.GetComponent<CardDisplay>() != null)
                    Destroy(child.gameObject);
            }
        }
        
        // 清理foundation
        for (int i = 0; i < foundationPiles.Length; i++)
        {
            foreach (Transform child in foundationPiles[i])
            {
                if (child.GetComponent<CardDisplay>() != null)
                    Destroy(child.gameObject);
            }
        }
        
        // 清理stock
        if (stockPile != null)
        {
            foreach (Transform child in stockPile)
            {
                if (child.GetComponent<CardDisplay>() != null)
                    Destroy(child.gameObject);
            }
        }
        
        // 清理handArea中的Joker
        if (handArea != null)
        {
            foreach (Transform child in handArea)
            {
                Destroy(child.gameObject);
            }
        }
        
        // 清理敌人
        if (enemySpawnPoint != null)
        {
            foreach (Transform child in enemySpawnPoint)
            {
                Destroy(child.gameObject);
            }
        }
        
        // 清理数据结构
        tableau.Clear();
        foundations.Clear();
        stock.Clear();
        waste.Clear();
    }
}