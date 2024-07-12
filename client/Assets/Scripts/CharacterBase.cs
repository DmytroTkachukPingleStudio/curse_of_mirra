using MoreMountains.Tools;
using TMPro;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerName,
        Hitbox,
        FeedbackContainer,
        skillIndicatorsManager,
        spawnFeedback,
        OrientationIndicator,
        OrientationArrow,
        CharacterCard,
        CanvasHolder,
        StaminaCharges,
        powerUpsIcon,
        powerUpsCount,
        characterShadow,
        healthBar;

    [SerializeField]
    public AudioClip spawnSfx;

    [SerializeField]
    Sound3DManager sound3DManager;

    const float SPAWN_SFX_VOLUME = 0.01f;

    public void ToggleSpawnFeedback(bool isActiveSound, string id)
    {
        spawnFeedback.SetActive(isActiveSound);
        if (isActiveSound)
        {
            MMSoundManagerSoundPlayEvent.Trigger(
                spawnSfx,
                MMSoundManager.MMSoundManagerTracks.Sfx,
                Utils.GetPlayer(GameServerConnectionManager.Instance.playerId).transform.position,
                false,
                SPAWN_SFX_VOLUME
            );
        }
    }

    public void SetPowerUpCount(ulong count)
    {
        powerUpsIcon.SetActive(!(count > 0));
        powerUpsCount.GetComponent<TextMeshProUGUI>().text = count.ToString();
        powerUpsCount.SetActive(count > 0);
    }
}
