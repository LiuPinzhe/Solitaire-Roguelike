using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] private Image enemyImage;
    private Sprite[] flySprites;
    private Sprite[] deathSprites;
    private Coroutine currentAnimation;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // 如果enemyImage没有手动赋值，自动查找
        if (enemyImage == null)
        {
            enemyImage = GetComponent<Image>();
            Debug.Log($"[Enemy] Auto-found enemyImage: {enemyImage != null}");
        }
        
        UpdateHealthUI();
        LoadAnimations();
        StartIdleAnimation();
    }
    
    void LoadAnimations()
    {
        Sprite[] allBatSprites = Resources.LoadAll<Sprite>("Monster/Bat");
        
        List<Sprite> flyList = new List<Sprite>();
        List<Sprite> deathList = new List<Sprite>();
        
        foreach (Sprite sprite in allBatSprites)
        {
            if (sprite.name.Contains("Bat_Fly"))
                flyList.Add(sprite);
            else if (sprite.name.Contains("Bat_Death"))
                deathList.Add(sprite);
        }
        
        flyList.Sort((x, y) => x.name.CompareTo(y.name));
        deathList.Sort((x, y) => x.name.CompareTo(y.name));
        
        flySprites = flyList.ToArray();
        deathSprites = deathList.ToArray();
        
        Debug.Log($"[Enemy] Loaded {flySprites.Length} fly sprites, {deathSprites.Length} death sprites");
    }
    
    void StartIdleAnimation()
    {
        Debug.Log($"[Enemy] StartIdleAnimation called. flySprites: {flySprites?.Length}, enemyImage: {enemyImage != null}");
        
        if (flySprites != null && flySprites.Length > 0 && enemyImage != null)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(PlayFlyAnimation());
            Debug.Log($"[Enemy] Started fly animation coroutine");
        }
        else
        {
            Debug.LogWarning($"[Enemy] Cannot start animation - missing components");
        }
    }
    
    System.Collections.IEnumerator PlayFlyAnimation()
    {
        int frameIndex = 0;
        Debug.Log($"[Enemy] PlayFlyAnimation started with {flySprites.Length} frames");
        
        while (true)
        {
            if (flySprites != null && flySprites.Length > 0)
            {
                enemyImage.sprite = flySprites[frameIndex];
                Debug.Log($"[Enemy] Set sprite frame {frameIndex}: {flySprites[frameIndex].name}");
                frameIndex = (frameIndex + 1) % flySprites.Length;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        

        
        // 显示伤害数字
        ShowDamageText(damage);
        
        // 播放受伤动画
        StartCoroutine(HitAnimation());
        
        // 更新血条
        UpdateHealthUI();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }
    
    System.Collections.IEnumerator HitAnimation()
    {
        // 简单的受伤闪烁效果
        if (enemyImage != null)
        {
            Color originalColor = enemyImage.color;
            enemyImage.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyImage.color = originalColor;
        }
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
    
    System.Collections.IEnumerator DamageTextAnimation()
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
    
    void OnDeath()
    {

        
        // 停止当前动画
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 播放死亡动画
        currentAnimation = StartCoroutine(PlayDeathAnimation());
    }
    
    System.Collections.IEnumerator PlayDeathAnimation()
    {
        // 播放死亡动画帧
        if (deathSprites != null && deathSprites.Length > 0)
        {
            for (int i = 0; i < deathSprites.Length; i++)
            {
                enemyImage.sprite = deathSprites[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // 动画播放完毕后渐隐
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
}