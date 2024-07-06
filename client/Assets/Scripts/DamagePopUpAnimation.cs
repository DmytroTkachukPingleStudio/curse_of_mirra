using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamagePopUpAnimation : MonoBehaviour
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public float moveY = 1.5f;
    public float moveX = 1f;
    
    void Start()
    {
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation() {
        // Get random tareget position
        float[] targetsX = {-moveX, 0f, moveX};
        System.Random random = new System.Random();
        var indexX = random.Next(0, targetsX.Length);

        // Move popUp to target position
        Vector2 targetPosition = new Vector2(rectTransform.anchoredPosition.x + targetsX[indexX], rectTransform.anchoredPosition.y + moveY);
        rectTransform.DOAnchorPos(targetPosition, 1f);
        
        // Scale popUp
        rectTransform.DOScale(1.5f, .2f);
        yield return new WaitForSeconds(.2f);
        rectTransform.DOScale(.3f, .6f); 
    }

}
