using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "RoyalBlood", menuName = "Skills/Royal Blood")]
public class RoyalBloodSkill : PassiveSkill
{
    public int bonusPerRoyal = 5;
    
    public override void OnAttack(List<CardDisplay> sequence, ref int damage)
    {
        int royalCount = sequence.Count(c => c.GetCard().rank >= Card.Rank.Jack);
        if (royalCount > 0)
        {
            int bonus = royalCount * bonusPerRoyal;
            damage += bonus;
            Debug.Log($"[Royal Blood] {royalCount} royal cards! +{bonus} damage");
        }
    }
}
