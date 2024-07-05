using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMaterialManager : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers = null;
    [SerializeField] private MaterialSettingsHolder holder = null;

    private List<RendererMaterialPair> renderer_material_pairs = new List<RendererMaterialPair>();
    private void Start()
    {
        init();
    }

    private void OnDestroy()
    {
        deinit();
    }

    public void init()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer_material_pairs.Add(new RendererMaterialPair(renderer, renderer.sharedMaterial));
        }
    }

    public void deinit()
    {
        renderer_material_pairs.Clear();
    }

    public void setMaterial(Material material)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            renderer.sharedMaterial = material;
        }
    }

    public void resetMaterial()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer_material_pairs.Add(new RendererMaterialPair(renderer, renderer.sharedMaterial));
            renderer.sharedMaterial = renderer_material_pairs.FirstOrDefault(x => x.renderer == renderer)?.material;
        }
    }

    public IEnumerator applyEffectByKey(MaterialSettingsKey key)
    {
        resetMaterial();
        MaterialSettingsBlock block = holder.getBlockByKey(key);

        foreach (Renderer renderer in renderers)
            block.applyToMaterial(renderer.sharedMaterial);

        float curent_duration = 0.0f;
        while (curent_duration <= block.apply_duration)
        {
            foreach (Renderer renderer in renderers)
                renderer.material.SetFloat(block.controll_property, curent_duration / block.apply_duration);

            curent_duration += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer renderer in renderers)
            renderer.material.SetFloat(block.controll_property, 1.0f);
    }

    public IEnumerator deapplyEffectByKey(MaterialSettingsKey key)
    {
        resetMaterial();
        MaterialSettingsBlock block = holder.getBlockByKey(key);

        float curent_duration = 0.0f;
        while (curent_duration <= block.apply_duration)
        {
            foreach (Renderer renderer in renderers)
                renderer.material.SetFloat(block.controll_property, 1.0f - curent_duration / block.apply_duration);

            curent_duration += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer renderer in renderers)
            renderer.material.SetFloat(block.controll_property, 0.0f);
    }

    public void RemoveInstantiatedEffects(Dictionary<ulong, GameObject> effects, List<Effect> playerEffects)
    {
        foreach (ulong effectId in effects.Keys.ToList())
        {
            // key not found exception
            if (!playerEffects.Exists(x => x.Id == effectId))
            {
                this.renderers[0].sharedMaterial.SetFloat("_FresnelEffectAmount", 0);
                PinnedEffectsController controller = effects[effectId].GetComponent<PinnedEffectsController>();
                controller?.ClearEffects();
                Destroy(effects[effectId]);
                effects.Remove(effectId);
            }
        }
    }
}

public class RendererMaterialPair
{
    public Renderer renderer = null;
    public Material material = null;

    public RendererMaterialPair(Renderer renderer, Material material)
    {
        this.renderer = renderer;
        this.material = material;
    }
}
