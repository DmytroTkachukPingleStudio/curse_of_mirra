using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BountyCompletionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popUpPrefab;
    private CanvasGroup bountyCompletedCG;

    void Start() 
    {
        bountyCompletedCG = GetComponent<CanvasGroup>();
    }

    public void GenerateBountyCompletionPopUp() 
    {
        GameObject popUp = Instantiate(popUpPrefab, transform);
        RectTransform popUpRectTransform = popUp.GetComponent<RectTransform>();

        Vector2 finalPosition = popUpRectTransform.anchoredPosition;
        popUpRectTransform.anchoredPosition = new Vector2(300f, popUpRectTransform.anchoredPosition.y);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(popUpRectTransform.DOAnchorPos(finalPosition, 0.5f))
                .Join(bountyCompletedCG.DOFade(1f, 0.7f))
                .AppendInterval(2f)
                .Append(bountyCompletedCG.DOFade(0f, 1f))
                .OnComplete(() => Destroy(popUp));

        sequence.Play();
    }

}
