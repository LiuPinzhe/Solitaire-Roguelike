using UnityEngine;
using UnityEngine.UI;

public class Joker2 : MonoBehaviour
{
    [SerializeField] private Image jokerImage;
    private bool isActive = true;
    private AbilityManager abilityManager;
    
    void Start()
    {
        abilityManager = FindObjectOfType<AbilityManager>();
        LoadJokerSprite();
        SetupButton();
    }
    
    void SetupButton()
    {
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(ActivateJoker2);
    }
    
    void LoadJokerSprite()
    {
        Sprite[] jokerSprites = Resources.LoadAll<Sprite>("Classic/ClassicDarkJokers");
        if (jokerSprites.Length > 0)
        {
            if (jokerImage == null)
                jokerImage = GetComponent<Image>();
            jokerImage.sprite = jokerSprites[0]; // 第一张DarkJoker
        }
    }
    
    public void ActivateJoker2()
    {
        // 检查是否已经在交换模式中，如果是则取消
        if (abilityManager != null && abilityManager.IsInCardSwapMode())
        {
            abilityManager.CancelCardSwap();
            return;
        }
        
        isActive = false;
        
        if (jokerImage != null)
        {
            jokerImage.color = Color.gray;
        }
        
        // 启用交换模式
        if (abilityManager != null)
        {
            abilityManager.EnterCardSwapMode();
        }
    }
    
    public void ResetJoker2()
    {
        isActive = true;
        jokerImage.color = Color.white;
    }
}