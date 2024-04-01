using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class PingleCheatPanelItems : MonoBehaviour
{
    public MaterialSettingsManager material_manager = null;
    public MaterialSettingsKey golden_clock_key = null;
    public MaterialSettingsKey mirras_blessing_key = null;
    public MaterialSettingsKey magic_boots_key = null;

    public GameObject golden_clock_vfx = null;
    public GameObject mirras_blessing_vfx = null;
    public GameObject magic_boots_vfx = null;
    public Transform compiler_root = null;

    public float start_delay = 0.4f;
    public float duration_01 = 0.4f;

    public Shader new_character_shader = null;

    private List<Character> character_instances = null;
    private List<GameObject> pool = new List<GameObject>();
    private const float ARENA_SIZE = 40.0f;
    private bool graphs_warmed = false;
    private HashSet<Material> mats = new HashSet<Material>();

    public void init()
    {
        //Application.targetFrameRate = 60;
        character_instances = FindObjectsOfType<Character>().ToList();
        clearPool();

        foreach( Renderer render in FindObjectsOfType<Renderer>() )
        {
          if ( render.sharedMaterial == null )
            continue;

          if ( render.sharedMaterial.shader == null )
            continue;

          if ( render.sharedMaterial.shader != new_character_shader )
            continue;

          mats.Add( render.sharedMaterial );
        }

        resetAnims();
        if ( !graphs_warmed )
          warmUpGraphs();
    }

    private void resetAnims()
    {
        foreach(Material mat in mats )
            mat.SetFloat("_FresnelEffectAmount", 0);

        foreach(Character character in character_instances)
        {
            character.CharacterAnimator.ResetTrigger("Skill1");
            character.CharacterAnimator.ResetTrigger("Skill2");
            character.CharacterAnimator.ResetTrigger("Skill3");
            character.CharacterAnimator.SetTrigger("Walking");
            character.CharacterAnimator.ResetTrigger("Walking");
        }
    }

    private void clearPool()
    {
        foreach(GameObject go in pool)
            Destroy(go);

        pool.Clear();
    }

    private void OnGUI()
    {
        if ( GUI.Button(new Rect( 100, 100, 80, 80 ), "init") )
            init();

        if ( GUI.Button(new Rect( 200, 100, 80, 80 ), "golden_clock") )
        {
            activateItemVFX(golden_clock_vfx);
            activateFresnel( golden_clock_key );
        }

        if ( GUI.Button(new Rect( 300, 100, 80, 80 ), "mirras_blessing") )
        {
            activateItemVFX(mirras_blessing_vfx);
            activateFresnel( mirras_blessing_key );
        }

        if ( GUI.Button(new Rect( 400, 100, 80, 80 ), "magic_boots") )
        {
            activateItemVFX(magic_boots_vfx);
            activateFresnel( magic_boots_key );
        }
    }

    private void activateItemVFX(GameObject vfx)
    {
        resetAnims();
        clearPool();

        if ( vfx == null )
          return;

        GameObject cached_vfx = null;
        foreach(Character character in character_instances)
        {
            cached_vfx = Instantiate(vfx, character.transform);
            pool.Add( cached_vfx );
        }
    }

    private void activateFresnel( MaterialSettingsKey key )
    {
      MaterialSettingsBlock block = material_manager.getBlockByKey( key );
      foreach(Material mat in mats )
        block.apllyToMaterial( mat );

      StartCoroutine(impl());
      IEnumerator impl()
      {
        float cached_duration = 0.0f;
        yield return new WaitForSeconds(start_delay);

        while ( cached_duration <= duration_01 )
        {
          foreach(Material mat in mats )
            mat.SetFloat("_FresnelEffectAmount", cached_duration / duration_01);

          Shader.SetGlobalFloat("_FresnelEffectAmount", cached_duration / duration_01);

          cached_duration += Time.deltaTime;
          yield return null;
        }
        foreach(Material mat in mats )
            mat.SetFloat("_FresnelEffectAmount", 1);
      }
    }

    private void warmUpGraphs()
    {
        //pool.Add( Instantiate(golden_clock_vfx, compiler_root) );
        //pool.Add( Instantiate(mirras_blessing_vfx, compiler_root) );
        //pool.Add( Instantiate(magic_boots_vfx, compiler_root) );
    }
}