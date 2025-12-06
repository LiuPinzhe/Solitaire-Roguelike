using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PassiveSkill skill;
    private GameObject tooltipPanel;
    
    public void SetSkill(PassiveSkill skill)
    {
        this.skill = skill;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skill == null) return;
        
        // 创建提示面板
        tooltipPanel = new GameObject("SkillTooltip");
        tooltipPanel.transform.SetParent(transform.root, false);
        
        Canvas canvas = tooltipPanel.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;
        
        RectTransform rect = tooltipPanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 100);
        rect.position = Input.mousePosition + new Vector3(10, -10, 0);
        
        // 背景
        Image bg = tooltipPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);
        
        // 文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(tooltipPanel.transform, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = $"<b>{skill.skillName}</b>\n{skill.description}";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            Destroy(tooltipPanel);
        }
    }
}
