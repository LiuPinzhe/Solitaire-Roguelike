using UnityEngine;
using System.Collections.Generic;

public abstract class PassiveSkill : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    
    // 触发时机
    public virtual void OnAttack(List<CardDisplay> sequence, ref int damage) { }
    public virtual void OnEnemyKilled(Enemy enemy) { }
    public virtual void OnCardDrawn(CardDisplay card) { }
    public virtual void OnTurnStart() { }
}
