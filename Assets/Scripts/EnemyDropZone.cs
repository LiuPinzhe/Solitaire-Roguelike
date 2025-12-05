using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EnemyDropZone : MonoBehaviour, IDropHandler
{
    private Enemy enemy;
    
    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        CardDisplay draggedCard = eventData.pointerDrag?.GetComponent<CardDisplay>();
        if (draggedCard != null && enemy != null && enemy.IsAlive())
        {
            // 获取拖拽的卡牌序列
            List<CardDisplay> sequence = draggedCard.GetDraggedSequence();
            
            // 计算伤害（基于卡牌链长度和点数）
            int damage = CalculateDamage(sequence);
            
            // 显示伤害计算动画
            DamageCalculationUI.ShowDamageCalculation(sequence, damage);
            
            // 立即移除卡牌并延迟造成伤害
            RemoveCardSequence(sequence);
            StartCoroutine(DelayedDamage(damage));
            
            Debug.Log($"Attacked enemy with {sequence.Count} cards for {damage} damage!");
        }
    }
    
    System.Collections.IEnumerator DelayedDamage(int damage)
    {
        yield return new WaitForSeconds(2.5f); // 等待动画播放完成
        
        // 对敌人造成伤害
        if (enemy != null && enemy.IsAlive())
        {
            enemy.TakeDamage(damage);
        }
    }
    
    int CalculateDamage(List<CardDisplay> sequence)
    {
        int chainLength = sequence.Count;
        int headRank = (int)sequence[0].GetCard().rank;
        
        int damage = chainLength * 4 + (headRank - 1) * 2;
        
        if (chainLength >= 6)
        {
            damage += chainLength * chainLength;
        }
        
        return damage;
    }
    
    void RemoveCardSequence(List<CardDisplay> sequence)
    {
        SolitaireGameManager gameManager = FindFirstObjectByType<SolitaireGameManager>();
        
        foreach (CardDisplay card in sequence)
        {
            if (card != null)
            {
                // 从游戏数据中移除
                gameManager.RemoveCardFromGame(card);
                
                // 销毁GameObject
                Destroy(card.gameObject);
            }
        }
        
        // 检查并翻开新的顶牌
        gameManager.CheckAndRevealAllTopCards();
    }
}