using UnityEngine;
using System.Collections;

public class CardAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    
    public void AnimateCardDraw()
    {
        StartCoroutine(ScaleAnimation(Vector3.zero, Vector3.one, animationDuration));
    }
    
    public void AnimateCardHover()
    {
        StartCoroutine(ScaleAnimation(transform.localScale, Vector3.one * 1.1f, 0.2f));
    }
    
    public void AnimateCardUnhover()
    {
        StartCoroutine(ScaleAnimation(transform.localScale, Vector3.one, 0.2f));
    }
    
    private IEnumerator ScaleAnimation(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localScale = to;
    }
    
    void OnMouseEnter()
    {
        AnimateCardHover();
    }
    
    void OnMouseExit()
    {
        AnimateCardUnhover();
    }
}