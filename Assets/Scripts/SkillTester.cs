using UnityEngine;

public class SkillTester : MonoBehaviour
{
    [Header("Test Skills - Check to Activate")]
    [SerializeField] private bool activateComboMaster = false;
    [SerializeField] private bool activateRoyalBlood = false;
    [SerializeField] private bool activateSpadeCritical = false;
    [SerializeField] private bool activateForestPower = false;
    [SerializeField] private bool activateSkipChain = false;
    
    void Start()
    {
        Invoke("ActivateTestSkills", 0.1f);
    }
    
    void ActivateTestSkills()
    {
        Debug.Log("SkillTester: Starting skill activation...");
        
        if (PassiveSkillManager.Instance == null)
        {
            Debug.LogError("PassiveSkillManager.Instance is NULL! Add PassiveSkillManager to scene.");
            return;
        }
        
        if (activateComboMaster)
            ActivateSkill("Skills/ComboMaster");
            
        if (activateRoyalBlood)
            ActivateSkill("Skills/RoyalBlood");
            
        if (activateSpadeCritical)
            ActivateSkill("Skills/SpadeCritical");
            
        if (activateForestPower)
            ActivateSkill("Skills/ForestPower");
            
        if (activateSkipChain)
            ActivateSkill("Skills/SkipChain");
    }
    
    void ActivateSkill(string path)
    {
        Debug.Log($"Trying to load skill from: {path}");
        PassiveSkill skill = Resources.Load<PassiveSkill>(path);
        
        if (skill == null)
        {
            Debug.LogError($"Failed to load skill from Resources/{path}. Make sure the ScriptableObject exists!");
            return;
        }
        
        PassiveSkillManager.Instance.AddSkill(skill);
        Debug.Log($"✓ Activated: {skill.skillName}");
    }
}
