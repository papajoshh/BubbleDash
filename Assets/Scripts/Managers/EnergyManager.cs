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
    
    // Methods called by WaveManager
    public void ApplyWaveDrainMultiplier(float multiplier)
    {
        // This affects the current drain rate calculation
        // Implementation is handled in GetCurrentDrainRate() method
        Debug.Log($"Wave drain multiplier applied: {multiplier}");
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