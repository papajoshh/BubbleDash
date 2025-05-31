using UnityEngine;
using System.Collections;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("Settings")]
    public bool showFPS = true;
    public bool autoDisableEffects = true;
    public float lowFPSThreshold = 30f;
    
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private int lowFPSFrameCount = 0;
    private bool effectsDisabled = false;
    
    void Start()
    {
        StartCoroutine(MonitorPerformance());
    }
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }
    
    IEnumerator MonitorPerformance()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            if (fps < lowFPSThreshold)
            {
                lowFPSFrameCount++;
                
                if (lowFPSFrameCount > 3 && !effectsDisabled && autoDisableEffects)
                {
                    DisableExpensiveEffects();
                }
            }
            else
            {
                lowFPSFrameCount = 0;
            }
        }
    }
    
    void DisableExpensiveEffects()
    {
        effectsDisabled = true;
        Debug.Log("Low FPS detected - disabling expensive effects");
        
        // Disable parallax backgrounds
        ParallaxBackground[] parallax = FindObjectsOfType<ParallaxBackground>();
        foreach (var p in parallax)
        {
            p.enabled = false;
        }
        
        // Reduce particle effects
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (var ps in particles)
        {
            var main = ps.main;
            main.maxParticles = Mathf.Min(main.maxParticles, 10);
        }
        
        // Disable screen shake
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.shakeIntensity = 0f;
        }
    }
    
    void OnGUI()
    {
        if (showFPS)
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            
            Rect rect = new Rect(w - 100, 10, 90, 20);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = 18;
            style.normal.textColor = fps > 45 ? Color.green : fps > 30 ? Color.yellow : Color.red;
            
            string text = string.Format("{0:0.} fps", fps);
            GUI.Label(rect, text, style);
        }
    }
}