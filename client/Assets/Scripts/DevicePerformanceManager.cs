using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevicePerformanceManager : MonoBehaviour
{
    public static DevicePerformanceManager Instance;

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

    public enum PerformanceLevel
    {
        High,
        Medium,
        Low
    }

    public PerformanceLevel CurrentPerformanceLevel { get; private set; }

    private void Start()
    {
        StartCoroutine(BenchmarkPerformance());
    }

    private IEnumerator BenchmarkPerformance()
    {
        float startTime = Time.realtimeSinceStartup;

        // Run some heavy computations here
        for (int i = 0; i < 1000000; i++)
        {
            float x = Mathf.Sqrt(i);

            // Yield return null allows other processes to run and avoids locking up the main thread
            if (i % 100000 == 0)
            {
                yield return null;
            }
        }

        float endTime = Time.realtimeSinceStartup;
        float duration = endTime - startTime;

        if (duration < 1f)
        {
            CurrentPerformanceLevel = PerformanceLevel.High;
        }
        else if (duration < 2f)
        {
            CurrentPerformanceLevel = PerformanceLevel.Medium;
        }
        else
        {
            CurrentPerformanceLevel = PerformanceLevel.Low;
        }
    }
}
