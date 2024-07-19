using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingleCheatPanelChest : MonoBehaviour
{
    [SerializeField] private Animator chest_animator = null;
    [SerializeField] private Renderer chest_renderer = null;
    [SerializeField] private GameObject open_vfx = null;
    [SerializeField] private GameObject idle_vfx = null;
    [SerializeField] private float clear_offset = 0.3f;
    [SerializeField] private float dissolve_offset = 0.3f;

    private GameObject idle_vfx_inst = null;
    private List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        resetAnimation();
    }

    private void OnGUI()
    {
        if ( GUI.Button(new Rect( 100, 100, 80, 80 ), "open") )
            animateOpen();

        if ( GUI.Button(new Rect( 200, 100, 80, 80 ), "reset") )
            resetAnimation();
    }

    private void clearPool()
    {
        foreach(GameObject go in pool)
            Destroy(go);

        pool.Clear();
    }

    private void animateOpen()
    {
        chest_animator.SetTrigger("Open");
        chest_animator.ResetTrigger("Idle");
        
        GameObject cached_vfx = null;

        if( open_vfx != null )
            cached_vfx = Instantiate(open_vfx, chest_animator.transform.parent);

        pool.Add(cached_vfx);

        StartCoroutine(impl());

        IEnumerator impl()
        {
          yield return new WaitForSeconds(clear_offset);
          Destroy(idle_vfx_inst);
          yield return new WaitForSeconds(dissolve_offset);
          chest_renderer.enabled = false;
        }
    }

    private void resetAnimation()
    {
        clearPool();
        chest_renderer.enabled = true;
        chest_animator.SetTrigger("Idle");
        chest_animator.ResetTrigger("Open");

        if( idle_vfx != null )
            idle_vfx_inst = Instantiate(idle_vfx, chest_animator.transform.parent);

        pool.Add(idle_vfx_inst);
    }
}
