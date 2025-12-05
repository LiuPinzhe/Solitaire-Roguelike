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
    [SerializeField] private Transform calculationContainer;
    
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
            int chainDamage = chainLength * 4;
            int rankDamage = (headRank - 1) * 2;
            int bonus = chainLength >= 6 ? chainLength * chainLength : 0;
            Debug.Log($"伤害计算: 链长度({chainLength}×4={chainDamage}) + 首牌点数(({headRank}-1)×2={rankDamage}) + 奖励({bonus}) = 总伤害({totalDamage})");
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
        
        int chainLength = sequence.Count;
        int headRank = (int)sequence[0].GetCard().rank;
        int chainDamage = chainLength * 4;
        int rankDamage = (headRank - 1) * 2;
        int bonus = chainLength >= 6 ? chainLength * chainLength : 0;
        
        Debug.Log($"Calculated: chainDamage={chainDamage}, rankDamage={rankDamage}, bonus={bonus}, total={totalDamage}");
        
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
        
        // 逐步显示计算过程
        if (chainLengthText != null)
        {
            chainLengthText.text = $"链长度: {chainLength} × 4 = {chainDamage}";
            Debug.Log($"Set chainLengthText: {chainLengthText.text}");
        }
        yield return new WaitForSeconds(0.8f);
        
        if (headRankText != null)
        {
            headRankText.text = $"首牌点数: ({headRank} - 1) × 2 = {rankDamage}";
            Debug.Log($"Set headRankText: {headRankText.text}");
        }
        yield return new WaitForSeconds(0.8f);
        
        if (bonus > 0 && bonusText != null)
        {
            bonusText.text = $"长链奖励: {chainLength} × {chainLength} = {bonus}";
            Debug.Log($"Set bonusText: {bonusText.text}");
            yield return new WaitForSeconds(0.8f);
        }
        
        // 显示总伤害
        if (totalDamageText != null)
        {
            totalDamageText.text = $"总伤害: {totalDamage}";
            totalDamageText.transform.localScale = Vector3.zero;
            totalDamageText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
            Debug.Log($"Set totalDamageText: {totalDamageText.text}");
        }
        yield return new WaitForSeconds(1.5f);
        
        // 淡出面板
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() => {
            calculationPanel.SetActive(false);
            Debug.Log("Animation completed");
        });
    }
}