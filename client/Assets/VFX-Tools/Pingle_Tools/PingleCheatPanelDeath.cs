using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PingleCheatPanelDeath : MonoBehaviour
{
    [SerializeField] private Renderer char_renderer = null;
    [SerializeField] private Transform vfx_root = null;
    [SerializeField] private GameObject death_vfx = null;
    [SerializeField] private float crystalisation_time = 0.8f;
    [SerializeField] private float explosion_time = 1.2f;

    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        
    }

    private void OnGUI()
  {
      if (GUI.Button(new Rect( 100, 100, 80, 80 ), "spawn"))
          spawnVFX();

  }

  private void spawnVFX()
  {
      StartCoroutine(impl());
      IEnumerator impl()
      {
        char_renderer.material.SetFloat("_DeathProgress", 0);
        char_renderer.material.SetFloat("_ExplosionProgress", 0);
        yield return new WaitForSeconds(1.0f);


        char_renderer.material.DOFloat(1, "_DeathProgress", crystalisation_time);
        yield return new WaitForSeconds(crystalisation_time);

        GameObject cached_vfx = null;
        if (death_vfx != null)
        {
            cached_vfx = Instantiate(death_vfx, vfx_root);
            pool.Add(cached_vfx);
        }

        char_renderer.material.DOFloat(1, "_ExplosionProgress", explosion_time);
      }
  }
}
