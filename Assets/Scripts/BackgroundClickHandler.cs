using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
{
    private SolitaireGameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<SolitaireGameManager>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager != null && gameManager.IsInCardSelectionMode())
        {
            gameManager.CancelCardSelection();
        }
    }
}