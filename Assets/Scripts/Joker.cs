using UnityEngine;
using UnityEngine.UI;

public class Joker : MonoBehaviour
{
    [SerializeField] private Image jokerImage;
    private bool isActive = true;
    private SolitaireGameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<SolitaireGameManager>();
        LoadJokerSprite();
        SetupButton();
        Debug.Log($"[Joker] Joker initialized, gameManager found: {gameManager != null}");
    }
    
    void SetupButton()
    {
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(ActivateJoker);
        Debug.Log($"[Joker] Button component setup complete");
    }
    
    void LoadJokerSprite()
    {
        Sprite[] jokerSprites = Resources.LoadAll<Sprite>("Classic/ClassicJokers");
        if (jokerSprites.Length > 0)
        {
            if (jokerImage == null)
                jokerImage = GetComponent<Image>();
            jokerImage.sprite = jokerSprites[0]; // 第一张Joker
        }
    }
    

    
    public void ActivateJoker()
    {
        Debug.Log($"[Joker] ActivateJoker called");
        
        // 检查是否已经在选择模式中，如果是则取消
        if (gameManager != null && gameManager.IsInCardSelectionMode())
        {
            Debug.Log($"[Joker] Canceling card selection mode");
            gameManager.CancelCardSelection();
            return;
        }
        
        isActive = false;
        
        if (jokerImage != null)
        {
            jokerImage.color = Color.gray;
            Debug.Log($"[Joker] Joker image set to gray");
        }
        else
        {
            Debug.LogError($"[Joker] jokerImage is null!");
        }
        
        // 启用选择模式
        if (gameManager != null)
        {
            Debug.Log($"[Joker] Calling EnterCardSelectionMode");
            gameManager.EnterCardSelectionMode();
        }
        else
        {
            Debug.LogError($"[Joker] gameManager is null!");
        }
    }
    
    public void ResetJoker()
    {
        isActive = true;
        jokerImage.color = Color.white;
    }
}