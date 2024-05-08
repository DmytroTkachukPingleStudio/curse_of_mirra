using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    Renderer dangerZone;
    [SerializeField]
    Renderer dangerZoneRing;

    void Start()
    {
        dangerZone.sharedMaterial.SetFloat("_AlphaMultiplier", 0f);
        dangerZoneRing.sharedMaterial.SetFloat("_AlphaMultiplier", 0f);
    }

    IEnumerator ShowDangerZone()
    {
        yield return new WaitForSeconds(1f);
        float currentValue = dangerZone.sharedMaterial.GetFloat("_AlphaMultiplier");
        if (currentValue == 0.70f)
            yield break;
        currentValue += 0.05f;
        currentValue = Mathf.Round(currentValue * 100) / 100f;
        dangerZone
            .sharedMaterial
            .SetFloat("_AlphaMultiplier", currentValue);
        dangerZoneRing
            .sharedMaterial
            .SetFloat("_AlphaMultiplier", currentValue);
    }

    void Update()
    {
        // We have a difference of x100 between the backend and frontend values
        float radius = GameServerConnectionManager.Instance.playableRadius / 100;
        bool isZoneEnabled = GameServerConnectionManager.Instance.zoneEnabled;

        if (isZoneEnabled && dangerZone.sharedMaterial.GetFloat("_AlphaMultiplier") != 0.70f)
        {
            StartCoroutine(ShowDangerZone());
        }
        // The vfx goes to 0 to 1.
        // The raidus from the backen goes from 0.552 to 0

        double finalVfxValue = 1;
        double initialPlayableRadius = 0.552;
        float currentRadius = radius / 100;
        double value = finalVfxValue + (currentRadius) * (-finalVfxValue) / (initialPlayableRadius);
        float ring_scale = 1 - (float)value;

        dangerZone.sharedMaterial.SetFloat("_Progress", (float)value);
        dangerZoneRing.transform.localScale = new Vector3( ring_scale, dangerZoneRing.transform.localScale.y, ring_scale );
    }
}
