using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CharacterMaterialManager : MonoBehaviour
{
    [SerializeField]
    private Renderer[] renderers = null;

    [SerializeField]
    private MaterialSettingsHolder holder = null;

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
            renderer_material_pairs.Add(
                new RendererMaterialPair(renderer, renderer.sharedMaterial)
            );
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
            renderer_material_pairs.Add(
                new RendererMaterialPair(renderer, renderer.sharedMaterial)
            );
            renderer.sharedMaterial = renderer_material_pairs
                .FirstOrDefault(x => x.renderer == renderer)
                ?.material;
        }
    }

    public void ApplyEffectByKey(MaterialSettingsKey key)
    {
        MaterialSettingsBlock block = holder.getBlockByKey(key);
        foreach (Renderer renderer in renderers)
        {
            block.ApplyToMaterial(renderer.sharedMaterial);
            renderer.material.DOFloat(1, block.controll_property, block.apply_duration);
        }
    }

    public void DeapplyEffectByKey(MaterialSettingsKey key)
    {
        MaterialSettingsBlock block = holder.getBlockByKey(key);
        foreach (Renderer renderer in renderers)
        {
            renderer.material.DOFloat(0, block.controll_property, block.apply_duration);
        }
    }

    public void RemoveInstantiatedEffects(
        Dictionary<ulong, GameObject> effects,
        List<Effect> playerEffects
    )
    {
        int prevEffectsCount = effects.Count;

        foreach (ulong effectId in effects.Keys.ToList())
        {
            if (!playerEffects.Exists(x => x.Id == effectId))
            {
                effects[effectId].GetComponent<PinnedEffectsController>()?.ClearEffects();
                Destroy(effects[effectId]);
                effects.Remove(effectId);
            }
        }
        if (prevEffectsCount != 0 && effects.Count == 0)
        {
            this.renderers[0].material.SetFloat("_FresnelEffectAmount", 0);
            resetMaterial();
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
