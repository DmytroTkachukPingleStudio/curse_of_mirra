using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityHandler : MonoBehaviour
{
    public ToggleButton highQualityButton;
    public ToggleButton mediumQualityButton;
    public ToggleButton lowQualityButton;

    void Start()
    {
        DevicePerformanceManager.PerformanceLevel performanceLevel = DevicePerformanceManager
            .Instance
            .CurrentPerformanceLevel;

        highQualityButton.ToggleUIState(
            performanceLevel == DevicePerformanceManager.PerformanceLevel.High
        );
        mediumQualityButton.ToggleUIState(
            performanceLevel == DevicePerformanceManager.PerformanceLevel.Medium
        );
        lowQualityButton.ToggleUIState(
            performanceLevel == DevicePerformanceManager.PerformanceLevel.Low
        );
    }

    public void SetHighQuality()
    {
        BattlePerformanceManager
            .Instance
            .ToggleComponentsBasedOnPerformance(DevicePerformanceManager.PerformanceLevel.High);
        highQualityButton.ToggleUIState(true);
        mediumQualityButton.ToggleUIState(false);
        lowQualityButton.ToggleUIState(false);
    }

    public void SetMediumQuality()
    {
        BattlePerformanceManager
            .Instance
            .ToggleComponentsBasedOnPerformance(DevicePerformanceManager.PerformanceLevel.Medium);

        highQualityButton.ToggleUIState(false);
        mediumQualityButton.ToggleUIState(true);
        lowQualityButton.ToggleUIState(false);
    }

    public void SetLowQuality()
    {
        BattlePerformanceManager
            .Instance
            .ToggleComponentsBasedOnPerformance(DevicePerformanceManager.PerformanceLevel.Low);

        highQualityButton.ToggleUIState(false);
        mediumQualityButton.ToggleUIState(false);
        lowQualityButton.ToggleUIState(true);
    }
}
