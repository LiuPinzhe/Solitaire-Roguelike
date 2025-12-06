using UnityEngine;

[CreateAssetMenu(fileName = "SkipChain", menuName = "Skills/Skip Chain")]
public class SkipChainSkill : PassiveSkill
{
    // 这个技能不修改伤害，而是改变游戏规则
    // 通过PassiveSkillManager检查是否激活
}
