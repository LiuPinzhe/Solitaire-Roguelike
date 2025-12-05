using UnityEngine;
using UnityEngine.UI;

public class S02 : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    private bool isActive = false;
    private SolitaireGameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<SolitaireGameManager>();
        LoadS02Sprite();
        SetupButton();
    }
    
    void SetupButton()
    {
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnS02Click);
    }
    
    void LoadS02Sprite()
    {
        // 加载森林梅花J贴图 (Forest Cards index 24 = 梅花J)
        Sprite[] forestSprites = Resources.LoadAll<Sprite>("Forest/ForestCards");
        if (forestSprites.Length > 24)
        {
            if (cardImage == null)
                cardImage = GetComponent<Image>();
            cardImage.sprite = forestSprites[46]; // 梅花J
        }
    }
    
    public void OnS02Click()
    {
        if (!isActive)
        {
            // 激活S02模式
            isActive = true;
            cardImage.color = Color.yellow;
            gameManager.EnterS02Mode();
        }
        else
        {
            // 取消S02模式
            ResetS02();
        }
    }
    
    public void ResetS02()
    {
        isActive = false;
        cardImage.color = Color.white;
    }
}