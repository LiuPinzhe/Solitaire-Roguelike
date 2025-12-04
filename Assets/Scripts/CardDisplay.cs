using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Card cardData;
    [SerializeField] private bool isRevealed = true;
    
    private Sprite cardBackSprite;
    
    void Start()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();
    }
    
    public void SetCard(Card card, Sprite backSprite = null)
    {
        cardData = card;
        cardBackSprite = backSprite;
        if (cardImage == null)
            cardImage = GetComponent<Image>();
        UpdateCardDisplay();
        Debug.Log("Set card: " + card.GetCardName() + ", sprite: " + (card.cardSprite != null ? card.cardSprite.name : "null"));
    }
    
    public void RevealCard()
    {
        isRevealed = true;
        UpdateCardDisplay();
    }
    
    public void HideCard()
    {
        isRevealed = false;
        UpdateCardDisplay();
    }
    
    void UpdateCardDisplay()
    {
        if (cardImage != null && cardData != null)
        {
            if (isRevealed && cardData.cardSprite != null)
            {
                cardImage.sprite = cardData.cardSprite;
                Debug.Log("Displaying card sprite: " + cardData.cardSprite.name);
            }
            else if (cardBackSprite != null)
            {
                cardImage.sprite = cardBackSprite;
            }
        }
    }
    
    public Card GetCard()
    {
        return cardData;
    }
    
    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;
    private List<CardDisplay> draggedSequence = new List<CardDisplay>();
    private List<Vector3> originalPositions = new List<Vector3>();
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isRevealed) return;
        
        // 只有顶部连续序列可以拖拽
        if (!CanDragFromPosition()) return;
        
        startPosition = transform.position;
        startParent = transform.parent;
        
        // 获取连续的牌序列
        draggedSequence = GetDraggableSequence();
        
        // 保存所有牌的原始位置
        originalPositions.Clear();
        foreach (CardDisplay card in draggedSequence)
        {
            originalPositions.Add(card.transform.position);
        }
        
        // 设置所有被拖拽的牌
        foreach (CardDisplay card in draggedSequence)
        {
            card.canvasGroup.blocksRaycasts = false;
            card.canvasGroup.alpha = 0.8f;
            card.transform.SetAsLastSibling();
        }
        
        Debug.Log($"Dragging sequence of {draggedSequence.Count} cards starting with {GetCard().GetCardName()}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isRevealed) return;
        
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPosition);
        
        // 移动整个序列
        Vector3 offset = worldPosition - transform.position;
        foreach (CardDisplay card in draggedSequence)
        {
            card.transform.position += offset;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isRevealed) return;
        
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        // 检查是否放置到有效区域
        bool validDrop = false;
        
        // 使用Raycast检测放置目标
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        Debug.Log($"Raycast found {raycastResults.Count} results");
        
        foreach (var result in raycastResults)
        {
            Debug.Log($"Raycast hit: {result.gameObject.name} (has Image: {result.gameObject.GetComponent<Image>() != null})");
            DropZone dropZone = result.gameObject.GetComponent<DropZone>();
            if (dropZone != null)
            {
                Debug.Log($"Found DropZone: {dropZone.name}");
                validDrop = dropZone.TryDropCard(this);
                if (validDrop) break;
            }
        }
        
        if (raycastResults.Count == 0)
        {
            Debug.LogWarning("No raycast results! Make sure target objects have Image components with Raycast Target enabled.");
        }
        
        if (!validDrop)
        {
            // 所有牌回到精确的原位置
            for (int i = 0; i < draggedSequence.Count; i++)
            {
                CardDisplay card = draggedSequence[i];
                card.transform.position = originalPositions[i];
                card.transform.SetParent(startParent);
                card.canvasGroup.blocksRaycasts = true;
                card.canvasGroup.alpha = 1f;
            }
            Debug.Log("Invalid drop, returning sequence to original positions");
        }
        else
        {
            // 恢复所有牌的状态
            foreach (CardDisplay card in draggedSequence)
            {
                card.canvasGroup.blocksRaycasts = true;
                card.canvasGroup.alpha = 1f;
            }
            Debug.Log("Valid drop completed for sequence");
        }
        
        draggedSequence.Clear();
        originalPositions.Clear();
    }
    
    public List<CardDisplay> GetDraggedSequence()
    {
        return new List<CardDisplay>(draggedSequence);
    }
    
    // 获取可拖拽的连续序列
    public List<CardDisplay> GetDraggableSequence()
    {
        List<CardDisplay> sequence = new List<CardDisplay>();
        SolitaireGameManager gameManager = FindFirstObjectByType<SolitaireGameManager>();
        
        // 找到当前牌在哪一列
        int columnIndex = -1;
        for (int i = 0; i < 7; i++)
        {
            List<CardDisplay> column = gameManager.GetTableauColumn(i);
            int cardIndex = column.IndexOf(this);
            if (cardIndex >= 0)
            {
                columnIndex = i;
                
                // 从当前牌开始检查连续序列
                for (int j = cardIndex; j < column.Count; j++)
                {
                    CardDisplay currentCard = column[j];
                    
                    // 检查是否连续
                    if (sequence.Count == 0)
                    {
                        sequence.Add(currentCard);
                    }
                    else
                    {
                        CardDisplay prevCard = sequence[sequence.Count - 1];
                        if ((int)currentCard.GetCard().rank == (int)prevCard.GetCard().rank - 1)
                        {
                            sequence.Add(currentCard);
                        }
                        else
                        {
                            break; // 不连续就停止
                        }
                    }
                }
                break;
            }
        }
        
        return sequence;
    }
    
    bool CanDragFromPosition()
    {
        SolitaireGameManager gm = FindFirstObjectByType<SolitaireGameManager>();
        for (int i = 0; i < 7; i++)
        {
            var col = gm.GetTableauColumn(i);
            int idx = col.IndexOf(this);
            if (idx >= 0)
            {
                // 检查从当前位置到末尾是否连续
                for (int j = idx; j < col.Count - 1; j++)
                    if ((int)col[j + 1].GetCard().rank != (int)col[j].GetCard().rank - 1)
                        return false;
                return true;
            }
        }
        return false;
    }
}