using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterFeedbackManager : MonoBehaviour
{
    public bool hasTransparentMaterial;

    [SerializeField]
    GameObject feedbacksContainer;

    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    [MMCondition("hasTransparentMaterial", true)]
    [SerializeField]
    Material transparentMaterial;

    [SerializeField]
    List<GameObject> vfxList;

    private Material initialMaterial;

    private Dictionary<ulong, GameObject> InstantiateItems = new Dictionary<ulong, GameObject>();

    void Awake()
    {
        initialMaterial = skinnedMeshRenderer?.material;
    }

    public void ManageStateFeedbacks(Entity playerUpdate, CustomCharacter character)
    {
        CharacterFeedbacks characterFeedbacks = character.GetComponent<CharacterFeedbacks>(); // maybe cache this? We can optimize this later.
        for (int i = 0; i < playerUpdate.Player.Effects.Count; i++)
        {
            var effect = playerUpdate.Player.Effects[i];
            var effectName = effect.Name;
            var effectId = effect.Id;
            var item = characterFeedbacks.SelectGO(effectName);
            if (!InstantiateItems.ContainsKey(effectId) && item != null)
            {
                var vfx = Instantiate(item, feedbacksContainer.transform);
                vfx.name = effectName + " ID " + effectId;
                InstantiateItems.Add(effectId, vfx);
                vfx.GetComponent<PinnedEffectsController>()
                    ?.Setup(character.GetComponent<PinnedEffectsManager>());
                vfx.GetComponent<EffectCharacterMaterialController>()
                    ?.Setup(character.GetComponent<CharacterMaterialManager>());
            }
        }
        // Refacor this to a single metho to handle effects.

        if (skinnedMeshRenderer != null && transparentMaterial != null)
        {
            if (hasEffect(playerUpdate.Player.Effects, "invisible"))
            {
                if (
                    skinnedMeshRenderer.materials[0].HasProperty("_ISTRANSPARENT")
                    && skinnedMeshRenderer.materials[0].GetInt("_ISTRANSPARENT") == 0
                )
                {
                    HandleInvisible(playerUpdate.Id, character);
                }
            }
            else
            {
                if (skinnedMeshRenderer.materials[0].HasProperty("_ISTRANSPARENT"))
                {
                    if (skinnedMeshRenderer.materials[0].GetInt("_ISTRANSPARENT") == 1)
                    {
                        skinnedMeshRenderer.materials[0] = initialMaterial;
                        SetMaterial(initialMaterial);

                        var canvasHolder = character.characterBase.CanvasHolder;
                        canvasHolder.GetComponent<CanvasGroup>().alpha = 1;
                        SetMeshes(true, character);
                        vfxList.ForEach(el => el.SetActive(true));
                        feedbacksContainer.SetActive(true);
                        character.characterBase.characterShadow.SetActive(true);
                        skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.On;
                    }
                }
            }
        }
        character
            .GetComponent<CharacterMaterialManager>()
            .RemoveInstantiatedEffects(InstantiateItems, playerUpdate.Player.Effects.ToList());
    }

    private void HandleInvisible(ulong id, CustomCharacter character)
    {
        bool isClient = GameServerConnectionManager.Instance.playerId == id;
        float alpha = isClient ? 0.5f : 0;
        transparentMaterial.SetFloat("_AlphaValue", alpha);
        transparentMaterial.SetInt("_ISTRANSPARENT", 1);

        if (!isClient)
        {
            var canvasHolder = character.characterBase.CanvasHolder;
            canvasHolder.GetComponent<CanvasGroup>().alpha = 0;
            vfxList.ForEach(el => el.SetActive(false));
            feedbacksContainer.SetActive(false);
            character.characterBase.characterShadow.SetActive(false);
            skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        SetMaterial(transparentMaterial);
    }

    // Will use this later when delay is implemented and improve code
    private void HandleCharacterCard(CustomCharacter character, bool visible)
    {
        var canvasHolder = character.characterBase.CanvasHolder;
        canvasHolder.GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
        character.characterBase.PlayerName.GetComponent<MeshRenderer>().enabled = visible;
    }

    private void SetMeshes(bool isActive, CustomCharacter character)
    {
        List<MeshRenderer> meshes = character
            .characterBase
            .CharacterCard
            .GetComponentsInChildren<MeshRenderer>()
            .ToList();
    }

    private bool hasEffect(ICollection<Effect> effects, string effectName)
    {
        foreach (var effect in effects)
        {
            if (effect.Name == effectName)
            {
                return true;
            }
        }
        return false;
    }

    public void HandlePickUpItemFeedback(Entity playerUpdate, CharacterFeedbacks characterFeedbacks)
    {
        if (playerUpdate.Player.Inventory != null && !characterFeedbacks.DidPickUp())
        {
            characterFeedbacks.ExecutePickUpItemFeedback(true);
        }
        else if (playerUpdate.Player.Inventory == null && characterFeedbacks.DidPickUp())
        {
            characterFeedbacks.ExecutePickUpItemFeedback(false);
        }
    }

    void SetMaterial(Material material)
    {
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0] = material;
        skinnedMeshRenderer.materials = mats;
    }
}
