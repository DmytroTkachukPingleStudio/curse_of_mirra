using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMuflusTest : MonoBehaviour
{
    [SerializeField] private Transform aim_disk = null;
    [SerializeField] private Transform arms_root = null;
    [SerializeField] private Transform arms_left = null;
    [SerializeField] private Transform arms_right = null;
    [SerializeField] private Transform arms_middle = null;
    [SerializeField] private Material material_0 = null;
    [SerializeField] private Material material_1 = null;
    [SerializeField] private float max_distance = 15.0f;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      float current_distance = Vector3.Distance( Vector3.zero, aim_disk.localPosition );
      material_0.SetFloat( "_AlphaPercent", current_distance/max_distance );
      material_1.SetFloat( "_AlphaPercent", current_distance/max_distance );

      float current_radius = aim_disk.localScale.z;

      Vector3 cached_vec = arms_left.localPosition;
      cached_vec.x = -current_radius;
      arms_left.localPosition = cached_vec;

      cached_vec = arms_right.localPosition;
      cached_vec.x = current_radius;
      arms_right.localPosition = cached_vec;

      cached_vec = arms_middle.localScale;
      cached_vec.x = current_radius;
      arms_middle.localScale = cached_vec;

      arms_root.LookAt(aim_disk);
    }
}
