using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CrateItem : MonoBehaviour
{
    public ulong serverId;

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject healthBar;

    [SerializeField]
    DamagePopUpGenerator damagePopUpGenerator;

    Health health;
    private Dictionary<string, int> animationHashes;

    void Awake()
    {
        animationHashes = new Dictionary<string, int>
        {
            { "Shake", Animator.StringToHash("Shake") },
            { "Open", Animator.StringToHash("Open") }
        };
        health = GetComponent<Health>();
    }

    public void Initialize(Entity item)
    {
        serverId = item.Id;

        var position = Utils.TransformBackendToFrontendPosition(item.Position);
        position.y = 0;
        transform.position = position;

        health = GetComponent<Health>();
        health.CurrentHealth = item.Crate.Health;
        health.InitialHealth = item.Crate.Health;
        health.MaximumHealth = item.Crate.Health;

        healthBar.GetComponent<HealthBarItem>().SetEnemyCrateHealthBar(item.Crate.Health);

        gameObject.SetActive(true);
    }

    public void UpdateHealth(ulong updatedHealth)
    {
        if (updatedHealth != health.CurrentHealth)
        {
            string damageType = GetDamageType(health.CurrentHealth - updatedHealth);
            damagePopUpGenerator.GenerateDamagePopUp(
                transform.position,
                (health.CurrentHealth - updatedHealth).ToString(),
                "heavy"
            );
            healthBar.GetComponent<HealthBarItem>().UpdateHealthBar(updatedHealth);
            health.SetHealth(updatedHealth);
            ExecuteHitFeedback();
        }
    }

    public void ExecuteHitFeedback()
    {
        PlayAnimation("Shake");
    }

    public void ExecuteOpenedFeedback()
    {
        damagePopUpGenerator.GenerateDamagePopUp(
            transform.position,
            (health.CurrentHealth).ToString(),
            "heavy"
        );
        health.SetHealth(0);
        healthBar.GetComponent<HealthBarItem>().UpdateHealthBar(0);
        PlayAnimation("Open");
    }

    public void PlayAnimation(string animationName)
    {
        if (animationHashes.TryGetValue(animationName, out int hash))
        {
            animator.SetTrigger(hash);
        }
    }

    private string GetDamageType(float damage)
    {
        float damagePercentage = damage / health.MaximumHealth;

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
}
