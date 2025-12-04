using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bat : Enemy
{
    private Sprite[] flySprites;
    private Sprite[] deathSprites;
    private Sprite[] hitSprites;
    
    protected override void InitializeEnemy()
    {
        LoadBatAnimations();
        StartIdleAnimation();
    }
    
    void LoadBatAnimations()
    {
        Sprite[] allBatSprites = Resources.LoadAll<Sprite>("Monster/Bat");
        
        List<Sprite> flyList = new List<Sprite>();
        List<Sprite> deathList = new List<Sprite>();
        
        List<Sprite> hitList = new List<Sprite>();
        
        foreach (Sprite sprite in allBatSprites)
        {
            if (sprite.name.Contains("Bat_Fly"))
                flyList.Add(sprite);
            else if (sprite.name.Contains("Bat_Death"))
                deathList.Add(sprite);
            else if (sprite.name.Contains("Bat_Hit"))
                hitList.Add(sprite);
        }
        
        flyList.Sort((x, y) => x.name.CompareTo(y.name));
        deathList.Sort((x, y) => x.name.CompareTo(y.name));
        hitList.Sort((x, y) => x.name.CompareTo(y.name));
        
        flySprites = flyList.ToArray();
        deathSprites = deathList.ToArray();
        hitSprites = hitList.ToArray();
    }
    
    void StartIdleAnimation()
    {
        if (flySprites != null && flySprites.Length > 0 && enemyImage != null)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(PlayFlyAnimation());
        }
    }
    
    IEnumerator PlayFlyAnimation()
    {
        int frameIndex = 0;
        
        while (true)
        {
            if (flySprites != null && flySprites.Length > 0)
            {
                enemyImage.sprite = flySprites[frameIndex];
                frameIndex = (frameIndex + 1) % flySprites.Length;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    protected override IEnumerator DeathSequence()
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
        
        // 渐隐效果
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
        

        
        // 通知GameManager生成下一个敌人
        SolitaireGameManager gameManager = FindObjectOfType<SolitaireGameManager>();
        if (gameManager != null)
        {

            gameManager.SpawnNextEnemy();
        }
        else
        {

        }
        

        gameObject.SetActive(false);
    }
    
    protected override void PlayHitEffect()
    {
        if (hitSprites != null && hitSprites.Length > 0)
        {
            StartCoroutine(PlayHitAnimation());
        }
        else
        {
            base.PlayHitEffect(); // 回退到基础变色效果
        }
    }
    
    IEnumerator PlayHitAnimation()
    {
        // 停止飞行动画
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 播放受伤动画
        for (int i = 0; i < hitSprites.Length; i++)
        {
            enemyImage.sprite = hitSprites[i];
            yield return new WaitForSeconds(0.05f);
        }
        
        // 恢复飞行动画
        currentAnimation = StartCoroutine(PlayFlyAnimation());
    }
}