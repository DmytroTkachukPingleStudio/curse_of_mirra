using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingleCheatPanelWalkingDust : MonoBehaviour
{
    [SerializeField] private Animator character = null;
    [SerializeField] private Transform root = null;
    [SerializeField] private Transform point1 = null;
    [SerializeField] private float speed = 0.01f;

    private GameObject cached_go = null;


    private void OnGUI()
    {
        if (GUI.Button(new Rect( 100, 100, 80, 80 ), "init"))
            init();

    }

  private void init()
  {
    character.SetTrigger("Walking");
    root.position = point1.position;
    StopAllCoroutines();

    StartCoroutine(impl());

    IEnumerator impl()
    {
      Vector3 cached_vector = Vector3.zero;

      while(true)
      {
        yield return null;
        cached_vector = root.position;
        cached_vector.x += Time.deltaTime * speed;
        root.position = cached_vector;
      }
    }
  }
}
