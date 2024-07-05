using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLevitatingController : MonoBehaviour
{
    [SerializeField] private AnimationCurve levitating_curve = null;
    [SerializeField] private float levitating_duration = 1.0f;
    private IEnumerator cor = null;
    void Start()
    {
        startLevitating();
    }

    private void startLevitating()
    {
      if (cor != null)
        StopCoroutine(cor);

      cor = impl();
      StartCoroutine(cor);

      IEnumerator impl()
      {
        float cached_duration = 0.0f;
        yield return null;
        Vector3 original_position = this.transform.localPosition;
        Vector3 cached_position = this.transform.localPosition;
        while(true)
        {
          cached_position.y = original_position.y + levitating_curve.Evaluate(cached_duration/levitating_duration);
          this.transform.localPosition = cached_position;
          cached_duration += Time.deltaTime;

          if (cached_duration >= levitating_duration)
            cached_duration = 0.0f;

          yield return null;
        }

      }
    }
}
