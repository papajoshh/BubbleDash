using UnityEngine;

public class OptimizationSettings : MonoBehaviour
{
    [Header("Performance Settings")]
    public int targetFrameRate = 60;
    public bool reducedQualityForMobile = true;
    
    void Awake()
    {
        // Set target framerate
        Application.targetFrameRate = targetFrameRate;
        
        // Optimize for mobile
        if (Application.isMobilePlatform || reducedQualityForMobile)
        {
            // Reduce quality settings
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadows = ShadowQuality.Disable;
            
            // Reduce physics update rate
            Time.fixedDeltaTime = 0.02f; // 50Hz instead of default
            
            // Reduce max delta time to prevent spiral of death
            Time.maximumDeltaTime = 0.1f;
        }
        
        // Optimize garbage collection
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}