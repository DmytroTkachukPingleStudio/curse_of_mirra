using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMuflusTest2 : MonoBehaviour
{
    [SerializeField] private Transform rotation_root = null;
    [SerializeField] private Transform dir_root = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dir_root.LookAt(rotation_root);
    }
}
