using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "SpadeCritical", menuName = "Skills/Spade Critical")]
public class SpadeCriticalSkill : PassiveSkill
{
    public float critChance = 0.3f;
    public float critMultiplier = 2f;
    
    public override void OnAttack(List<CardDisplay> sequence, ref int damage)
    {
        bool hasSpade = sequence.Any(c => c.GetCard().suit == Card.Suit.Spades);
        if (hasSpade && Random.value < critChance)
        {
            int bonus = (int)(damage * (critMultiplier - 1f));
            damage += bonus;
            Debug.Log($"[Spade Critical] CRITICAL HIT! +{bonus} damage");
        }
    }
}
