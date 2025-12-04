using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropZone : MonoBehaviour
{
    public enum ZoneType { Tableau, Foundation, Stock, Waste }
    
    [SerializeField] private ZoneType zoneType;
    [SerializeField] private int zoneIndex;
    
    private SolitaireGameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<SolitaireGameManager>();
    }
    
    public bool TryDropCard(CardDisplay cardDisplay)
    {
        bool canMove = false;
        
        switch (zoneType)
        {
            case ZoneType.Foundation:
                canMove = gameManager.CanMoveToFoundation(cardDisplay, zoneIndex);
                break;
            case ZoneType.Tableau:
                canMove = gameManager.CanMoveToTableau(cardDisplay, zoneIndex);
                break;
        }
        
        if (canMove)
        {
            // 获取被拖拽的序列
            List<CardDisplay> draggedSequence = cardDisplay.GetDraggedSequence();
            
            if (zoneType == ZoneType.Tableau)
            {
                gameManager.MoveCardSequence(draggedSequence, zoneIndex);
            }
            else
            {
                // Foundation只能放单张牌
                gameManager.MoveCard(cardDisplay, zoneType, zoneIndex);
            }
            
            return true;
        }
        
        return false;
    }
}