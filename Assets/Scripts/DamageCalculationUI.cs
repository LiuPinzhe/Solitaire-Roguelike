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
        
        // 卡牌飞向敌人，等待完成后再淡出面板
        yield return StartCoroutine(AnimateCardsToEnemy());
        
        // 等待卡牌飞行完成后淡出面板
        yield return new WaitForSeconds(0.7f); // 等待最后一张卡牌飞行完成
        
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() => {
            calculationPanel.SetActive(false);
            Debug.Log("Animation completed");
        });
    }
    
    void DisplayCards(List<CardDisplay> sequence)
    {
        if (cardDisplayContainer == null || cardPrefab == null) return;
        
        // 清理之前的卡牌
        ClearDisplayedCards();
        
        StartCoroutine(AnimateCardsFromRight(sequence));
    }
    
    System.Collections.IEnumerator AnimateCardsFromRight(List<CardDisplay> sequence)
    {
        int cardCount = sequence.Count;
        
        // 根据卡牌数量调整间距
        float cardWidth = 80f;
        float spacing = 35f;
        
        // 计算每行卡牌数
        int cardsPerRow = Mathf.Min(6, cardCount);
        
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
                
                // 设置锚点为左上角，类似于tableau列
                cardRect.anchorMin = new Vector2(0f, 1f);
                cardRect.anchorMax = new Vector2(0f, 1f);
                cardRect.pivot = new Vector2(0f, 1f);
                
                // 计算目标位置：从左到右按间距排列，统一Y位置
                Vector2 targetPos = new Vector2(i * spacing, 0f);
                
                // 设置初始位置（从右侧飞入）
                Vector2 startPos = new Vector2(500f, 0f);
                cardRect.anchoredPosition = startPos;
                
                // 设置正确的层级顺序（在动画前设置）
                cardObj.transform.SetSiblingIndex(i);
                
                // 动画到目标位置
                cardRect.DOAnchorPos(targetPos, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.08f);
                
                // 等待一小段时间再发下一张牌
                yield return new WaitForSeconds(0.08f);
            }
        }
        
        yield return null;
    }
    
    System.Collections.IEnumerator AnimateCardsToEnemy()
    {
        if (cardDisplayContainer == null) yield break;
        
        // 敌人位置（向右侧的第5张卡牌位置下方）
        Vector2 enemyPos = new Vector2(140f, -400f);
        
        // 获取所有卡牌并飞向敌人
        Transform[] cards = new Transform[cardDisplayContainer.childCount];
        for (int i = 0; i < cardDisplayContainer.childCount; i++)
        {
            cards[i] = cardDisplayContainer.GetChild(i);
        }
        
        foreach (Transform cardTransform in cards)
        {
            if (cardTransform != null)
            {
                RectTransform cardRect = cardTransform.GetComponent<RectTransform>();
                if (cardRect != null)
                {
                    // 停止所有在运行的动画
                    cardRect.DOKill();
                    
                    cardRect.DOAnchorPos(enemyPos, 0.6f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => {
                            if (cardTransform != null && cardTransform.gameObject != null)
                                Destroy(cardTransform.gameObject);
                        });
                }
            }
            
            // 卡牌间隔飞出
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return null;
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