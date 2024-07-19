using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingleCheatPanelDeath : MonoBehaviour
{
    [SerializeField] private Transform vfx_root = null;
    [SerializeField] private GameObject death_vfx = null;

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
      GameObject cached_vfx = null;
      if (death_vfx != null)
        {
            cached_vfx = Instantiate(death_vfx, vfx_root);
            pool.Add(cached_vfx);
        }
  }
}
