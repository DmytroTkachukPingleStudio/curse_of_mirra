using System.Collections;
using UnityEngine;

public class EffectCharacterMaterialController : MonoBehaviour
{
    [SerializeField] private Material character_material = null;
    [SerializeField] public MaterialSettingsKey material_settings_key = null;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private EffectMaterialControllerType controller_type = EffectMaterialControllerType.MATERIAL;

    private CharacterMaterialManager character_material_manager = null;
    
    ///TEST CODE! REMOVE BEFORE MERGE
    private void Start()
    {
      Setup(GetComponentInParent<CharacterMaterialManager>());
    }
    ///

    public void Setup(CharacterMaterialManager materialManager)
    {
        character_material_manager = materialManager;
        switch (controller_type)
        {
            case EffectMaterialControllerType.MATERIAL: StartCoroutine(switchMaterialForTime()); break;
            case EffectMaterialControllerType.PROPERTIES: ApplyEffectProperties(); break;
        }
    }

    private void OnDestroy()
    {
        deinit();
    }

    public void init()
    {
        character_material_manager?.setMaterial(character_material);
    }

    public void deinit()
    {
        character_material_manager?.resetMaterial();
    }

    private IEnumerator switchMaterialForTime()
    {
        init();
        yield return new WaitForSeconds(duration);
        deinit();
    }

    private void ApplyEffectProperties()
    {
        character_material_manager?.ApplyEffectByKey(material_settings_key);
        ///TEST CODE! REMOVE BEFORE MERGE
        StartCoroutine(impl());

        IEnumerator impl()
        {
            yield return new WaitForSeconds(duration);
            character_material_manager?.DeapplyEffectByKey(material_settings_key);
        }
        ///
    }
}

public enum EffectMaterialControllerType
{
    NONE = 0,
    MATERIAL = 1,
    PROPERTIES = 2
}
