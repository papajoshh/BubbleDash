using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    [Header("Wave Settings")]
    public WaveData[] waves;
    public float distancePerWave = 200f;
    
    // Current state
    private int currentWaveIndex = 0;
    private float totalDistanceTraveled = 0f;
    private bool isGameRunning = false;
    private bool hasTriggeredWaveTransition = false;
    
    // Events
    public static event Action<WaveData, int> OnWaveStarted;
    public static event Action<WaveData, int> OnWaveCompleted;
    
    [System.Serializable]
    public class WaveData
    {
        public string waveName;
        public string description;
        public float energyDrainMultiplier = 1f;
        public float objectiveFrequencyMultiplier = 1f;
        public Color waveColor = Color.white;
        public AudioClip waveMusic;
        
        [Header("Objective Focus")]
        public ObjectiveManager.ObjectiveType[] preferredObjectiveTypes;
        public float focusWeight = 1.5f;
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Create default waves if none assigned
        if (waves == null || waves.Length == 0)
        {
            CreateDefaultWaves();
        }
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += StartWaveSystem;
            GameManager.Instance.OnGameOver += StopWaveSystem;
            GameManager.Instance.OnGameRestart += RestartWaveSystem;
        }
        
        // Subscribe to score manager for distance tracking
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnDistanceChanged += OnDistanceChanged;
        }
    }
    
    void CreateDefaultWaves()
    {
        waves = new WaveData[]
        {
            new WaveData
            {
                waveName = "LEARNING ZONE",
                description = "Get familiar with the controls",
                energyDrainMultiplier = 0.8f,
                objectiveFrequencyMultiplier = 0.8f,
                waveColor = Color.green,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.HitBubbles, ObjectiveManager.ObjectiveType.GetCombo }
            },
            new WaveData
            {
                waveName = "SPEED CHALLENGE",
                description = "Build momentum and speed",
                energyDrainMultiplier = 1f,
                objectiveFrequencyMultiplier = 1f,
                waveColor = Color.cyan,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.ReachMomentum, ObjectiveManager.ObjectiveType.GetCombo }
            },
            new WaveData
            {
                waveName = "PRECISION ZONE",
                description = "Test your accuracy",
                energyDrainMultiplier = 1.2f,
                objectiveFrequencyMultiplier = 1.1f,
                waveColor = Color.yellow,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.PerfectAccuracy, ObjectiveManager.ObjectiveType.NoMissStreak }
            },
            new WaveData
            {
                waveName = "COIN RUSH",
                description = "Collect coins while surviving",
                energyDrainMultiplier = 1.2f,
                objectiveFrequencyMultiplier = 1.2f,
                waveColor = new Color(1f, 0.8f, 0f), // Gold color
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.CollectCoins, ObjectiveManager.ObjectiveType.HitBubbles }
            },
            new WaveData
            {
                waveName = "CHAOS MODE",
                description = "Survive the ultimate challenge",
                energyDrainMultiplier = 1.5f,
                objectiveFrequencyMultiplier = 1.3f,
                waveColor = Color.red,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.HitBubbles, ObjectiveManager.ObjectiveType.GetCombo, ObjectiveManager.ObjectiveType.ReachMomentum }
            }
        };
    }
    
    public void StartWaveSystem()
    {
        isGameRunning = true;
        currentWaveIndex = 0;
        totalDistanceTraveled = 0f;
        hasTriggeredWaveTransition = false;
        
        StartWave(0);
    }
    
    public void StopWaveSystem()
    {
        isGameRunning = false;
    }
    
    public void RestartWaveSystem()
    {
        StopWaveSystem();
        StartWaveSystem();
    }
    
    void OnDistanceChanged(int distance)
    {
        if (!isGameRunning) return;
        
        totalDistanceTraveled = distance;
        
        // Check if we should advance to next wave
        int targetWaveIndex = Mathf.FloorToInt(totalDistanceTraveled / distancePerWave);
        
        if (targetWaveIndex > currentWaveIndex && targetWaveIndex < waves.Length && !hasTriggeredWaveTransition)
        {
            hasTriggeredWaveTransition = true;
            CompleteCurrentWave();
            StartWave(targetWaveIndex);
            
            // Reset transition flag after a short delay
            StartCoroutine(ResetTransitionFlag());
        }
    }
    
    System.Collections.IEnumerator ResetTransitionFlag()
    {
        yield return new WaitForSeconds(1f);
        hasTriggeredWaveTransition = false;
    }
    
    void StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Length) 
        {
            // If beyond last wave, stay on last wave
            waveIndex = waves.Length - 1;
        }
        
        currentWaveIndex = waveIndex;
        WaveData currentWave = waves[currentWaveIndex];
        
        // Trigger safe zone before wave starts (except first wave)
        if (waveIndex > 0 && EnergyManager.Instance != null)
        {
            EnergyManager.Instance.StartSafeZone();
        }
        
        // Apply wave effects to energy system
        ApplyWaveEffects(currentWave);
        
        OnWaveStarted?.Invoke(currentWave, currentWaveIndex);
        
        // Visual effects
        if (SimpleEffects.Instance != null)
        {
            // Screen flash removed - ScreenFlash method not available
            // TODO: Implement screen flash effect or use alternative visual feedback
        }
        
        // Audio feedback
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayWaveTransition();
        }
        
        Debug.Log($"Wave {currentWaveIndex + 1} Started: {currentWave.waveName}");
    }
    
    void ApplyWaveEffects(WaveData wave)
    {
        // Apply energy drain multiplier effects
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.ApplyWaveDrainMultiplier(wave.energyDrainMultiplier);
        }
        
        // Apply objective frequency effects
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.ApplyWaveFrequencyMultiplier(wave.objectiveFrequencyMultiplier);
        }
        
        // Additional wave-specific effects could be added here
        // For example: coin spawn rate, obstacle frequency, etc.
    }
    
    void CompleteCurrentWave()
    {
        if (currentWaveIndex >= waves.Length) return;
        
        WaveData completedWave = waves[currentWaveIndex];
        OnWaveCompleted?.Invoke(completedWave, currentWaveIndex);
        
        Debug.Log($"Wave {currentWaveIndex + 1} Completed: {completedWave.waveName}");
    }
    
    // Public getters
    public WaveData GetCurrentWave()
    {
        return currentWaveIndex < waves.Length ? waves[currentWaveIndex] : waves[waves.Length - 1];
    }
    
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
    
    public string GetCurrentWaveName()
    {
        var currentWave = GetCurrentWave();
        return currentWave != null ? currentWave.waveName : "UNKNOWN";
    }
    
    public float GetWaveProgress()
    {
        if (currentWaveIndex >= waves.Length - 1)
        {
            // On final wave, progress based on how far beyond the last threshold
            float finalWaveStart = (waves.Length - 1) * distancePerWave;
            float distanceInFinalWave = totalDistanceTraveled - finalWaveStart;
            return Mathf.Clamp01(distanceInFinalWave / distancePerWave);
        }
        
        float waveStartDistance = currentWaveIndex * distancePerWave;
        float waveEndDistance = (currentWaveIndex + 1) * distancePerWave;
        return (totalDistanceTraveled - waveStartDistance) / (waveEndDistance - waveStartDistance);
    }
    
    public float GetDistanceToNextWave()
    {
        if (currentWaveIndex >= waves.Length - 1)
        {
            return 0f; // No next wave
        }
        
        float nextWaveDistance = (currentWaveIndex + 1) * distancePerWave;
        return Mathf.Max(0, nextWaveDistance - totalDistanceTraveled);
    }
    
    public int GetTotalWaves()
    {
        return waves != null ? waves.Length : 0;
    }
    
    public bool IsOnFinalWave()
    {
        return currentWaveIndex >= waves.Length - 1;
    }
    
    // Method for other systems to check current wave preferences
    public bool IsObjectiveTypePreferred(ObjectiveManager.ObjectiveType type)
    {
        var currentWave = GetCurrentWave();
        if (currentWave == null || currentWave.preferredObjectiveTypes == null) return false;
        
        foreach (var preferredType in currentWave.preferredObjectiveTypes)
        {
            if (preferredType == type) return true;
        }
        
        return false;
    }
    
    public float GetCurrentWaveObjectiveFrequency()
    {
        var currentWave = GetCurrentWave();
        return currentWave != null ? currentWave.objectiveFrequencyMultiplier : 1f;
    }
    
    public float GetCurrentWaveEnergyDrainMultiplier()
    {
        var currentWave = GetCurrentWave();
        return currentWave != null ? currentWave.energyDrainMultiplier : 1f;
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= StartWaveSystem;
            GameManager.Instance.OnGameOver -= StopWaveSystem;
            GameManager.Instance.OnGameRestart -= RestartWaveSystem;
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnDistanceChanged -= OnDistanceChanged;
        }
    }
}