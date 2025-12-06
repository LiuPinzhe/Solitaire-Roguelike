using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform skillIconContainer;
    [SerializeField] private GameObject skillIconPrefab;
    
    private PassiveSkillManager skillManager;
    private List<GameObject> skillIcons = new List<GameObject>();
    
    void Start()
    {
        skillManager = PassiveSkillManager.Instance;
        
        // 自动激活初始技能
        AutoActivateStartingSkills();
    }
    
    void AutoActivateStartingSkills()
    {
        // 留空，不自动激活任何技能
    }
    
    public void ActivateSkill(string skillPath)
    {
        PassiveSkill skill = Resources.Load<PassiveSkill>(skillPath);
        if (skill != null)
        {
            skillManager.AddSkill(skill);
            AddSkillIcon(skill);
        }
    }
    
    public void AddSkillIcon(PassiveSkill skill)
    {
        if (skillIconPrefab == null || skillIconContainer == null) return;
        
        GameObject iconObj = Instantiate(skillIconPrefab, skillIconContainer);
        Image iconImage = iconObj.GetComponent<Image>();
        
        if (iconImage != null && skill.icon != null)
        {
            iconImage.sprite = skill.icon;
        }
        
        // 添加悬停提示
        SkillTooltip tooltip = iconObj.AddComponent<SkillTooltip>();
        tooltip.SetSkill(skill);
        
        skillIcons.Add(iconObj);
    }
    
    public void ClearSkillIcons()
    {
        foreach (var icon in skillIcons)
        {
            Destroy(icon);
        }
        skillIcons.Clear();
    }
}
