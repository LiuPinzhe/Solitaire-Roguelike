using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ForestPower", menuName = "Skills/Forest Power")]
public class ForestPowerSkill : PassiveSkill
{
    public int bonusPerForestCard = 3;
    
    public override void OnAttack(List<CardDisplay> sequence, ref int damage)
    {
        int forestCount = sequence.Count(c => c.GetCard().set == "Forest/Backsides/Classic");
        if (forestCount > 0)
        {
            int bonus = forestCount * bonusPerForestCard;
            damage += bonus;
            Debug.Log($"[Forest Power] {forestCount} forest cards! +{bonus} damage");
        }
    }
}
