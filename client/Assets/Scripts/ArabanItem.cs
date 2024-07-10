using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArabanItem : MonoBehaviour
{
    [SerializeField]
    private Transform vfxRoot;

    [SerializeField]
    private GameObject[] vfxList;
    private List<GameObject> vfxPool = new List<GameObject>();

    public void TriggerVFX(int vfxId)
    {
        ClearPool();
        GameObject cached_vfx = Instantiate(vfxList[vfxId], vfxRoot);
        vfxPool.Add(cached_vfx);
    }

    private void ClearPool()
    {
        foreach (GameObject go in vfxPool)
            Destroy(go);

        vfxPool.Clear();
    }
}
