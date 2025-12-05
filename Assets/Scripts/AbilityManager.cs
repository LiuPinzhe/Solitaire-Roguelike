using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class AbilityManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private SolitaireGameManager gameManager;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform[] tableauColumns;
    
    // 模式状态
    private bool cardSelectionMode = false;
    private bool cardSwapMode = false;
    private bool s02Mode = false;
    private CardDisplay firstSelectedCard = null;
    
    void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<SolitaireGameManager>();
            
        // 获取tableauColumns引用
        if (tableauColumns == null || tableauColumns.Length == 0)
        {
            GameObject[] columns = new GameObject[7];
            for (int i = 0; i < 7; i++)
            {
                columns[i] = GameObject.Find($"Column{i}");
            }
            tableauColumns = columns.Select(go => go?.transform).ToArray();
        }
    }
    
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (cardSelectionMode) CheckForCancelSelection();
            if (cardSwapMode) CheckForCancelSwap();
            if (s02Mode) CheckForCancelS02();
        }
    }
    
    // Joker1 - 卡牌消除功能
    public void EnterCardSelectionMode()
    {
        cardSelectionMode = true;
    }
    
    public bool IsInCardSelectionMode()
    {
        return cardSelectionMode;
    }
    
    public void SelectCardForRemoval(CardDisplay cardDisplay)
    {
        if (!cardSelectionMode) return;
        
        gameManager.RemoveCardFromCurrentLocation(cardDisplay);
        Destroy(cardDisplay.gameObject);
        gameManager.CheckAndRevealAllTopCards();
        
        Joker joker = FindObjectOfType<Joker>();
        if (joker != null) joker.ResetJoker();
        
        cardSelectionMode = false;
    }
    
    public void CancelCardSelection()
    {
        if (!cardSelectionMode) return;
        
        Joker joker = FindObjectOfType<Joker>();
        if (joker != null) joker.ResetJoker();
        
        cardSelectionMode = false;
    }
    
    // Joker2 - 卡牌交换功能
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
            firstSelectedCard = cardDisplay;
            cardDisplay.GetComponent<Image>().color = Color.yellow;
        }
        else if (firstSelectedCard == cardDisplay)
        {
            cardDisplay.GetComponent<Image>().color = Color.white;
            firstSelectedCard = null;
        }
        else
        {
            SwapCards(firstSelectedCard, cardDisplay);
            firstSelectedCard.GetComponent<Image>().color = Color.white;
            firstSelectedCard = null;
            
            Joker2 joker2 = FindObjectOfType<Joker2>();
            if (joker2 != null) joker2.ResetJoker2();
            
            cardSwapMode = false;
        }
    }
    
    void SwapCards(CardDisplay card1, CardDisplay card2)
    {
        Card tempCard = card1.GetCard();
        card1.SetCard(card2.GetCard());
        card2.SetCard(tempCard);
    }
    
    public void CancelCardSwap()
    {
        if (!cardSwapMode) return;
        
        if (firstSelectedCard != null)
        {
            firstSelectedCard.GetComponent<Image>().color = Color.white;
            firstSelectedCard = null;
        }
        
        Joker2 joker2 = FindObjectOfType<Joker2>();
        if (joker2 != null) joker2.ResetJoker2();
        
        cardSwapMode = false;
    }
    
    // S02 - 万能卡功能
    public void EnterS02Mode()
    {
        s02Mode = true;
    }
    
    public bool IsInS02Mode()
    {
        return s02Mode;
    }
    
    public void SelectCardForS02(CardDisplay targetCard)
    {
        if (!s02Mode) return;
        
        Sprite[] forestSprites = Resources.LoadAll<Sprite>("Forest/ForestCards");
        Sprite jackSprite = forestSprites.Length > 46 ? forestSprites[46] : null;
        Card s02Card = new Card(Card.Suit.Clubs, Card.Rank.Jack, jackSprite, "S02");
        
        InsertS02Card(targetCard, s02Card);
        
        S02 s02Component = FindObjectOfType<S02>();
        if (s02Component != null) s02Component.ResetS02();
        
        s02Mode = false;
    }
    
    void InsertS02Card(CardDisplay targetCard, Card s02Card)
    {
        var tableau = gameManager.GetTableauData();
        
        for (int col = 0; col < tableau.Count; col++)
        {
            int targetIndex = tableau[col].IndexOf(targetCard);
            if (targetIndex >= 0)
            {
                GameObject s02CardObj = Instantiate(cardPrefab, tableauColumns[col]);
                CardDisplay s02Display = s02CardObj.GetComponent<CardDisplay>();
                s02Display.SetCard(s02Card);
                s02Display.RevealCard();
                
                // 插入到数据列表
                tableau[col].Insert(targetIndex + 1, s02Display);
                
                // 设置正确的Transform层级顺序
                s02Display.transform.SetSiblingIndex(targetIndex + 1);
                
                gameManager.RearrangeColumn(col);
                break;
            }
        }
    }
    
    public void CancelS02()
    {
        if (!s02Mode) return;
        
        S02 s02Component = FindObjectOfType<S02>();
        if (s02Component != null) s02Component.ResetS02();
        
        s02Mode = false;
    }
    
    // 取消检测
    void CheckForCancelSelection()
    {
        if (!HitCard()) CancelCardSelection();
    }
    
    void CheckForCancelSwap()
    {
        if (!HitCard()) CancelCardSwap();
    }
    
    void CheckForCancelS02()
    {
        if (!HitCard()) CancelS02();
    }
    
    bool HitCard()
    {
        var raycastResults = new List<UnityEngine.EventSystems.RaycastResult>();
        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, raycastResults);
        
        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<CardDisplay>() != null)
                return true;
        }
        return false;
    }
    
    // 重置所有模式
    public void ResetAllModes()
    {
        cardSelectionMode = false;
        cardSwapMode = false;
        s02Mode = false;
        firstSelectedCard = null;
    }
}