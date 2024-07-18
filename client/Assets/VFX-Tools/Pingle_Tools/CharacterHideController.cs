using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHideController : MonoBehaviour
{
    [SerializeField] private CharacterMaterialManager characterMaterialManager = null;
    [SerializeField] private MaterialSettingsKey hideKey = null;

    private bool is_hiden = false;
    private int hide_frames_count = 0;
    private int HIDE_TRANSITION_DURATION = 30;
    private IEnumerator hide_cor = null;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!is_hiden)
        {
          hide_frames_count = HIDE_TRANSITION_DURATION;
          hideCoroutine();
        }
        
        is_hiden = true;
        hide_frames_count++;
        Debug.Log("stay " + hide_frames_count);
    }

    private void hideCoroutine()
    {
        if (is_hiden)
          return;

        hide_cor = impl();

        StartCoroutine(hide_cor);
        
        IEnumerator impl()
        {
          characterMaterialManager.ApplyEffectByKey(hideKey);
          while(hide_frames_count > 0)
          {
              yield return null;
              hide_frames_count--;
              Debug.Log("stay " + hide_frames_count);
          }
          characterMaterialManager.DeapplyEffectByKey(hideKey);
          is_hiden = false;
          hide_cor = null;
        }
    }
}
