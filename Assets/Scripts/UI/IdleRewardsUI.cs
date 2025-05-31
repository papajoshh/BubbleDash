using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using DG.Tweening;

public class IdleRewardsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject idleRewardsPanel;
    public TextMeshProUGUI offlineTimeText;
    public TextMeshProUGUI coinsEarnedText;
    public TextMeshProUGUI idleRateText;
    public Button claimButton;
    public Button doubleRewardsButton; // For ad reward
    
    [Header("Visual Effects")]
    public GameObject coinIcon;
    public float animationDuration = 0.5f;
    
    private int pendingCoins = 0;
    private bool rewardsShown = false;
    
    void Awake()
    {
        // Setup buttons
        if (claimButton != null)
            claimButton.onClick.AddListener(ClaimRewards);
            
        if (doubleRewardsButton != null)
            doubleRewardsButton.onClick.AddListener(WatchAdForDoubleRewards);
        
        // Hide panel initially - but don't disable if we're currently showing rewards
        if (idleRewardsPanel != null)
        {
            idleRewardsPanel.SetActive(false);
        }
    }
    
    public void ShowRewards(TimeSpan offlineTime, int coinsEarned)
    {
        Debug.Log($"ShowRewards called: rewardsShown={rewardsShown}, coinsEarned={coinsEarned}");
        
        if (coinsEarned <= 0) 
        {
            Debug.LogWarning($"ShowRewards blocked: coinsEarned={coinsEarned}");
            return;
        }
        
        // Force reset in case it got stuck
        rewardsShown = false;
        
        pendingCoins = coinsEarned;
        
        // Update UI texts
        if (offlineTimeText != null)
            offlineTimeText.text = $"Welcome back!\nYou were away for {FormatTime(offlineTime)}";
            
        if (coinsEarnedText != null)
            coinsEarnedText.text = $"You earned {coinsEarned} coins!";
            
        if (idleRateText != null)
        {
            float rate = IdleManager.Instance?.GetIdleCoinsPerSecond() ?? 0f;
            idleRateText.text = $"Idle rate: {rate:F2} coins/sec";
        }
        
        // Show/hide ad button based on availability
        if (doubleRewardsButton != null)
        {
            bool adAvailable = AdManager.Instance?.IsRewardedAdReady() ?? false;
            doubleRewardsButton.gameObject.SetActive(adAvailable);
        }
        
        // Show panel with animation
        if (idleRewardsPanel != null)
        {
            Debug.Log("Activating idleRewardsPanel...");
            idleRewardsPanel.SetActive(true);
            Debug.Log($"Panel active state: {idleRewardsPanel.activeSelf}");
            
            // Check again after a frame to see if something deactivated it
            StartCoroutine(CheckPanelStateAfterFrame());
            
            // Animate panel entrance
            idleRewardsPanel.transform.localScale = Vector3.zero;
            idleRewardsPanel.transform.DOScale(1f, animationDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(()=> {
                    rewardsShown = true;
                })
                .OnStart(() => {
                    // Pause game while showing rewards
                    if (GameManager.Instance != null)
                        GameManager.Instance.PauseGameSilent();
                });
            
            // Animate coin icon if present
            if (coinIcon != null)
            {
                // Bounce animation
                coinIcon.transform.DOScale(1.2f, 0.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
                
                // Rotation animation
                coinIcon.transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true);
            }
            
            // Animate texts
            AnimateTextElements();
        }
    }
    
    void ClaimRewards()
    {
        if (pendingCoins > 0)
        {
            // Add coins to system
            if (CoinSystem.Instance != null)
            {
                CoinSystem.Instance.AddCoins(pendingCoins);
            }
            
            // Play sound
            if (SimpleSoundManager.Instance != null)
                SimpleSoundManager.Instance.PlayCoinCollect();
            
            Debug.Log($"Claimed {pendingCoins} idle coins!");
        }
        
        CloseRewardsPanel();
    }
    
    void WatchAdForDoubleRewards()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd((success) => {
                if (success)
                {
                    // Double the rewards
                    int bonusCoins = pendingCoins;
                    
                    if (CoinSystem.Instance != null)
                    {
                        CoinSystem.Instance.AddCoins(pendingCoins + bonusCoins);
                    }
                    
                    // Update UI to show doubled amount
                    if (coinsEarnedText != null)
                        coinsEarnedText.text = $"You earned {pendingCoins * 2} coins! (Doubled!)";
                    
                    // Play sound
                    if (SimpleSoundManager.Instance != null)
                        SimpleSoundManager.Instance.PlayCoinCollect();
                    
                    Debug.Log($"Claimed {pendingCoins * 2} idle coins (doubled)!");
                    
                    // Hide double rewards button
                    if (doubleRewardsButton != null)
                        doubleRewardsButton.gameObject.SetActive(false);
                        
                    // Auto-close after a delay
                    Invoke(nameof(CloseRewardsPanel), 1.5f);
                }
                else
                {
                    Debug.Log("Ad failed, claiming normal rewards");
                    ClaimRewards();
                }
            });
        }
        else
        {
            // No ad manager, just claim normal rewards
            ClaimRewards();
        }
    }
    
    void CloseRewardsPanel()
    {
        if (idleRewardsPanel != null)
        {
            // Kill all animations on this panel
            idleRewardsPanel.transform.DOKill();
            if (coinIcon != null)
                coinIcon.transform.DOKill();
            
            // Animate panel exit
            idleRewardsPanel.transform.DOScale(0f, animationDuration * 0.5f)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .OnComplete(() => {
                    idleRewardsPanel.SetActive(false);
                    
                    // Resume game
                    if (GameManager.Instance != null)
                        GameManager.Instance.ResumeGame();
                });
        }
        
        pendingCoins = 0;
        rewardsShown = false;
    }
    
    void AnimateTextElements()
    {
        // Animate coin earned text with counting effect
        if (coinsEarnedText != null && pendingCoins > 0)
        {
            int displayCoins = 0;
            DOTween.To(() => displayCoins, x => displayCoins = x, pendingCoins, 1f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .OnUpdate(() => {
                    coinsEarnedText.text = $"You earned {displayCoins} coins!";
                });
            
            // Pulse effect on final number
            coinsEarnedText.transform.DOScale(1.1f, 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetDelay(1f)
                .SetUpdate(true);
        }
        
        // Fade in other texts
        if (offlineTimeText != null)
        {
            offlineTimeText.DOFade(0f, 0f);
            offlineTimeText.DOFade(1f, animationDuration).SetUpdate(true);
        }
        
        if (idleRateText != null)
        {
            idleRateText.DOFade(0f, 0f);
            idleRateText.DOFade(1f, animationDuration).SetDelay(0.2f).SetUpdate(true);
        }
    }
    
    string FormatTime(TimeSpan time)
    {
        if (time.TotalDays >= 1)
            return $"{(int)time.TotalDays}d {time.Hours}h {time.Minutes}m";
        else if (time.TotalHours >= 1)
            return $"{time.Hours}h {time.Minutes}m";
        else if (time.TotalMinutes >= 1)
            return $"{time.Minutes}m {time.Seconds}s";
        else
            return $"{time.Seconds}s";
    }
    
    // Called when player opens upgrade menu or similar
    public void ResetRewardsFlag()
    {
        rewardsShown = false;
    }
    
    System.Collections.IEnumerator CheckPanelStateAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Wait 2 frames to be sure
        
        if (idleRewardsPanel != null)
        {
            Debug.Log($"Panel state after 2 frames: {idleRewardsPanel.activeSelf}");
            if (!idleRewardsPanel.activeSelf)
            {
                Debug.LogError("PANEL WAS DEACTIVATED BY SOMETHING ELSE!");
                
                // Try to reactivate it
                Debug.Log("Attempting to reactivate panel...");
                idleRewardsPanel.SetActive(true);
                Debug.Log($"Reactivation result: {idleRewardsPanel.activeSelf}");
            }
        }
    }
}