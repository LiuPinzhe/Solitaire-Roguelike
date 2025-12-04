using UnityEngine;
using UnityEngine.EventSystems;

public class StockPileClickHandler : MonoBehaviour, IPointerClickHandler
{
    private SolitaireGameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<SolitaireGameManager>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Stock pile clicked!");
        if (gameManager != null)
        {
            gameManager.DrawFromStock();
        }
    }
}