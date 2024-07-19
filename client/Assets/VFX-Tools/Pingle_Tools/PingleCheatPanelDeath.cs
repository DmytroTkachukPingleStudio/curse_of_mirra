using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class PingleCheatPanelDeath : MonoBehaviour
{
    [SerializeField] private Animator char_animator = null;
    [SerializeField] private Renderer char_renderer = null;
    [SerializeField] private Transform vfx_root = null;
    [SerializeField] private GameObject death_vfx = null;
    [SerializeField] private GameObject death_vfx2 = null;
    [SerializeField] private float crystalisation_time = 0.8f;
    [SerializeField] private float crystalisation_offset = 0.5f;
    [SerializeField] private float explosion_time = 1.2f;
    [SerializeField] private float explosion_offset = 1.2f;
    [SerializeField] private int slow_time = 30;

    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        
    }

    private void OnGUI()
  {
      if (GUI.Button(new Rect( 100, 100, 80, 80 ), "spawn"))
          spawnVFX();

      if (GUI.Button(new Rect( 100, 200, 80, 80 ), "spawn2"))
          spawnVFX2();
  }

  private void spawnVFX()
  {
      StartCoroutine(impl());
      IEnumerator impl()
      {
        char_renderer.material.EnableKeyword("_USE_DEATH");
        char_renderer.material.SetFloat("_DeathProgress", 0);
        char_renderer.material.SetFloat("_ExplosionProgress", 0);
        yield return new WaitForSeconds(1.0f);


        char_renderer.material.DOFloat(1, "_DeathProgress", crystalisation_time);
        yield return new WaitForSeconds(crystalisation_time + crystalisation_offset);

        GameObject cached_vfx = null;
        if (death_vfx != null)
        {
            cached_vfx = Instantiate(death_vfx, vfx_root);
            pool.Add(cached_vfx);
        }

        char_renderer.material.DOFloat(1, "_ExplosionProgress", explosion_time);
      }
  }

  private void spawnVFX2()
  {
      StartCoroutine(impl());
      IEnumerator impl()
      {
        char_renderer.material.EnableKeyword("_USE_DEATH");
        char_renderer.material.SetFloat("_DeathProgress", 0);
        char_renderer.material.SetFloat("_ExplosionProgress", 0);
        char_animator.speed = 1;
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(crystalisation_time);

        death_vfx2.GetComponent<VisualEffect>().Play();

        for(int i = slow_time; i > 0; i--)
        {
          char_animator.speed = (float)i / slow_time;
          yield return null;
        }

        yield return new WaitForSeconds(explosion_offset);

        char_renderer.material.DOFloat(1, "_ExplosionProgress", explosion_time);

        yield return new WaitForSeconds(crystalisation_offset);
        char_renderer.material.SetFloat("_ExplosionProgress", 0);
      }
  }
}
