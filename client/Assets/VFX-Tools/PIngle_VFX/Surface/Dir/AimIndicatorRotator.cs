using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIndicatorRotator : MonoBehaviour
{
    [SerializeField] private Transform rotation_root = null;
    [SerializeField] private Transform aim_root = null;
    void Update()
    {
        rotation_root.LookAt(aim_root);
    }
}
