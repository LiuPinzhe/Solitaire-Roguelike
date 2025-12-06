using UnityEngine;
using System.Collections.Generic;

public class PassiveSkillManager : MonoBehaviour
{
    private static PassiveSkillManager instance;
    public static PassiveSkillManager Instance => instance;
    
    private List<PassiveSkill> activeSkills = new List<PassiveSkill>();
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AddSkill(PassiveSkill skill)
    {
        if (!activeSkills.Contains(skill))
        {
            activeSkills.Add(skill);
            Debug.Log($"Added passive skill: {skill.skillName}");
        }
    }
    
    public void RemoveSkill(PassiveSkill skill)
    {
        activeSkills.Remove(skill);
    }
    
    public List<PassiveSkill> GetActiveSkills()
    {
        return new List<PassiveSkill>(activeSkills);
    }
    
    // 修改伤害
    public int ModifyDamage(List<CardDisplay> sequence, int baseDamage)
    {
        int damage = baseDamage;
        foreach (var skill in activeSkills)
        {
            skill.OnAttack(sequence, ref damage);
        }
        return damage;
    }
    
    // 触发敌人死亡事件
    public void TriggerEnemyKilled(Enemy enemy)
    {
        foreach (var skill in activeSkills)
        {
            skill.OnEnemyKilled(enemy);
        }
    }
    
    // 触发抽牌事件
    public void TriggerCardDrawn(CardDisplay card)
    {
        foreach (var skill in activeSkills)
        {
            skill.OnCardDrawn(card);
        }
    }
    
    // 触发回合开始事件
    public void TriggerTurnStart()
    {
        foreach (var skill in activeSkills)
        {
            skill.OnTurnStart();
        }
    }
}
