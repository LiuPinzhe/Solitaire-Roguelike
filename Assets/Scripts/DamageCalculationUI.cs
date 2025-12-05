using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DamageCalculationUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject calculationPanel;
    [SerializeField] private Text chainLengthText;
    [SerializeField] private Text headRankText;
    [SerializeField] private Text bonusText;
    [SerializeField] private Text totalDamageText;
    [SerializeField] private Transform cardDisplayContainer;
    [SerializeField] private GameObject cardPrefab;
    
    private static DamageCalculationUI instance;
    
    void Awake()
    {
        instance = this;
        if (calculationPanel != null)
            calculationPanel.SetActive(false);
    }
    
    public static void ShowDamageCalculation(List<CardDisplay> sequence, int totalDamage)
    {
        if (instance != null)
        {
            Debug.Log($"ShowDamageCalculation called with {sequence.Count} cards, {totalDamage} damage");
            instance.StartCoroutine(instance.AnimateDamageCalculation(sequence, totalDamage));
        }
        else
        {
            Debug.LogError("DamageCalculationUI instance is null! Creating temporary debug output.");
            // 临时调试输出
            int chainLength = sequence.Count;
            int headRank = (int)sequence[0].GetCard().rank;
            Debug.Log($"伤害计算: {headRank} × {chainLength} = {totalDamage}");
        }
    }
    
    IEnumerator AnimateDamageCalculation(List<CardDisplay> sequence, int totalDamage)
    {
        Debug.Log("AnimateDamageCalculation started");
        
        if (calculationPanel == null)
        {
            Debug.LogError("calculationPanel is null!");
            yield break;
        }
        
        calculationPanel.SetActive(true);
        
        // 显示打出的卡牌
        DisplayCards(sequence);
        
        int chainLength = sequence.Count;
        int headRank = (int)sequence[0].GetCard().rank;
        
        Debug.Log($"Calculated: headRank={headRank}, chainLength={chainLength}, total={totalDamage}");
        
        // 重置所有文本
        if (chainLengthText != null) chainLengthText.text = "";
        if (headRankText != null) headRankText.text = "";
        if (bonusText != null) bonusText.text = "";
        if (totalDamageText != null) totalDamageText.text = "";
        
        // 设置初始透明度
        CanvasGroup canvasGroup = calculationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = calculationPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        
        // 淡入面板
        canvasGroup.DOFade(1f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        
        // 显示计算公式
        if (chainLengthText != null)
        {
            chainLengthText.text = $"{headRank} × {chainLength}";
            chainLengthText.color = Color.cyan;
            Debug.Log($"Set chainLengthText: {chainLengthText.text}");
        }
        yield return new WaitForSeconds(0.8f);
        
        // 显示总伤害
        if (totalDamageText != null)
        {
            totalDamageText.text = $"= {totalDamage}";
            totalDamageText.color = Color.green;
            totalDamageText.transform.localScale = Vector3.zero;
            totalDamageText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
            Debug.Log($"Set totalDamageText: {totalDamageText.text}");
        }
        yield return new WaitForSeconds(1.5f);
        
        // 淡出面板
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() => {
            calculationPanel.SetActive(false);
            ClearDisplayedCards();
            Debug.Log("Animation completed");
        });
    }
    
    void DisplayCards(List<CardDisplay> sequence)
    {
        if (cardDisplayContainer == null || cardPrefab == null) return;
        
        // 清理之前的卡牌
        ClearDisplayedCards();
        
        int cardCount = sequence.Count;
        float containerWidth = cardDisplayContainer.GetComponent<RectTransform>().rect.width;
        
        // 根据卡牌数量调整间距
        float cardWidth = 80f;
        float maxSpacing = 35f;
        float minSpacing = 35f;
        
        // 计算每行卡牌数和间距
        int cardsPerRow = Mathf.Min(6, cardCount);
        float totalWidth = cardsPerRow * cardWidth + (cardsPerRow - 1) * maxSpacing;
        
        // 如果超出容器宽度，减少间距
        float spacing = maxSpacing;
        if (totalWidth > containerWidth)
        {
            spacing = Mathf.Max(minSpacing, (containerWidth - cardsPerRow * cardWidth) / (cardsPerRow - 1));
        }
        
        for (int i = 0; i < cardCount; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardDisplayContainer);
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
            
            if (cardDisplay != null)
            {
                cardDisplay.SetCard(sequence[i].GetCard());
                cardDisplay.RevealCard();
                
                RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                cardRect.sizeDelta = new Vector2(cardWidth, 120f);
                
                // 计算位置
                int row = i / cardsPerRow;
                int col = i % cardsPerRow;
                
                float startX = -(cardsPerRow - 1) * spacing / 2f;
                Vector2 targetPos = new Vector2(startX + col * (cardWidth + spacing), -row * 130f);
                
                // 直接设置位置
                cardRect.anchoredPosition = targetPos;
            }
        }
    }
    
    void ClearDisplayedCards()
    {
        if (cardDisplayContainer == null) return;
        
        foreach (Transform child in cardDisplayContainer)
        {
            Destroy(child.gameObject);
        }
    }
}