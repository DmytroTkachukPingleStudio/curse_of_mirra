using System.Collections.Generic;
using CandyCoded.HapticFeedback;
using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public enum HapticFeedbackType
{
    Light,
    Heavy
}

public class CharacterFeedbacks : MonoBehaviour
{
    [Header("Setup")]
    public GameObject characterModelBody;

    [Header("Feedbacks")]
    [SerializeField]
    List<GameObject> feedbacksStatesPrefabs;

    [SerializeField]
    GameObject deathFeedback,
        damageFeedback,
        healFeedback,
        hitFeedback,
        pickUpFeedback,
        useItemFeedback;

    [SerializeField]
    GameObject goldenClockVFX,
        magicBootsVFX,
        myrrasBlessingVFX,
        giantFruitVFX;

    [SerializeField]
    HealthBarItem healthBar;

    [SerializeField]
    DamagePopUpGenerator damagePopUpGenerator;

    [SerializeField]
    CustomCharacter customCharacter;

    private float previousPlayerRadius;
    private bool didPickUp = false;
    private ulong playerID;
    private Material characterMaterial;
    private Animator modelAnimator;
    private float overlayMultiplier = 0f;
    private float overlayEffectSpeed = 3f;
    GameObject aimDirection;
    float initialPlayerScale,
        initialSkillIndicatorScale;

    // didPickUp value should ideally come from backend
    public bool DidPickUp()
    {
        return didPickUp;
    }

    void Start()
    {
        playerID = GameServerConnectionManager.Instance.playerId;
        characterMaterial = characterModelBody.GetComponent<SkinnedMeshRenderer>().materials[0];
        modelAnimator = GetComponent<CustomCharacter>().CharacterModel.GetComponent<Animator>();
        initialPlayerScale = this.transform.localScale.x;
        aimDirection = GetComponent<CustomCharacter>().characterBase.AimDirection;
        initialSkillIndicatorScale = aimDirection.transform.localScale.x;
    }

    void Update()
    {
        if (overlayMultiplier > 0)
        {
            overlayMultiplier -= Time.deltaTime * overlayEffectSpeed;
            overlayMultiplier = Mathf.Clamp01(overlayMultiplier);
            characterMaterial.SetFloat("_OverlayColorIntensity", overlayMultiplier);
        }
    }

    private HapticFeedbackType GetHapticTypeByDamage(ulong damage) =>
        damage switch
        {
            < 70 => HapticFeedbackType.Light,
            >= 70 => HapticFeedbackType.Heavy,
        };

    public void SetActiveStateFeedback(string name, bool active)
    {
        GameObject feedbackToActivate = feedbacksStatesPrefabs.Find(el => el.name == name);
        feedbackToActivate?.SetActive(active);
    }

    public List<GameObject> GetFeedbackStateList()
    {
        return feedbacksStatesPrefabs;
    }

    public void PlayDeathFeedback()
    {
        if (characterModelBody.activeSelf == true)
        {
            deathFeedback.SetActive(true);
        }
    }

    public GameObject SelectGO(string name)
    {
        switch (name)
        {
            case "mirra_blessing_effect":
                return myrrasBlessingVFX;
            case "magic_boots_effect":
                return magicBootsVFX;
            case "golden_clock_effect":
                return goldenClockVFX;
            case "giant_effect":
                return giantFruitVFX;
            default:
                return null;
        }
    }

    public void ExecutePickUpItemFeedback(bool state)
    {
        didPickUp = state;
        pickUpFeedback.SetActive(state);
    }

    public void ExecuteHealthFeedback(float clientHealth, float serverPlayerHealth, ulong playerId)
    {
        if (serverPlayerHealth < clientHealth)
        {
            float healthDiff = clientHealth - serverPlayerHealth;

            if (playerId == GameServerConnectionManager.Instance.playerId)
            {
                damageFeedback.GetComponent<MMF_Player>().PlayFeedbacks();
                HapticFeedbackType feedbackType = GetHapticTypeByDamage((ulong)healthDiff);
                TriggerHapticFeedback(feedbackType);
            }
            ApplyDamageOverlay();
            string damageType = GetDamageType(healthDiff);
            damagePopUpGenerator.GenerateDamagePopUp(
                transform.position,
                healthDiff.ToString(),
                damageType
            );
        }
        if (clientHealth < serverPlayerHealth)
        {
            healFeedback.GetComponent<ParticleSystem>().Play();
            if (playerId == GameServerConnectionManager.Instance.playerId)
            {
                TriggerHapticFeedback(HapticFeedbackType.Light);
            }
        }

        healthBar.UpdateHealthBar(serverPlayerHealth);
    }

    public void ApplyDamageOverlay()
    {
        overlayMultiplier = 1f;
        UpdateOverlayColor(overlayMultiplier);
    }

    public void TriggerHapticFeedback(HapticFeedbackType hapticType)
    {
        switch (hapticType)
        {
            case HapticFeedbackType.Heavy:
                HapticFeedback.HeavyFeedback();
                break;
            case HapticFeedbackType.Light:
                HapticFeedback.LightFeedback();
                break;
        }
    }

    private void UpdateOverlayColor(float colorIntensity)
    {
        characterMaterial.SetFloat("_OverlayColorIntensity", colorIntensity);
    }

    public void SetActiveFeedback(GameObject player, string feedbackName, bool value)
    {
        SetActiveStateFeedback(feedbackName, value);
    }

    public void ClearAllFeedbacks(GameObject player)
    {
        GetFeedbackStateList().ForEach(el => el.SetActive(false));
    }

    public void PlayHitFeedback()
    {
        hitFeedback.GetComponent<MMF_Player>().PlayFeedbacks();
        HapticFeedback.LightFeedback();
    }

    private string GetDamageType(float damage)
    {
        float damagePercentage = damage / customCharacter.CharacterHealth.MaximumHealth;

        if (damagePercentage <= 0.15f)
        {
            return "light";
        }

        if (damagePercentage >= 0.3f)
        {
            return "heavy";
        }

        return "medium";
    }

    public void UpdateCharacterScale(float serverPlayerRadius)
    {
        // scale logic
        if (serverPlayerRadius != previousPlayerRadius)
        {
            float radiusDiff = serverPlayerRadius - previousPlayerRadius;

            if (previousPlayerRadius != 0)
            {
                float scaleAnimationDuration = 0.25f;
                // scale animation
                if (radiusDiff > 0)
                {
                    // scale up
                    modelAnimator.SetFloat("Giant_Speed_Multiplier", 0.5f);
                    float playerMultiplier = CalculateScaleMultiplier(
                        serverPlayerRadius,
                        previousPlayerRadius
                    );
                    float indicatorNewScale = CalculateScaleMultiplier(
                        previousPlayerRadius,
                        serverPlayerRadius
                    );
                    float newPlayerScale = initialPlayerScale * playerMultiplier;
                    this.transform.DOScale(
                        new Vector3(newPlayerScale, newPlayerScale, newPlayerScale),
                        scaleAnimationDuration
                    );
                    // scale down skill indicators
                    aimDirection
                        .transform
                        .DOScale(
                            new Vector3(indicatorNewScale, indicatorNewScale, indicatorNewScale),
                            scaleAnimationDuration
                        );
                }
                else
                {
                    // scale down
                    modelAnimator.SetFloat("Giant_Speed_Multiplier", 1f);
                    this.transform.DOScale(
                        new Vector3(initialPlayerScale, initialPlayerScale, initialPlayerScale),
                        scaleAnimationDuration
                    );
                    // scale up skill indicators
                    aimDirection
                        .transform
                        .DOScale(
                            new Vector3(
                                initialSkillIndicatorScale,
                                initialSkillIndicatorScale,
                                initialSkillIndicatorScale
                            ),
                            scaleAnimationDuration
                        );
                }
            }
            previousPlayerRadius = serverPlayerRadius;
        }
    }

    float CalculateScaleMultiplier(float newValue, float previousValue)
    {
        float diffPercentage = (newValue * 100) / previousValue;
        float scaleMultiplier = diffPercentage / 100;
        return scaleMultiplier;
    }
}
