using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
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
}