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
        int baseDamage = headRank * chainLength;
        int skillBonus = totalDamage - baseDamage;
        
        Debug.Log($"Calculated: headRank={headRank}, chainLength={chainLength}, base={baseDamage}, bonus={skillBonus}, total={totalDamage}");
        
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
        
        // 显示基础计算公式
        if (chainLengthText != null)
        {
            chainLengthText.text = $"{headRank} × {chainLength}";
            chainLengthText.color = Color.cyan;
        }
        yield return new WaitForSeconds(0.8f);
        
        // 显示技能加成
        if (skillBonus > 0 && bonusText != null)
        {
            bonusText.text = $"+ {skillBonus} (技能加成)";
            bonusText.color = Color.yellow;
            bonusText.transform.localScale = Vector3.zero;
            bonusText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.6f);
        }
        
        // 显示总伤害
        if (totalDamageText != null)
        {
            totalDamageText.text = $"= {totalDamage}";
            totalDamageText.color = Color.green;
            totalDamageText.transform.localScale = Vector3.zero;
            totalDamageText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
        }
        yield return new WaitForSeconds(1.2f);
        
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
        
        // 多行布局参数
        float cardWidth = 80f;
        float cardHeight = 120f;
        float spacing = 35f;
        int maxCardsPerRow = 5;
        float rowSpacing = 130f; // 行间距
        
        for (int i = 0; i < cardCount; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardDisplayContainer);
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
            
            if (cardDisplay != null)
            {
                cardDisplay.SetCard(sequence[i].GetCard());
                cardDisplay.RevealCard();
                
                RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                cardRect.sizeDelta = new Vector2(cardWidth, cardHeight);
                
                // 设置锚点为左上角
                cardRect.anchorMin = new Vector2(0f, 1f);
                cardRect.anchorMax = new Vector2(0f, 1f);
                cardRect.pivot = new Vector2(0f, 1f);
                
                // 计算行列位置
                int row = i / maxCardsPerRow;
                int col = i % maxCardsPerRow;
                
                // 计算目标位置
                Vector2 targetPos = new Vector2(col * spacing, -row * rowSpacing);
                
                // 设置初始位置（从右侧飞入）
                Vector2 startPos = new Vector2(500f, -row * rowSpacing);
                cardRect.anchoredPosition = startPos;
                
                // 设置层级顺序
                cardObj.transform.SetSiblingIndex(i);
                
                // 动画到目标位置
                cardRect.DOAnchorPos(targetPos, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.08f);
                
                yield return new WaitForSeconds(0.08f);
            }
        }
        
        yield return null;
    }
    
    System.Collections.IEnumerator AnimateCardsToEnemy()
    {
        if (cardDisplayContainer == null) yield break;
        
        // 敌人位置
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