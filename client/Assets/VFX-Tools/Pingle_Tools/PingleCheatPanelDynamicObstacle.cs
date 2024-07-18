using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingleCheatPanelDynamicObstacle : MonoBehaviour
{
    [SerializeField] private SpikesContainer spikesContainer = null;
    [SerializeField] private Transform spawn_root = null;
    [SerializeField] private GameObject start_vfx = null;
    [SerializeField] private GameObject stop_vfx = null;
    [SerializeField] private GameObject idle_vfx = null;
    [SerializeField] private float despawn_delay = 1.0f;

    private List<GameObject> pool = new List<GameObject>();
    private GameObject idle_inst = null;
    
    private void clearPool()
    {
        foreach(GameObject go in pool)
            Destroy(go);

        pool.Clear();
    }

    private void OnGUI()
    {
        if ( GUI.Button(new Rect( 100, 100, 80, 80 ), "start") )
            startObstacle();

        if ( GUI.Button(new Rect( 100, 200, 80, 80 ), "stop") )
            stopObstacle();
    }

    private void startObstacle()
    {
      spikesContainer.ToggleSpikes(true);
      clearPool();
      GameObject cached_vfx = null;
      cached_vfx = Instantiate(start_vfx, spikesContainer.transform);
      pool.Add( cached_vfx );

      idle_inst = Instantiate(idle_vfx, spikesContainer.transform);
      pool.Add( idle_inst );
    }

    private void stopObstacle()
    {
      spikesContainer.ToggleSpikes(false);
      GameObject cached_vfx = null;

      cached_vfx = Instantiate(stop_vfx, spikesContainer.transform);
      pool.Add( cached_vfx );
      StartCoroutine(impl());

      IEnumerator impl()
      {
        yield return new WaitForSeconds(despawn_delay);
        Destroy(idle_inst);
      }
    }
}
