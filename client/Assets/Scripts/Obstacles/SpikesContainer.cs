using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesContainer : MonoBehaviour
{
    [SerializeField]
    List<Animator> spikesAnimators;

    [SerializeField]
    MeshRenderer indicatorMesh;

    public float fadeInDuration = .5f;
    public float fadeInPeakDuration = .5f;
    public float flickeringDuration = 2f;
    public float fadeOutDuration = .5f;

    public void ToggleSpikes(bool state)
    {
        foreach(Animator spikeAnimator in spikesAnimators)
        {
            spikeAnimator.SetBool("Raised", state);
        }

        if(state)
        {
            StartCoroutine(FadeOutIndicator());
        }
    }

    private IEnumerator FadeOutIndicator()
    {
        float elapsedTime = 0f;
        Color tempColor = indicatorMesh.material.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            tempColor.a = Mathf.Clamp01(1 - (elapsedTime / fadeOutDuration));
            indicatorMesh.material.color = tempColor;
            yield return null;
        }

        tempColor.a = 0;
        indicatorMesh.material.color = tempColor;
        indicatorMesh.enabled = false;
    }

    public void TransitionIndicator()
    {
        indicatorMesh.gameObject.SetActive(true);
        StartCoroutine(FadeInIndicator());
    }

    private IEnumerator FadeInIndicator()
    {
        float elapsedTime = 0f;
        Color tempColor = indicatorMesh.material.color;
        float showInidicatorTotalDuration = fadeInDuration + fadeInPeakDuration + flickeringDuration;

        tempColor.a = Mathf.Clamp01(elapsedTime / showInidicatorTotalDuration);
        indicatorMesh.material.color = tempColor;
        indicatorMesh.enabled = true;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            tempColor.a = Mathf.Clamp01(elapsedTime / fadeInDuration / 1.5f);
            indicatorMesh.material.color = tempColor;
            yield return null;
        }

        tempColor.a = 1 / 1.5f;
        indicatorMesh.material.color = tempColor;

        yield return new WaitForSeconds(fadeInPeakDuration);
        elapsedTime += fadeInPeakDuration;

        bool isActive = true;
        float flickerInterval = 0.25f;

        while(elapsedTime < showInidicatorTotalDuration)
        {
            isActive = !isActive;
            tempColor.a = isActive ? (1 / 1.5f) : 0.3f; 
            indicatorMesh.material.color = tempColor;
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
            flickerInterval = Mathf.Lerp(0.25f, 0.05f, (elapsedTime - (fadeInDuration + fadeInPeakDuration)) / (showInidicatorTotalDuration - (fadeInDuration + fadeInPeakDuration)));
        }
    }
}
