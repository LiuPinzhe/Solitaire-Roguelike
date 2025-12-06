using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ComboMaster", menuName = "Skills/Combo Master")]
public class ComboMasterSkill : PassiveSkill
{
    public int minChainLength = 5;
    public float damageMultiplier = 1.5f;
    
    public override void OnAttack(List<CardDisplay> sequence, ref int damage)
    {
        if (sequence.Count >= minChainLength)
        {
            int bonus = (int)(damage * (damageMultiplier - 1f));
            damage += bonus;
            Debug.Log($"[Combo Master] Chain of {sequence.Count}! +{bonus} damage");
        }
    }
}
