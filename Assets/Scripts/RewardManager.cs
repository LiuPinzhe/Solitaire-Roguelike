using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject rewardCanvas;
    [SerializeField] private Button[] rewardButtons = new Button[3];
    [SerializeField] private Transform[] rewardCardAreas = new Transform[3];
    [SerializeField] private GameObject cardPrefab;
    
    private List<Card>[] rewardOptions = new List<Card>[3];
    private SolitaireGameManager gameManager;
    private Deck deck;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<SolitaireGameManager>();
        deck = FindFirstObjectByType<Deck>();
        
        for (int i = 0; i < 3; i++)
        {
            int optionIndex = i;
            rewardButtons[i].onClick.AddListener(() => SelectReward(optionIndex));
        }
    }
    
    public void ShowRewards()
    {
        Debug.Log("ShowRewards called");
        GenerateRewardOptions();
        DisplayRewardOptions();
        if (rewardCanvas != null)
        {
            Debug.Log("Activating reward canvas");
            rewardCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Reward canvas is null!");
        }
    }
    
    void GenerateRewardOptions()
    {
        string[] sets = { "Classic", "Forest/Backsides/Classic", "Space/Backsides/Classic", "Fire/Backsides/Classic" };
        
        for (int i = 0; i < 3; i++)
        {
            rewardOptions[i] = new List<Card>();
            string selectedSet = sets[Random.Range(0, sets.Length)];
            int cardCount = Random.Range(3, 6);
            int startRank = Random.Range(1, 14 - cardCount);
            Card.Suit suit = (Card.Suit)Random.Range(0, 4);
            
            for (int j = 0; j < cardCount; j++)
            {
                Card.Rank rank = (Card.Rank)(startRank + j);
                Sprite cardSprite = GetCardSprite(selectedSet, suit, rank);
                Card newCard = new Card(suit, rank, cardSprite, selectedSet);
                rewardOptions[i].Add(newCard);
            }
        }
    }
    
    Sprite GetCardSprite(string set, Card.Suit suit, Card.Rank rank)
    {
        string folderPath = set == "Classic" ? "Classic/ClassicCards" : 
                           set == "Forest/Backsides/Classic" ? "Forest/ForestCards" :
                           set == "Space/Backsides/Classic" ? "Space/SpaceCards" : "Fire/FireCards";
        
        int spriteIndex = ((int)rank - 1) * 2;
        if ((int)suit == 0 || (int)suit == 3)
        {
            spriteIndex += ((int)suit == 0) ? 0 : 1;
        }
        else
        {
            spriteIndex += 26 + (((int)suit == 2) ? 0 : 1);
        }
        
        Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
        return sprites.Length > spriteIndex ? sprites[spriteIndex] : null;
    }
    
    void DisplayRewardOptions()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (Transform child in rewardCardAreas[i])
            {
                Destroy(child.gameObject);
            }
            
            for (int j = 0; j < rewardOptions[i].Count; j++)
            {
                GameObject cardObj = Instantiate(cardPrefab, rewardCardAreas[i]);
                CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
                cardDisplay.SetCard(rewardOptions[i][j]);
                cardDisplay.RevealCard();
                
                RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                cardRect.sizeDelta = new Vector2(46f, 70f);
                cardRect.anchoredPosition = new Vector2(j * 50f, 0);
            }
        }
    }
    
    public void SelectReward(int optionIndex)
    {
        foreach (Card card in rewardOptions[optionIndex])
        {
            deck.AddCard(card);
        }
        
        rewardCanvas.SetActive(false);
        gameManager.StartNextLevel();
    }
}