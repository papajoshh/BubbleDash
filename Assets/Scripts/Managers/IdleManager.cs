using UnityEngine;
using System;

public class IdleManager : MonoBehaviour
{
    public static IdleManager Instance { get; private set; }
    
    [Header("Idle Settings")]
    public bool enableIdleProgression = true;
    public float idleCoinsPerSecond = 0.1f; // Base idle earning rate
    public float maxIdleHours = 8f; // Max offline time to calculate
    public int maxIdleCoins = 500; // Cap on idle earnings
    
    [Header("Idle Multipliers")]
    public float speedUpgradeMultiplier = 0.05f; // Each speed upgrade adds 5% idle rate
    
    [Header("UI References")]
    public IdleRewardsUI idleRewardsUI; // Drag from scene
    
    private const string LAST_SAVE_TIME_KEY = "LastSaveTime";
    private const string IDLE_COINS_RATE_KEY = "IdleCoinsRate";
    
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
        if (enableIdleProgression)
        {
            CheckIdleProgress();
        }
        
        // Subscribe to application events
        Application.focusChanged += OnApplicationFocus;
    }
    
    void CheckIdleProgress()
    {
        string lastSaveTimeString = PlayerPrefs.GetString(LAST_SAVE_TIME_KEY, "");
        
        if (string.IsNullOrEmpty(lastSaveTimeString))
        {
            // First time playing, just save current time
            SaveCurrentTime();
            return;
        }
        
        // Parse the saved time
        if (DateTime.TryParse(lastSaveTimeString, out DateTime lastSaveTime))
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = currentTime - lastSaveTime;
            
            // Only calculate if offline for more than 30 seconds
            if (timeDifference.TotalSeconds > 30)
            {
                CalculateIdleRewards(timeDifference);
            }
        }
        
        SaveCurrentTime();
    }
    
    void CalculateIdleRewards(TimeSpan offlineTime)
    {
        // Cap the offline time
        double offlineHours = Math.Min(offlineTime.TotalHours, maxIdleHours);
        double offlineSeconds = offlineHours * 3600;
        
        // Calculate base idle rate
        float currentIdleRate = GetCurrentIdleRate();
        
        // Calculate total coins earned
        int coinsEarned = Mathf.FloorToInt((float)(offlineSeconds * currentIdleRate));
        coinsEarned = Mathf.Min(coinsEarned, maxIdleCoins);
        
        if (coinsEarned > 0)
        {
            ShowIdleRewardsPopup(offlineTime, coinsEarned);
        }
    }
    
    float GetCurrentIdleRate()
    {
        float rate = idleCoinsPerSecond;
        
        // Add bonuses from upgrades
        if (UpgradeSystem.Instance != null)
        {
            var upgrades = UpgradeSystem.Instance.GetAllUpgrades();
            foreach (var upgrade in upgrades)
            {
                if (upgrade.id == "speed_boost" && upgrade.currentLevel > 0)
                {
                    rate += upgrade.currentLevel * speedUpgradeMultiplier;
                }
            }
        }
        
        return rate;
    }
    
    void ShowIdleRewardsPopup(TimeSpan offlineTime, int coinsEarned)
    {
        // Create a simple popup
        Debug.Log($"Idle Rewards: Offline for {FormatTime(offlineTime)}, earned {coinsEarned} coins!");
        
        // Add coins to system
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(coinsEarned);
        }
        
        // Show UI popup
        if (idleRewardsUI != null)
        {
            idleRewardsUI.ShowRewards(offlineTime, coinsEarned);
        }
        else
        {
            // Fallback: Show in console and add to player
            Debug.Log($"IDLE PROGRESS: You were away for {FormatTime(offlineTime)} and earned {coinsEarned} coins!");
        }
    }
    
    string FormatTime(TimeSpan time)
    {
        if (time.TotalDays >= 1)
            return $"{(int)time.TotalDays}d {time.Hours}h {time.Minutes}m";
        else if (time.TotalHours >= 1)
            return $"{time.Hours}h {time.Minutes}m";
        else
            return $"{time.Minutes}m {time.Seconds}s";
    }
    
    void SaveCurrentTime()
    {
        string currentTimeString = DateTime.Now.ToString();
        PlayerPrefs.SetString(LAST_SAVE_TIME_KEY, currentTimeString);
        
        // Also save current idle rate for reference
        PlayerPrefs.SetFloat(IDLE_COINS_RATE_KEY, GetCurrentIdleRate());
        PlayerPrefs.Save();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (enableIdleProgression)
        {
            if (hasFocus)
            {
                // App gained focus - check for idle progress
                CheckIdleProgress();
            }
            else
            {
                // App lost focus - save current time
                SaveCurrentTime();
            }
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (enableIdleProgression)
        {
            if (pauseStatus)
            {
                // App paused - save time
                SaveCurrentTime();
            }
            else
            {
                // App resumed - check progress
                CheckIdleProgress();
            }
        }
    }
    
    void OnDestroy()
    {
        Application.focusChanged -= OnApplicationFocus;
        SaveCurrentTime();
    }
    
    // Public methods
    public float GetIdleCoinsPerSecond()
    {
        return GetCurrentIdleRate();
    }
    
    public void ForceIdleCalculation()
    {
        CheckIdleProgress();
    }
    
    public void ResetIdleProgress()
    {
        PlayerPrefs.DeleteKey(LAST_SAVE_TIME_KEY);
        PlayerPrefs.DeleteKey(IDLE_COINS_RATE_KEY);
    }
}