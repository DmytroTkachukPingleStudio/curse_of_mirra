using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIndicatorRotator : MonoBehaviour
{
    [SerializeField] private Transform rotation_root = null;
    [SerializeField] private Transform aim_root = null;
    [SerializeField] private Transform aim_center_root = null;

    [SerializeField] private Transform h4ck_left_dir = null;
    [SerializeField] private Transform h4ck_mid_dir = null;
    [SerializeField] private Transform h4ck_right_dir = null;
    [SerializeField] private float h4ck_angle = 30.0f;
    


    private float scale = 0.0f;
    void Update()
    {
        rotation_root.LookAt(aim_root);
        scale = Mathf.Max(1.0f, aim_root.localScale.x/2.0f) * 2;

        aim_center_root.localScale = new Vector3(scale,scale,scale);

        h4ck_left_dir. localRotation = Quaternion.Euler( 0.0f, -h4ck_angle/2, 0.0f );
        h4ck_right_dir.localRotation = Quaternion.Euler( 0.0f, h4ck_angle/2, 0.0f );
    }
}
