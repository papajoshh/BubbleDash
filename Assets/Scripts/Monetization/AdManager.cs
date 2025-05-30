using UnityEngine;
using System;
using System.Collections;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    
    [Header("AdMob IDs")]
    private const string APP_ID = "ca-app-pub-8144713847135030";
    private const string REWARDED_AD_ID = "ca-app-pub-8144713847135030/3740665346";
    private const string INTERSTITIAL_AD_ID = "ca-app-pub-8144713847135030/8801420338";
    private const string BANNER_AD_ID = "ca-app-pub-8144713847135030/2116576573";
    
    [Header("Ad Settings")]
    public bool testMode = true;
    public int gamesUntilInterstitial = 3;
    
    [Header("Simulation Settings")]
    public bool simulateAds = true; // For testing without actual ads
    public float simulatedAdDuration = 3f;
    
    private int gamesSinceLastInterstitial = 0;
    private Action<bool> rewardedAdCallback;
    private bool isRewardedAdReady = true; // Always ready in simulation
    private bool isInterstitialAdReady = true;
    private bool isShowingAd = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAds()
    {
        Debug.Log($"AdManager initialized with App ID: {APP_ID}");
        Debug.Log("IMPORTANT: To use real ads, integrate Google Mobile Ads SDK or Unity LevelPlay");
        
        if (simulateAds)
        {
            Debug.Log("Ad Simulation Mode: ON");
        }
    }
    
    #region Rewarded Ads
    
    public void ShowRewardedAd(Action<bool> callback)
    {
        if (isShowingAd)
        {
            Debug.LogWarning("Already showing an ad!");
            return;
        }
        
        rewardedAdCallback = callback;
        
        if (simulateAds)
        {
            StartCoroutine(SimulateRewardedAd());
        }
        else
        {
            // TODO: Implement actual AdMob rewarded ad
            Debug.LogWarning("Real ads not implemented yet. Enable simulateAds for testing.");
            callback?.Invoke(false);
        }
    }
    
    IEnumerator SimulateRewardedAd()
    {
        isShowingAd = true;
        
        Debug.Log("=== SIMULATED REWARDED AD ===");
        Debug.Log($"Ad ID: {REWARDED_AD_ID}");
        Debug.Log("User watching ad for Double Coins...");
        
        // Pause game
        Time.timeScale = 0f;
        
        // Wait for simulated duration
        yield return new WaitForSecondsRealtime(simulatedAdDuration);
        
        Debug.Log("Ad completed successfully!");
        
        // Resume game
        Time.timeScale = 1f;
        
        isShowingAd = false;
        rewardedAdCallback?.Invoke(true);
        rewardedAdCallback = null;
    }
    
    public bool IsRewardedAdReady()
    {
        return isRewardedAdReady && !isShowingAd;
    }
    
    #endregion
    
    #region Interstitial Ads
    
    public void ShowInterstitialAd()
    {
        if (isShowingAd)
        {
            Debug.LogWarning("Already showing an ad!");
            return;
        }
        
        gamesSinceLastInterstitial++;
        
        if (gamesSinceLastInterstitial >= gamesUntilInterstitial)
        {
            if (simulateAds)
            {
                StartCoroutine(SimulateInterstitialAd());
            }
            else
            {
                // TODO: Implement actual AdMob interstitial
                Debug.LogWarning("Real ads not implemented yet. Enable simulateAds for testing.");
            }
            
            gamesSinceLastInterstitial = 0;
        }
    }
    
    IEnumerator SimulateInterstitialAd()
    {
        isShowingAd = true;
        
        Debug.Log("=== SIMULATED INTERSTITIAL AD ===");
        Debug.Log($"Ad ID: {INTERSTITIAL_AD_ID}");
        Debug.Log("Showing interstitial ad...");
        
        // Pause game
        Time.timeScale = 0f;
        
        // Wait for simulated duration
        yield return new WaitForSecondsRealtime(simulatedAdDuration);
        
        Debug.Log("Interstitial ad closed");
        
        // Resume game
        Time.timeScale = 1f;
        
        isShowingAd = false;
    }
    
    public bool ShouldShowInterstitial()
    {
        return gamesSinceLastInterstitial >= gamesUntilInterstitial && !isShowingAd;
    }
    
    public void IncrementGameCount()
    {
        gamesSinceLastInterstitial++;
    }
    
    #endregion
    
    #region Banner Ads
    
    public void ShowBanner()
    {
        if (simulateAds)
        {
            Debug.Log($"=== SIMULATED BANNER AD ===");
            Debug.Log($"Ad ID: {BANNER_AD_ID}");
            Debug.Log("Banner ad shown at bottom of screen");
        }
        else
        {
            // TODO: Implement actual AdMob banner
            Debug.LogWarning("Real banner ads not implemented yet.");
        }
    }
    
    public void HideBanner()
    {
        if (simulateAds)
        {
            Debug.Log("Banner ad hidden");
        }
    }
    
    #endregion
    
    #region Google Mobile Ads Integration Guide
    
    /* 
    INTEGRATION STEPS FOR GOOGLE MOBILE ADS:
    
    1. Install Google Mobile Ads Unity Plugin:
       - Download from: https://github.com/googleads/googleads-mobile-unity/releases
       - Or via Package Manager: https://github.com/googleads/googleads-mobile-unity.git
    
    2. Configure AndroidManifest.xml:
       Add your App ID to the manifest
    
    3. Replace the simulated methods with actual AdMob code:
       
       Example for Rewarded Ad:
       ```
       private RewardedAd rewardedAd;
       
       private void LoadRewardedAd()
       {
           AdRequest request = new AdRequest.Builder().Build();
           RewardedAd.Load(REWARDED_AD_ID, request, (RewardedAd ad, LoadAdError error) =>
           {
               if (error != null || ad == null)
               {
                   Debug.LogError("Rewarded ad failed to load: " + error);
                   return;
               }
               rewardedAd = ad;
               isRewardedAdReady = true;
           });
       }
       ```
    
    4. For Unity LevelPlay (ironSource):
       - Different integration process
       - Supports mediation with multiple ad networks
       - Better fill rates and eCPM optimization
    */
    
    #endregion
}