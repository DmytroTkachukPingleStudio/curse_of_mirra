using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private MMSoundManager soundManager;
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    // Start is called before the first frame update
    void Awake()
    {
        soundManager = FindObjectOfType<MMSoundManager>();
        masterVolumeSlider = GameObject.Find("MasterVolumeSlider").GetComponent<Slider>();
        musicVolumeSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        sfxVolumeSlider = GameObject.Find("SfxVolumeSlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    public void ChangeMasterVolume()
    {
        soundManager.SetVolumeMaster(masterVolumeSlider.value);
    }

    public void ChangeMusicVolume()
    {
        soundManager.SetVolumeMusic(musicVolumeSlider.value);
    }

    public void ChangeSfxVolume()
    {
        soundManager.SetVolumeSfx(sfxVolumeSlider.value);
    }
}
