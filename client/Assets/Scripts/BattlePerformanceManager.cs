using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePerformanceManager : MonoBehaviour
{
    public GameObject[] HighQualityComponents;
    public GameObject[] MediumQualityComponents;

    public static BattlePerformanceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DevicePerformanceManager.PerformanceLevel performanceLevel = DevicePerformanceManager
            .Instance
            .CurrentPerformanceLevel;
        ToggleComponentsBasedOnPerformance(performanceLevel);
    }

    public void ToggleComponentsBasedOnPerformance(
        DevicePerformanceManager.PerformanceLevel performanceLevel
    )
    {
        switch (performanceLevel)
        {
            case DevicePerformanceManager.PerformanceLevel.High:
                ToggleComponents(HighQualityComponents, true);
                ToggleComponents(MediumQualityComponents, true);
                break;
            case DevicePerformanceManager.PerformanceLevel.Medium:
                ToggleComponents(HighQualityComponents, false);
                ToggleComponents(MediumQualityComponents, true);
                break;
            case DevicePerformanceManager.PerformanceLevel.Low:
                ToggleComponents(HighQualityComponents, false);
                ToggleComponents(MediumQualityComponents, false);
                break;
        }
    }

    private void ToggleComponents(GameObject[] componentsArray, bool toggleState)
    {
        foreach (GameObject vfx in componentsArray)
        {
            if (vfx != null)
            {
                vfx.SetActive(toggleState);
            }
        }
    }
}
