using UnityEngine;
using System;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    
    [Header("Timer Settings")]
    public float baseDurationSeconds = 180f; // 3 minutes base
    public bool enableTimer = true;
    public bool pauseTimerOnGamePause = true;
    
    [Header("Warning Thresholds")]
    public float urgentWarningTime = 30f; // Last 30 seconds
    public float warningTime = 60f; // Last minute
    
    [Header("Timer Extensions")]
    public float timeExtensionBonus = 0f; // From upgrades
    
    // Events
    public static event System.Action<float> OnTimerChanged;
    public static event System.Action OnTimerWarning;
    public static event System.Action OnTimerUrgent;
    public static event System.Action OnTimerExpired;
    
    // State
    private float currentTime;
    private float totalDuration;
    private bool isRunning = false;
    private bool hasWarned = false;
    private bool hasUrgentWarned = false;
    private bool isExpired = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Calculate total duration including upgrades
        CalculateTotalDuration();
        
        // Subscribe to game state changes
        if (GameManager.Instance != null)
        {
            // We'll add event subscriptions when GameManager events are available
        }
    }
    
    void Update()
    {
        if (!enableTimer || !isRunning || isExpired) return;
        
        // Pause timer if game is paused
        if (pauseTimerOnGamePause && GameManager.Instance != null)
        {
            if (GameManager.Instance.currentState == GameState.Paused)
                return;
        }
        
        // Update timer
        currentTime -= Time.deltaTime;
        
        // Check for warnings
        CheckWarnings();
        
        // Notify listeners
        OnTimerChanged?.Invoke(currentTime);
        
        // Check if expired
        if (currentTime <= 0f && !isExpired)
        {
            ExpireTimer();
        }
    }
    
    void CalculateTotalDuration()
    {
        // Base duration
        totalDuration = baseDurationSeconds;
        
        // Add time extension bonuses from upgrades
        float headStartBonus = PlayerPrefs.GetFloat("HeadStartBonus", 0f);
        timeExtensionBonus = headStartBonus;
        totalDuration += timeExtensionBonus;
        
        Debug.Log($"Timer Duration: {totalDuration}s (Base: {baseDurationSeconds}s + Bonus: {timeExtensionBonus}s)");
    }
    
    public void StartTimer()
    {
        if (!enableTimer) return;
        
        CalculateTotalDuration();
        currentTime = totalDuration;
        isRunning = true;
        isExpired = false;
        hasWarned = false;
        hasUrgentWarned = false;
        
        Debug.Log($"Timer started: {totalDuration} seconds");
        OnTimerChanged?.Invoke(currentTime);
    }
    
    public void PauseTimer()
    {
        isRunning = false;
        Debug.Log("Timer paused");
    }
    
    public void ResumeTimer()
    {
        if (!isExpired)
        {
            isRunning = true;
            Debug.Log("Timer resumed");
        }
    }
    
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer stopped");
    }
    
    public void AddTime(float seconds)
    {
        if (isExpired) return;
        
        currentTime += seconds;
        Debug.Log($"Added {seconds} seconds to timer. New time: {currentTime}");
        OnTimerChanged?.Invoke(currentTime);
    }
    
    void CheckWarnings()
    {
        // Urgent warning (last 30 seconds)
        if (!hasUrgentWarned && currentTime <= urgentWarningTime)
        {
            hasUrgentWarned = true;
            OnTimerUrgent?.Invoke();
            Debug.Log("URGENT: Timer critical!");
        }
        // Warning (last minute)
        else if (!hasWarned && currentTime <= warningTime)
        {
            hasWarned = true;
            OnTimerWarning?.Invoke();
            Debug.Log("WARNING: Timer running low!");
        }
    }
    
    void ExpireTimer()
    {
        isExpired = true;
        isRunning = false;
        currentTime = 0f;
        
        Debug.Log("TIMER EXPIRED - Game Over!");
        OnTimerExpired?.Invoke();
        
        // Trigger game over
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOverTimer();
        }
    }
    
    // Public getters
    public float GetCurrentTime() => currentTime;
    public float GetTotalDuration() => totalDuration;
    public float GetTimeRemaining() => Mathf.Max(0f, currentTime);
    public float GetTimeElapsed() => totalDuration - currentTime;
    public float GetProgress() => totalDuration > 0 ? (totalDuration - currentTime) / totalDuration : 0f;
    public bool IsRunning() => isRunning && !isExpired;
    public bool IsExpired() => isExpired;
    public bool IsInWarningZone() => currentTime <= warningTime && currentTime > urgentWarningTime;
    public bool IsInUrgentZone() => currentTime <= urgentWarningTime;
    
    // Format time for display
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
    public string GetFormattedTimeWithMilliseconds()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime % 1f) * 100f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
    
    // Reset for new game
    public void ResetTimer()
    {
        StopTimer();
        isExpired = false;
        hasWarned = false;
        hasUrgentWarned = false;
        currentTime = 0f;
    }
    
    void OnDestroy()
    {
        OnTimerChanged = null;
        OnTimerWarning = null;
        OnTimerUrgent = null;
        OnTimerExpired = null;
    }
}