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
    private float noMissTimer = 0f;
    private bool isTrackingNoMiss = false;
    
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
        
        // Special tracking for time-based objectives
        public float startTime;
        public int hitCountAtStart;
        
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
    }
    
    void Update()
    {
        // Update no-miss timer if tracking
        if (isTrackingNoMiss && isGameRunning)
        {
            noMissTimer += Time.deltaTime;
            
            // Update progress for NoMissStreak objectives
            if (currentObjective != null && currentObjective.type == ObjectiveType.NoMissStreak)
            {
                int secondsElapsed = Mathf.FloorToInt(noMissTimer);
                currentObjective.currentProgress = secondsElapsed;
                CheckObjectiveCompletion();
            }
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
        noMissTimer = 0f;
        isTrackingNoMiss = true; // Start tracking from beginning
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
            icon = selectedData.icon,
            startTime = Time.time,
            hitCountAtStart = totalHits
        };
        
        // Special initialization for certain objective types
        if (currentObjective.type == ObjectiveType.NoMissStreak)
        {
            noMissTimer = 0f; // Reset no-miss timer for this objective
        }
        
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
                    
                case ObjectiveType.NoMissStreak:
                    // Only offer if player has reasonable accuracy
                    float recentAccuracy = totalHits + totalMisses > 0 ? (float)totalHits / (totalHits + totalMisses) : 1f;
                    if (recentAccuracy < 0.6f) adjustedWeight *= 0.3f;
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
        while (objective.timeRemaining > 0 && !objective.isCompleted && !objective.isFailed && isGameRunning)
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
                else
                {
                    currentObjective.currentProgress = consecutiveHits;
                }
                break;
                
            case ObjectiveType.HitSpecificColor:
                // Would need to track specific color - implement if needed
                break;
                
            case ObjectiveType.PerfectAccuracy:
                // Count hits since objective started
                int hitsSinceStart = totalHits - currentObjective.hitCountAtStart;
                currentObjective.currentProgress = hitsSinceStart;
                break;
        }
        
        CheckObjectiveCompletion();
    }
    
    public void OnBubbleMiss()
    {
        totalMisses++;
        consecutiveHits = 0;
        
        // Reset no-miss timer
        noMissTimer = 0f;
        
        // Reset certain objectives on miss
        if (currentObjective != null && !currentObjective.isCompleted)
        {
            if (currentObjective.type == ObjectiveType.NoMissStreak)
            {
                currentObjective.currentProgress = 0;
            }
            else if (currentObjective.type == ObjectiveType.PerfectAccuracy)
            {
                // Check if this miss happened during the objective period
                FailCurrentObjective();
            }
            else if (currentObjective.type == ObjectiveType.GetCombo)
            {
                currentObjective.currentProgress = 0; // Reset combo progress
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
            else
            {
                currentObjective.currentProgress = momentum;
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
    
    public int GetConsecutiveHits()
    {
        return consecutiveHits;
    }
    
    public float GetAccuracy()
    {
        int totalShots = totalHits + totalMisses;
        return totalShots > 0 ? (float)totalHits / totalShots : 1f;
    }
    
    // Methods called by WaveManager
    public void ApplyWaveFrequencyMultiplier(float multiplier)
    {
        // Adjust objective interval based on wave
        objectiveInterval = 20f / multiplier; // Faster spawning with higher multiplier
        Debug.Log($"Objective frequency multiplier applied: {multiplier}, new interval: {objectiveInterval}s");
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