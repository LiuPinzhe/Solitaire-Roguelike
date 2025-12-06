using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    [Header("UI Elements")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Text damageText;
    
    [Header("Animation")]
    [SerializeField] protected Image enemyImage;
    protected Coroutine currentAnimation;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        
        if (enemyImage == null)
        {
            enemyImage = GetComponent<Image>();
        }
        
        // 自动查找UI元素
        if (healthBar == null)
        {
            healthBar = GameObject.Find("HealthBar")?.GetComponent<Image>();
        }
        if (healthText == null)
        {
            healthText = GameObject.Find("HealthText")?.GetComponent<Text>();
        }
        if (damageText == null)
        {
            damageText = GameObject.Find("DamageText")?.GetComponent<Text>();
        }
        
        UpdateHealthUI();
        InitializeEnemy();
    }
    
    protected virtual void InitializeEnemy()
    {
        // 子类重写此方法进行特定初始化
    }
    

    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // 显示伤害数字
        ShowDamageText(damage);
        
        // 播放受伤动画
        PlayHitEffect();
        
        // 更新血条
        UpdateHealthUI();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    protected virtual void PlayHitEffect()
    {
        if (enemyImage != null)
        {
            StartCoroutine(HitFlash());
        }
    }
    
    protected IEnumerator HitFlash()
    {
        Color originalColor = enemyImage.color;
        enemyImage.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyImage.color = originalColor;
    }
    
    void ShowDamageText(int damage)
    {
        if (damageText != null)
        {
            damageText.text = $"-{damage}";
            damageText.gameObject.SetActive(true);
            
            // 使用Coroutine实现伤害数字动画
            StartCoroutine(DamageTextAnimation());
        }
    }
    
    IEnumerator DamageTextAnimation()
    {
        Vector3 startPos = damageText.transform.position;
        Vector3 endPos = startPos + Vector3.up * 50f;
        
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            damageText.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            yield return null;
        }
        
        damageText.gameObject.SetActive(false);
    }
    
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
    
    protected virtual void Die()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 触发被动技能
        PassiveSkillManager skillManager = PassiveSkillManager.Instance;
        if (skillManager != null)
        {
            skillManager.TriggerEnemyKilled(this);
        }
        
        // 显示奖励界面
        RewardManager rewardManager = FindObjectOfType<RewardManager>();
        if (rewardManager != null)
        {
            rewardManager.ShowRewards();
        }
        
        StartCoroutine(DeathSequence());
    }
    
    protected virtual IEnumerator DeathSequence()
    {
        // 基础死亡效果：渐隐
        if (enemyImage != null)
        {
            float alpha = 1f;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * 3f;
                Color color = enemyImage.color;
                color.a = alpha;
                enemyImage.color = color;
                yield return null;
            }
        }
        
        gameObject.SetActive(false);
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
}