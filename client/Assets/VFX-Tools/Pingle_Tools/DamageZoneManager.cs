using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZoneManager : MonoBehaviour
{
    [SerializeField] private Renderer dangerZone = null;
    [SerializeField] private Renderer dangerZoneRing = null;
    [SerializeField, Range(0, 0.9f)] private float progress = 0.0f;
    [SerializeField] private float progress_speed = 0.1f;
    [SerializeField] private bool simulate_progress = true;

    private IEnumerator simulate_cor = null;
    private void Start()
    {
      dangerZone
            .sharedMaterial
            .SetFloat("_AlphaMultiplier", 0.70f);
        dangerZoneRing
            .sharedMaterial
            .SetFloat("_AlphaMultiplier", 0.70f);
    }

    private IEnumerator simulateProgress()
    {
      while( true )
      {
        yield return null;
        progress += Time.deltaTime * progress_speed;
        progress = progress > 0.9f ? 0.0f : progress;
      }
    }

    private void Update()
    {
        dangerZone.sharedMaterial.SetFloat("_Progress", progress);
        dangerZoneRing.transform.localScale = new Vector3( 1-progress, dangerZoneRing.transform.localScale.y, 1-progress );
          
        if (!simulate_progress)
        {
            StopAllCoroutines();
            simulate_cor = null;
            return;
        }

        if(simulate_cor == null)
        {
            simulate_cor = simulateProgress();
            StartCoroutine(simulateProgress());
        }
    }
}
