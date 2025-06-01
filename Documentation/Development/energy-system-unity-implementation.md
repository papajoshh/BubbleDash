# 🔧 ENERGY SYSTEM - GUÍA DETALLADA DE IMPLEMENTACIÓN UNITY

## 📋 OVERVIEW DE IMPLEMENTACIÓN

Este documento te guía **paso a paso** para implementar el Energy System en Unity, reemplazando completamente el timer system actual.

---

## 🏗️ PARTE 1: NUEVO ENERGY MANAGER

### Paso 1: Crear EnergyManager.cs

Crea el archivo: `/Assets/Scripts/Managers/EnergyManager.cs`

```csharp
using UnityEngine;
using System;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }
    
    [Header("Energy Settings")]
    public float maxEnergy = 10f;
    public float currentEnergy { get; private set; }
    public float energyDrainRate = 1f; // per second
    public float energyPerHit = 1f;
    public float energyPerObjective = 3f;
    public float energyPerCoin = 0.5f;
    
    [Header("Learning Curve")]
    public float beginnerEnergyBonus = 5f;
    public float beginnerDrainReduction = 0.5f;
    public int beginnerRunsCount = 3;
    
    [Header("Energy Shield")]
    public float maxShieldTime = 5f;
    public float currentShieldTime { get; private set; }
    
    [Header("Safe Zones")]
    public float safeZoneDuration = 3f;
    public bool isInSafeZone { get; private set; }
    
    // Events
    public static event Action<float> OnEnergyChanged;
    public static event Action OnEnergyEmpty;
    public static event Action<float> OnShieldChanged;
    public static event Action<bool> OnSafeZoneChanged;
    
    // Internal state
    private bool isGameRunning = false;
    private bool isBeginnerMode = false;
    private Coroutine energyDrainCoroutine;
    private Coroutine shieldCoroutine;
    
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
        // Check if player is in beginner mode
        int totalRuns = PlayerPrefs.GetInt("TotalRuns", 0);
        isBeginnerMode = totalRuns < beginnerRunsCount;
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += StartEnergySystem;
            GameManager.Instance.OnGameOver += StopEnergySystem;
            GameManager.Instance.OnGameRestart += RestartEnergySystem;
        }
        
        // Subscribe to gameplay events
        if (MomentumSystem.Instance != null)
        {
            // We'll connect this when MomentumSystem calls OnBubbleHit
        }
        
        InitializeEnergy();
    }
    
    void InitializeEnergy()
    {
        // Set starting energy based on difficulty
        float startingEnergy = maxEnergy;
        if (isBeginnerMode)
        {
            startingEnergy += beginnerEnergyBonus;
        }
        
        currentEnergy = startingEnergy;
        currentShieldTime = 0f;
        isInSafeZone = false;
        
        OnEnergyChanged?.Invoke(currentEnergy);
        OnShieldChanged?.Invoke(currentShieldTime);
        OnSafeZoneChanged?.Invoke(isInSafeZone);
    }
    
    public void StartEnergySystem()
    {
        isGameRunning = true;
        InitializeEnergy();
        StartEnergyDrain();
        
        Debug.Log($"Energy System Started - Starting Energy: {currentEnergy}, Drain Rate: {GetCurrentDrainRate()}");
    }
    
    public void StopEnergySystem()
    {
        isGameRunning = false;
        StopEnergyDrain();
        
        // Increment total runs for beginner mode tracking
        int totalRuns = PlayerPrefs.GetInt("TotalRuns", 0);
        PlayerPrefs.SetInt("TotalRuns", totalRuns + 1);
        PlayerPrefs.Save();
    }
    
    public void RestartEnergySystem()
    {
        StopEnergySystem();
        StartEnergySystem();
    }
    
    void StartEnergyDrain()
    {
        if (energyDrainCoroutine != null)
        {
            StopCoroutine(energyDrainCoroutine);
        }
        energyDrainCoroutine = StartCoroutine(EnergyDrainLoop());
    }
    
    void StopEnergyDrain()
    {
        if (energyDrainCoroutine != null)
        {
            StopCoroutine(energyDrainCoroutine);
            energyDrainCoroutine = null;
        }
    }
    
    IEnumerator EnergyDrainLoop()
    {
        while (isGameRunning && currentEnergy > 0)
        {
            yield return new WaitForSeconds(0.1f); // Update every 100ms for smooth UI
            
            if (!isInSafeZone && currentShieldTime <= 0)
            {
                float drainAmount = GetCurrentDrainRate() * 0.1f; // 0.1 second worth
                ModifyEnergy(-drainAmount);
                
                if (currentEnergy <= 0)
                {
                    OnGameOver();
                    break;
                }
            }
            
            // Update shield time
            if (currentShieldTime > 0)
            {
                currentShieldTime -= 0.1f;
                currentShieldTime = Mathf.Max(0, currentShieldTime);
                OnShieldChanged?.Invoke(currentShieldTime);
            }
        }
    }
    
    float GetCurrentDrainRate()
    {
        float baseRate = energyDrainRate;
        
        // Apply beginner mode reduction
        if (isBeginnerMode)
        {
            baseRate -= beginnerDrainReduction;
        }
        
        // Apply momentum system bonus
        if (MomentumSystem.Instance != null)
        {
            int momentum = MomentumSystem.Instance.GetComboCount();
            if (momentum >= 15) baseRate *= 0.8f; // 20% reduction at max momentum
            else if (momentum >= 10) baseRate *= 0.9f; // 10% reduction at high momentum
        }
        
        return Mathf.Max(0.1f, baseRate); // Never go below 0.1/second
    }
    
    // Called by other systems when energy should be gained
    public void OnBubbleHit()
    {
        float energyGain = energyPerHit;
        
        // Apply momentum bonus
        if (MomentumSystem.Instance != null)
        {
            int momentum = MomentumSystem.Instance.GetComboCount();
            if (momentum >= 10) energyGain *= 1.5f;
            else if (momentum >= 5) energyGain *= 1.2f;
        }
        
        ModifyEnergy(energyGain);
        
        Debug.Log($"Energy gained from bubble hit: +{energyGain} (Total: {currentEnergy})");
    }
    
    public void OnObjectiveComplete()
    {
        ModifyEnergy(energyPerObjective);
        
        // Grant shield time
        AddShieldTime(3f);
        
        Debug.Log($"Energy gained from objective: +{energyPerObjective} (Total: {currentEnergy})");
    }
    
    public void OnCoinCollected()
    {
        ModifyEnergy(energyPerCoin);
        
        Debug.Log($"Energy gained from coin: +{energyPerCoin} (Total: {currentEnergy})");
    }
    
    void ModifyEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Max(0, currentEnergy); // Can't go below 0
        
        OnEnergyChanged?.Invoke(currentEnergy);
    }
    
    public void AddShieldTime(float seconds)
    {
        currentShieldTime += seconds;
        currentShieldTime = Mathf.Min(maxShieldTime, currentShieldTime);
        OnShieldChanged?.Invoke(currentShieldTime);
        
        // Visual/audio feedback
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.PlayShieldActivation();
        }
    }
    
    public void StartSafeZone()
    {
        if (isInSafeZone) return;
        
        isInSafeZone = true;
        OnSafeZoneChanged?.Invoke(true);
        
        // Auto-end safe zone after duration
        StartCoroutine(EndSafeZoneAfterDelay());
        
        // Give small energy bonus
        ModifyEnergy(1f);
        
        Debug.Log("Safe Zone Started");
    }
    
    IEnumerator EndSafeZoneAfterDelay()
    {
        yield return new WaitForSeconds(safeZoneDuration);
        EndSafeZone();
    }
    
    public void EndSafeZone()
    {
        if (!isInSafeZone) return;
        
        isInSafeZone = false;
        OnSafeZoneChanged?.Invoke(false);
        
        Debug.Log("Safe Zone Ended");
    }
    
    void OnGameOver()
    {
        isGameRunning = false;
        OnEnergyEmpty?.Invoke();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        Debug.Log("Game Over - Energy Depleted");
    }
    
    // Public getters for UI and other systems
    public float GetEnergyPercent()
    {
        float maxPossibleEnergy = maxEnergy + (isBeginnerMode ? beginnerEnergyBonus : 0);
        return currentEnergy / maxPossibleEnergy;
    }
    
    public bool HasShield()
    {
        return currentShieldTime > 0;
    }
    
    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }
    
    public bool IsBeginnerMode()
    {
        return isBeginnerMode;
    }
    
    void OnDestroy()
    {
        // Clean up events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= StartEnergySystem;
            GameManager.Instance.OnGameOver -= StopEnergySystem;
            GameManager.Instance.OnGameRestart -= RestartEnergySystem;
        }
    }
}
```

---

## 🎯 PARTE 2: MINI-OBJECTIVES MANAGER

### Paso 2: Crear ObjectiveManager.cs

Crea el archivo: `/Assets/Scripts/Managers/ObjectiveManager.cs`

```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }
    
    [Header("Objective Settings")]
    public float objectiveInterval = 20f;
    public float objectiveTimeLimit = 30f;
    public int maxActiveObjectives = 1;
    
    [Header("Objective Types")]
    public ObjectiveData[] availableObjectives;
    
    // Current active objective
    private Objective currentObjective;
    private Coroutine objectiveSpawnCoroutine;
    private bool isGameRunning = false;
    
    // Events
    public static event Action<Objective> OnObjectiveStarted;
    public static event Action<Objective> OnObjectiveCompleted;
    public static event Action<Objective> OnObjectiveFailed;
    
    // Player state tracking for smart objective selection
    private int consecutiveHits = 0;
    private int totalHits = 0;
    private int totalMisses = 0;
    private int coinsCollected = 0;
    private int currentMomentum = 0;
    
    [System.Serializable]
    public class ObjectiveData
    {
        public ObjectiveType type;
        public string displayName;
        public string description;
        public int targetValue;
        public float weight = 1f;
        public Sprite icon;
    }
    
    public enum ObjectiveType
    {
        HitBubbles,           // Hit X bubbles
        GetCombo,             // Get X consecutive hits
        CollectCoins,         // Collect X coins
        HitSpecificColor,     // Hit X bubbles of specific color
        NoMissStreak,         // Don't miss for X seconds
        ReachMomentum,        // Reach X momentum level
        MaintainSpeed,        // Maintain max speed for X seconds
        PerfectAccuracy       // Hit X bubbles with 100% accuracy
    }
    
    [System.Serializable]
    public class Objective
    {
        public ObjectiveType type;
        public string displayName;
        public string description;
        public int targetValue;
        public int currentProgress;
        public float timeRemaining;
        public bool isCompleted;
        public bool isFailed;
        public Sprite icon;
        
        public float GetProgressPercent()
        {
            return targetValue > 0 ? (float)currentProgress / targetValue : 0f;
        }
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
        // Create default objectives if none assigned
        if (availableObjectives == null || availableObjectives.Length == 0)
        {
            CreateDefaultObjectives();
        }
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += StartObjectiveSystem;
            GameManager.Instance.OnGameOver += StopObjectiveSystem;
            GameManager.Instance.OnGameRestart += RestartObjectiveSystem;
        }
        
        // Subscribe to gameplay events for tracking
        if (MomentumSystem.Instance != null)
        {
            // We'll connect this in the MomentumSystem modifications
        }
    }
    
    void CreateDefaultObjectives()
    {
        availableObjectives = new ObjectiveData[]
        {
            new ObjectiveData { type = ObjectiveType.HitBubbles, displayName = "BUBBLE BURST", description = "Hit {0} bubbles", targetValue = 8, weight = 1f },
            new ObjectiveData { type = ObjectiveType.GetCombo, displayName = "COMBO MASTER", description = "Get {0} consecutive hits", targetValue = 5, weight = 1f },
            new ObjectiveData { type = ObjectiveType.CollectCoins, displayName = "COIN HUNTER", description = "Collect {0} coins", targetValue = 3, weight = 0.8f },
            new ObjectiveData { type = ObjectiveType.NoMissStreak, displayName = "PERFECT AIM", description = "Don't miss for {0} seconds", targetValue = 15, weight = 0.7f },
            new ObjectiveData { type = ObjectiveType.ReachMomentum, displayName = "SPEED DEMON", description = "Reach {0}x momentum", targetValue = 10, weight = 0.9f },
            new ObjectiveData { type = ObjectiveType.PerfectAccuracy, displayName = "SNIPER MODE", description = "Hit {0} bubbles perfectly", targetValue = 6, weight = 0.6f }
        };
    }
    
    public void StartObjectiveSystem()
    {
        isGameRunning = true;
        ResetPlayerTracking();
        StartObjectiveSpawning();
        
        Debug.Log("Objective System Started");
    }
    
    public void StopObjectiveSystem()
    {
        isGameRunning = false;
        StopObjectiveSpawning();
        
        if (currentObjective != null && !currentObjective.isCompleted)
        {
            FailCurrentObjective();
        }
    }
    
    public void RestartObjectiveSystem()
    {
        StopObjectiveSystem();
        StartObjectiveSystem();
    }
    
    void ResetPlayerTracking()
    {
        consecutiveHits = 0;
        totalHits = 0;
        totalMisses = 0;
        coinsCollected = 0;
        currentMomentum = 0;
    }
    
    void StartObjectiveSpawning()
    {
        if (objectiveSpawnCoroutine != null)
        {
            StopCoroutine(objectiveSpawnCoroutine);
        }
        objectiveSpawnCoroutine = StartCoroutine(ObjectiveSpawnLoop());
    }
    
    void StopObjectiveSpawning()
    {
        if (objectiveSpawnCoroutine != null)
        {
            StopCoroutine(objectiveSpawnCoroutine);
            objectiveSpawnCoroutine = null;
        }
    }
    
    IEnumerator ObjectiveSpawnLoop()
    {
        // Wait a bit before first objective
        yield return new WaitForSeconds(5f);
        
        while (isGameRunning)
        {
            if (currentObjective == null)
            {
                SpawnNewObjective();
            }
            
            yield return new WaitForSeconds(1f); // Check every second
        }
    }
    
    void SpawnNewObjective()
    {
        ObjectiveData selectedData = SelectSmartObjective();
        if (selectedData == null) return;
        
        currentObjective = new Objective
        {
            type = selectedData.type,
            displayName = selectedData.displayName,
            description = string.Format(selectedData.description, selectedData.targetValue),
            targetValue = selectedData.targetValue,
            currentProgress = 0,
            timeRemaining = objectiveTimeLimit,
            isCompleted = false,
            isFailed = false,
            icon = selectedData.icon
        };
        
        OnObjectiveStarted?.Invoke(currentObjective);
        StartCoroutine(ObjectiveTimer(currentObjective));
        
        Debug.Log($"New Objective: {currentObjective.displayName} - {currentObjective.description}");
    }
    
    ObjectiveData SelectSmartObjective()
    {
        List<ObjectiveData> weightedPool = new List<ObjectiveData>();
        
        foreach (var obj in availableObjectives)
        {
            float adjustedWeight = obj.weight;
            
            // Adjust weights based on player performance
            switch (obj.type)
            {
                case ObjectiveType.GetCombo:
                    if (consecutiveHits < 3) adjustedWeight *= 1.5f; // Encourage combos if player isn't chaining
                    break;
                    
                case ObjectiveType.CollectCoins:
                    if (coinsCollected < 2) adjustedWeight *= 1.3f; // Encourage coin collection
                    break;
                    
                case ObjectiveType.ReachMomentum:
                    if (currentMomentum < 5) adjustedWeight *= 1.4f; // Encourage momentum building
                    break;
                    
                case ObjectiveType.PerfectAccuracy:
                    float accuracy = totalHits + totalMisses > 0 ? (float)totalHits / (totalHits + totalMisses) : 1f;
                    if (accuracy < 0.7f) adjustedWeight *= 0.5f; // Reduce if player is struggling with accuracy
                    break;
            }
            
            // Add to weighted pool
            int weightCount = Mathf.RoundToInt(adjustedWeight * 10);
            for (int i = 0; i < weightCount; i++)
            {
                weightedPool.Add(obj);
            }
        }
        
        return weightedPool.Count > 0 ? weightedPool[UnityEngine.Random.Range(0, weightedPool.Count)] : availableObjectives[0];
    }
    
    IEnumerator ObjectiveTimer(Objective objective)
    {
        while (objective.timeRemaining > 0 && !objective.isCompleted && !objective.isFailed)
        {
            yield return new WaitForSeconds(1f);
            objective.timeRemaining -= 1f;
        }
        
        if (!objective.isCompleted && !objective.isFailed)
        {
            FailCurrentObjective();
        }
    }
    
    void CompleteCurrentObjective()
    {
        if (currentObjective == null || currentObjective.isCompleted) return;
        
        currentObjective.isCompleted = true;
        OnObjectiveCompleted?.Invoke(currentObjective);
        
        // Give energy bonus
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnObjectiveComplete();
        }
        
        // Clear current objective after delay for UI feedback
        StartCoroutine(ClearObjectiveAfterDelay(2f));
        
        Debug.Log($"Objective Completed: {currentObjective.displayName}");
    }
    
    void FailCurrentObjective()
    {
        if (currentObjective == null || currentObjective.isFailed) return;
        
        currentObjective.isFailed = true;
        OnObjectiveFailed?.Invoke(currentObjective);
        
        // Clear current objective after delay for UI feedback
        StartCoroutine(ClearObjectiveAfterDelay(1f));
        
        Debug.Log($"Objective Failed: {currentObjective.displayName}");
    }
    
    IEnumerator ClearObjectiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentObjective = null;
        
        // Wait before spawning next objective
        yield return new WaitForSeconds(objectiveInterval - delay);
    }
    
    // Called by other systems to update objective progress
    public void OnBubbleHit(BubbleColor color)
    {
        totalHits++;
        consecutiveHits++;
        
        if (currentObjective == null || currentObjective.isCompleted) return;
        
        switch (currentObjective.type)
        {
            case ObjectiveType.HitBubbles:
                currentObjective.currentProgress++;
                break;
                
            case ObjectiveType.GetCombo:
                if (consecutiveHits >= currentObjective.targetValue)
                {
                    currentObjective.currentProgress = currentObjective.targetValue;
                }
                break;
                
            case ObjectiveType.HitSpecificColor:
                // Would need to track specific color - implement if needed
                break;
                
            case ObjectiveType.PerfectAccuracy:
                currentObjective.currentProgress++;
                break;
        }
        
        CheckObjectiveCompletion();
    }
    
    public void OnBubbleMiss()
    {
        totalMisses++;
        consecutiveHits = 0;
        
        // Reset certain objectives on miss
        if (currentObjective != null && !currentObjective.isCompleted)
        {
            if (currentObjective.type == ObjectiveType.NoMissStreak ||
                currentObjective.type == ObjectiveType.PerfectAccuracy)
            {
                currentObjective.currentProgress = 0;
            }
        }
    }
    
    public void OnCoinCollected()
    {
        coinsCollected++;
        
        if (currentObjective != null && currentObjective.type == ObjectiveType.CollectCoins)
        {
            currentObjective.currentProgress++;
            CheckObjectiveCompletion();
        }
    }
    
    public void OnMomentumChanged(int momentum)
    {
        currentMomentum = momentum;
        
        if (currentObjective != null && currentObjective.type == ObjectiveType.ReachMomentum)
        {
            if (momentum >= currentObjective.targetValue)
            {
                currentObjective.currentProgress = currentObjective.targetValue;
                CheckObjectiveCompletion();
            }
        }
    }
    
    void CheckObjectiveCompletion()
    {
        if (currentObjective != null && currentObjective.currentProgress >= currentObjective.targetValue)
        {
            CompleteCurrentObjective();
        }
    }
    
    // Public getters
    public Objective GetCurrentObjective()
    {
        return currentObjective;
    }
    
    public bool HasActiveObjective()
    {
        return currentObjective != null && !currentObjective.isCompleted && !currentObjective.isFailed;
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= StartObjectiveSystem;
            GameManager.Instance.OnGameOver -= StopObjectiveSystem;
            GameManager.Instance.OnGameRestart -= RestartObjectiveSystem;
        }
    }
}
```

---

## 🌊 PARTE 3: WAVE SYSTEM

### Paso 3: Crear WaveManager.cs

Crea el archivo: `/Assets/Scripts/Managers/WaveManager.cs`

```csharp
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
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.HitBubbles }
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
                waveColor = Color.green,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.CollectCoins }
            },
            new WaveData
            {
                waveName = "CHAOS MODE",
                description = "Survive the ultimate challenge",
                energyDrainMultiplier = 1.5f,
                objectiveFrequencyMultiplier = 1.3f,
                waveColor = Color.red,
                preferredObjectiveTypes = new[] { ObjectiveManager.ObjectiveType.HitBubbles, ObjectiveManager.ObjectiveType.GetCombo }
            }
        };
    }
    
    public void StartWaveSystem()
    {
        isGameRunning = true;
        currentWaveIndex = 0;
        totalDistanceTraveled = 0f;
        
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
        
        if (targetWaveIndex > currentWaveIndex && targetWaveIndex < waves.Length)
        {
            CompleteCurrentWave();
            StartWave(targetWaveIndex);
        }
    }
    
    void StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Length) return;
        
        currentWaveIndex = waveIndex;
        WaveData currentWave = waves[currentWaveIndex];
        
        // Trigger safe zone before wave starts (except first wave)
        if (waveIndex > 0 && EnergyManager.Instance != null)
        {
            EnergyManager.Instance.StartSafeZone();
        }
        
        // Apply wave effects to energy system
        if (EnergyManager.Instance != null)
        {
            // This would require adding a method to EnergyManager to modify drain rate
            // We'll handle this in the EnergyManager modifications below
        }
        
        // Apply wave effects to objective system
        if (ObjectiveManager.Instance != null)
        {
            // This would require adding methods to ObjectiveManager for wave preferences
            // We'll handle this in ObjectiveManager modifications
        }
        
        OnWaveStarted?.Invoke(currentWave, currentWaveIndex);
        
        // Visual effects
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.ScreenFlash(currentWave.waveColor, 0.5f);
        }
        
        Debug.Log($"Wave {currentWaveIndex + 1} Started: {currentWave.waveName}");
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
        return currentWaveIndex < waves.Length ? waves[currentWaveIndex] : null;
    }
    
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
    
    public float GetWaveProgress()
    {
        float waveStartDistance = currentWaveIndex * distancePerWave;
        float waveEndDistance = (currentWaveIndex + 1) * distancePerWave;
        return (totalDistanceTraveled - waveStartDistance) / (waveEndDistance - waveStartDistance);
    }
    
    public float GetDistanceToNextWave()
    {
        float nextWaveDistance = (currentWaveIndex + 1) * distancePerWave;
        return Mathf.Max(0, nextWaveDistance - totalDistanceTraveled);
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
```

---

## 🔧 PARTE 4: MODIFICACIONES A SISTEMAS EXISTENTES

### Paso 4: Modificar MomentumSystem.cs

Agrega estas líneas al método `OnBubbleHit()` en MomentumSystem.cs:

```csharp
public void OnBubbleHit()
{
    consecutiveHits++;
    lastHitTime = Time.time;
    
    // Calculate new speed multiplier (existing code)...
    
    // NEW: Notify Energy and Objective systems
    if (EnergyManager.Instance != null)
    {
        EnergyManager.Instance.OnBubbleHit();
    }
    
    if (ObjectiveManager.Instance != null)
    {
        // You'll need to add BubbleColor parameter or get it from shooting system
        ObjectiveManager.Instance.OnBubbleHit(BubbleColor.Red); // Placeholder
        ObjectiveManager.Instance.OnMomentumChanged(consecutiveHits);
    }
    
    // Existing code continues...
}
```

Agrega estas líneas al método `OnBubbleMiss()`:

```csharp
public void OnBubbleMiss()
{
    // NEW: Notify Objective system
    if (ObjectiveManager.Instance != null)
    {
        ObjectiveManager.Instance.OnBubbleMiss();
    }
    
    if (resetOnMiss)
    {
        ResetMomentum();
    }
}
```

### Paso 5: Modificar CoinSystem.cs

Agrega esta línea al método donde se colecta una moneda:

```csharp
// En el método donde se colecta coin (normalmente en Coin.cs o CoinBubble.cs)
public void CollectCoin()
{
    // Existing coin collection code...
    
    // NEW: Notify Energy and Objective systems
    if (EnergyManager.Instance != null)
    {
        EnergyManager.Instance.OnCoinCollected();
    }
    
    if (ObjectiveManager.Instance != null)
    {
        ObjectiveManager.Instance.OnCoinCollected();
    }
    
    // Continue with existing code...
}
```

### Paso 6: Remover TimerManager.cs

1. **Eliminar** completamente el archivo `TimerManager.cs`
2. **Remover** todas las referencias al TimerManager en otros scripts
3. **Eliminar** UI elementos relacionados con timer

---

## 🎨 PARTE 5: UI PARA ENERGY SYSTEM

### Paso 7: Crear EnergyUI.cs

Crea el archivo: `/Assets/Scripts/UI/EnergyUI.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnergyUI : MonoBehaviour
{
    [Header("Energy Display")]
    public Image energyBar;
    public Image energyBarBackground;
    public TextMeshProUGUI energyText;
    public Gradient energyGradient;
    
    [Header("Shield Display")]
    public Image shieldIndicator;
    public TextMeshProUGUI shieldTimeText;
    
    [Header("Safe Zone Display")]
    public GameObject safeZonePanel;
    public TextMeshProUGUI safeZoneText;
    
    [Header("Wave Display")]
    public TextMeshProUGUI waveText;
    public Image waveProgressBar;
    public TextMeshProUGUI distanceToNextWaveText;
    
    [Header("Effects")]
    public float pulseIntensity = 1.2f;
    public float lowEnergyThreshold = 0.3f;
    
    private bool isLowEnergyPulsing = false;
    
    void Start()
    {
        // Subscribe to energy events
        EnergyManager.OnEnergyChanged += UpdateEnergyDisplay;
        EnergyManager.OnShieldChanged += UpdateShieldDisplay;
        EnergyManager.OnSafeZoneChanged += UpdateSafeZoneDisplay;
        
        // Subscribe to wave events
        WaveManager.OnWaveStarted += OnWaveStarted;
        
        // Initialize UI
        if (safeZonePanel != null) safeZonePanel.SetActive(false);
        if (shieldIndicator != null) shieldIndicator.gameObject.SetActive(false);
    }
    
    void Update()
    {
        UpdateWaveProgress();
    }
    
    void UpdateEnergyDisplay(float currentEnergy)
    {
        if (EnergyManager.Instance == null) return;
        
        float energyPercent = EnergyManager.Instance.GetEnergyPercent();
        
        // Update bar fill
        if (energyBar != null)
        {
            energyBar.fillAmount = energyPercent;
            
            // Color gradient based on energy level
            if (energyGradient != null)
            {
                energyBar.color = energyGradient.Evaluate(energyPercent);
            }
        }
        
        // Update text
        if (energyText != null)
        {
            energyText.text = $"{currentEnergy:F1}";
        }
        
        // Low energy warning
        if (energyPercent <= lowEnergyThreshold && !isLowEnergyPulsing)
        {
            StartLowEnergyPulse();
        }
        else if (energyPercent > lowEnergyThreshold && isLowEnergyPulsing)
        {
            StopLowEnergyPulse();
        }
    }
    
    void StartLowEnergyPulse()
    {
        isLowEnergyPulsing = true;
        
        if (energyBarBackground != null)
        {
            energyBarBackground.DOScale(pulseIntensity, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
    
    void StopLowEnergyPulse()
    {
        isLowEnergyPulsing = false;
        
        if (energyBarBackground != null)
        {
            energyBarBackground.DOKill();
            energyBarBackground.transform.localScale = Vector3.one;
        }
    }
    
    void UpdateShieldDisplay(float shieldTime)
    {
        bool hasShield = shieldTime > 0;
        
        if (shieldIndicator != null)
        {
            shieldIndicator.gameObject.SetActive(hasShield);
            
            if (hasShield)
            {
                // Animate shield
                shieldIndicator.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
            }
            else
            {
                shieldIndicator.transform.DOKill();
            }
        }
        
        if (shieldTimeText != null)
        {
            shieldTimeText.gameObject.SetActive(hasShield);
            if (hasShield)
            {
                shieldTimeText.text = $"{shieldTime:F1}s";
            }
        }
    }
    
    void UpdateSafeZoneDisplay(bool isInSafeZone)
    {
        if (safeZonePanel != null)
        {
            safeZonePanel.SetActive(isInSafeZone);
            
            if (isInSafeZone && safeZoneText != null)
            {
                safeZoneText.text = "SAFE ZONE";
                
                // Animate safe zone text
                safeZoneText.transform.DOScale(1.1f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
    }
    
    void OnWaveStarted(WaveManager.WaveData wave, int waveIndex)
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {waveIndex + 1}: {wave.waveName}";
            waveText.color = wave.waveColor;
            
            // Animate wave announcement
            waveText.transform.localScale = Vector3.zero;
            waveText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }
    
    void UpdateWaveProgress()
    {
        if (WaveManager.Instance == null) return;
        
        // Update wave progress bar
        if (waveProgressBar != null)
        {
            waveProgressBar.fillAmount = WaveManager.Instance.GetWaveProgress();
        }
        
        // Update distance to next wave
        if (distanceToNextWaveText != null)
        {
            float distanceToNext = WaveManager.Instance.GetDistanceToNextWave();
            distanceToNextWaveText.text = $"{distanceToNext:F0}m to next wave";
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        EnergyManager.OnEnergyChanged -= UpdateEnergyDisplay;
        EnergyManager.OnShieldChanged -= UpdateShieldDisplay;
        EnergyManager.OnSafeZoneChanged -= UpdateSafeZoneDisplay;
        WaveManager.OnWaveStarted -= OnWaveStarted;
        
        // Stop any animations
        if (energyBarBackground != null) energyBarBackground.DOKill();
        if (shieldIndicator != null) shieldIndicator.transform.DOKill();
        if (safeZoneText != null) safeZoneText.transform.DOKill();
    }
}
```

### Paso 8: Crear ObjectiveUI.cs

Crea el archivo: `/Assets/Scripts/UI/ObjectiveUI.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ObjectiveUI : MonoBehaviour
{
    [Header("Objective Display")]
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveTitle;
    public TextMeshProUGUI objectiveDescription;
    public Image objectiveProgressBar;
    public TextMeshProUGUI objectiveProgressText;
    public Image objectiveIcon;
    public TextMeshProUGUI objectiveTimer;
    
    [Header("Completion Effects")]
    public GameObject completionEffect;
    public TextMeshProUGUI completionText;
    public Color completionColor = Color.green;
    public Color failureColor = Color.red;
    
    void Start()
    {
        // Subscribe to objective events
        ObjectiveManager.OnObjectiveStarted += ShowObjective;
        ObjectiveManager.OnObjectiveCompleted += OnObjectiveCompleted;
        ObjectiveManager.OnObjectiveFailed += OnObjectiveFailed;
        
        // Initialize UI
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (completionEffect != null) completionEffect.SetActive(false);
    }
    
    void Update()
    {
        UpdateObjectiveDisplay();
    }
    
    void ShowObjective(ObjectiveManager.Objective objective)
    {
        if (objectivePanel != null) objectivePanel.SetActive(true);
        
        // Set objective info
        if (objectiveTitle != null)
            objectiveTitle.text = objective.displayName;
            
        if (objectiveDescription != null)
            objectiveDescription.text = objective.description;
            
        if (objectiveIcon != null && objective.icon != null)
            objectiveIcon.sprite = objective.icon;
        
        // Animate panel entrance
        if (objectivePanel != null)
        {
            objectivePanel.transform.localScale = Vector3.zero;
            objectivePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }
    
    void UpdateObjectiveDisplay()
    {
        var currentObjective = ObjectiveManager.Instance?.GetCurrentObjective();
        if (currentObjective == null) return;
        
        // Update progress bar
        if (objectiveProgressBar != null)
        {
            objectiveProgressBar.fillAmount = currentObjective.GetProgressPercent();
        }
        
        // Update progress text
        if (objectiveProgressText != null)
        {
            objectiveProgressText.text = $"{currentObjective.currentProgress}/{currentObjective.targetValue}";
        }
        
        // Update timer
        if (objectiveTimer != null)
        {
            objectiveTimer.text = $"{currentObjective.timeRemaining:F0}s";
            
            // Color timer based on remaining time
            if (currentObjective.timeRemaining <= 5f)
                objectiveTimer.color = Color.red;
            else if (currentObjective.timeRemaining <= 10f)
                objectiveTimer.color = Color.yellow;
            else
                objectiveTimer.color = Color.white;
        }
    }
    
    void OnObjectiveCompleted(ObjectiveManager.Objective objective)
    {
        ShowCompletionEffect(true);
        HideObjectiveAfterDelay(2f);
    }
    
    void OnObjectiveFailed(ObjectiveManager.Objective objective)
    {
        ShowCompletionEffect(false);
        HideObjectiveAfterDelay(1f);
    }
    
    void ShowCompletionEffect(bool completed)
    {
        if (completionEffect != null)
        {
            completionEffect.SetActive(true);
            
            if (completionText != null)
            {
                completionText.text = completed ? "OBJECTIVE COMPLETE!" : "OBJECTIVE FAILED";
                completionText.color = completed ? completionColor : failureColor;
                
                // Animate completion text
                completionText.transform.localScale = Vector3.zero;
                completionText.transform.DOScale(1.2f, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => {
                        completionText.transform.DOScale(1f, 0.2f);
                    });
            }
        }
        
        // Screen flash effect
        if (SimpleEffects.Instance != null)
        {
            Color flashColor = completed ? completionColor : failureColor;
            SimpleEffects.Instance.ScreenFlash(flashColor, 0.3f);
        }
    }
    
    void HideObjectiveAfterDelay(float delay)
    {
        StartCoroutine(HideAfterDelay(delay));
    }
    
    System.Collections.IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (objectivePanel != null)
        {
            objectivePanel.transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => objectivePanel.SetActive(false));
        }
        
        if (completionEffect != null)
        {
            completionEffect.SetActive(false);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        ObjectiveManager.OnObjectiveStarted -= ShowObjective;
        ObjectiveManager.OnObjectiveCompleted -= OnObjectiveCompleted;
        ObjectiveManager.OnObjectiveFailed -= OnObjectiveFailed;
    }
}
```

---

## 🔧 PARTE 6: SETUP EN UNITY

### Paso 9: Configuración en Unity Scene

1. **Crear GameObjects vacíos**:
   - "EnergyManager" → Attach `EnergyManager.cs`
   - "ObjectiveManager" → Attach `ObjectiveManager.cs`  
   - "WaveManager" → Attach `WaveManager.cs`

2. **Configurar UI Canvas**:
   - Agregar UI elements para energy bar, objective panel, wave display
   - Crear GameObjects con `EnergyUI.cs` y `ObjectiveUI.cs`

3. **Configurar valores iniciales**:
   ```
   EnergyManager:
   - Max Energy: 10
   - Energy Drain Rate: 1
   - Energy Per Hit: 1
   - Energy Per Objective: 3
   
   ObjectiveManager:
   - Objective Interval: 20
   - Objective Time Limit: 30
   
   WaveManager:
   - Distance Per Wave: 200
   ```

4. **Referencias UI**:
   - Conectar todos los UI elements en EnergyUI y ObjectiveUI
   - Configurar gradients y colors

### Paso 10: Testing y Balance

1. **Primera run**: Verificar que energy system funciona básicamente
2. **Balance testing**: Ajustar drain rate y energy gains
3. **Objective testing**: Verificar que objectives aparecen y se completan
4. **Wave testing**: Confirmar transiciones de wave funcionan
5. **UI testing**: Verificar feedback visual es claro

---

## ⚡ EXTENSIONES FUTURAS (SimpleEffects)

Para completar la implementación, agrega estos métodos a `SimpleEffects.cs`:

```csharp
// Add to SimpleEffects.cs
public void PlayShieldActivation()
{
    // Create shield activation effect
    Vector3 playerPos = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;
    
    GameObject shieldEffect = new GameObject("ShieldActivation");
    shieldEffect.transform.position = playerPos;
    
    // Add visual effect here (particles, glow, etc.)
    
    Destroy(shieldEffect, 1f);
}
```

Esta guía te da una implementación completa del Energy System que reemplaza el timer system y añade engagement a través de mini-objetivos y waves.

**¿Necesitas que detalle algún paso específico o hay alguna parte que no esté clara?**