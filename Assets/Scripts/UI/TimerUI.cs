using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Display")]
    public TextMeshProUGUI timerText;
    public Image timerFillBar; // Optional circular fill or bar (use Fill Image from Slider)
    public Slider timerSlider; // Alternative: use entire Slider component
    public GameObject timerContainer;
    
    [Header("Visual States")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color urgentColor = Color.red;
    
    [Header("Animation Settings")]
    public bool enablePulseAnimation = true;
    public bool enableShakeOnUrgent = true;
    public float pulseScale = 1.1f;
    public float pulseDuration = 0.5f;
    
    [Header("Sound Effects")]
    public bool enableTimerSounds = true;
    
    private Tween currentPulseTween;
    private Tween currentShakeTween;
    private bool isInWarningState = false;
    private bool isInUrgentState = false;
    
    void Awake()
    {
        // Ensure timer container is visible
        if (timerContainer != null)
        {
            timerContainer.SetActive(true);
        }
    }
    
    void Start()
    {
        // Subscribe to timer events
        TimerManager.OnTimerChanged += UpdateTimerDisplay;
        TimerManager.OnTimerWarning += OnTimerWarning;
        TimerManager.OnTimerUrgent += OnTimerUrgent;
        TimerManager.OnTimerExpired += OnTimerExpired;
        
        // Initialize display
        SetNormalState();
    }
    
    void UpdateTimerDisplay(float timeRemaining)
    {
        if (TimerManager.Instance == null) return;
        
        // Update text
        if (timerText != null)
        {
            timerText.text = TimerManager.Instance.GetFormattedTime();
        }
        
        // Update fill bar (Image) or slider
        float progress = 1f - (timeRemaining / TimerManager.Instance.GetTotalDuration());
        
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = progress;
        }
        
        if (timerSlider != null)
        {
            timerSlider.value = progress;
        }
        
        // Update visual state based on time remaining
        UpdateVisualState(timeRemaining);
    }
    
    void UpdateVisualState(float timeRemaining)
    {
        if (TimerManager.Instance == null) return;
        
        bool shouldBeUrgent = TimerManager.Instance.IsInUrgentZone();
        bool shouldBeWarning = TimerManager.Instance.IsInWarningZone();
        
        if (shouldBeUrgent && !isInUrgentState)
        {
            SetUrgentState();
        }
        else if (shouldBeWarning && !isInWarningState && !isInUrgentState)
        {
            SetWarningState();
        }
        else if (!shouldBeWarning && !shouldBeUrgent && (isInWarningState || isInUrgentState))
        {
            SetNormalState();
        }
    }
    
    void SetNormalState()
    {
        isInWarningState = false;
        isInUrgentState = false;
        
        // Stop animations
        StopCurrentAnimations();
        
        // Set normal color
        if (timerText != null)
        {
            timerText.color = normalColor;
        }
        
        if (timerFillBar != null)
        {
            timerFillBar.color = normalColor;
        }
        
        if (timerSlider != null)
        {
            Image fillImage = timerSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null) fillImage.color = normalColor;
        }
        
        // Reset scale
        if (timerText != null)
        {
            timerText.transform.localScale = Vector3.one;
        }
    }
    
    void SetWarningState()
    {
        isInWarningState = true;
        isInUrgentState = false;
        
        // Stop current animations
        StopCurrentAnimations();
        
        // Set warning color
        if (timerText != null)
        {
            timerText.color = warningColor;
        }
        
        if (timerFillBar != null)
        {
            timerFillBar.color = warningColor;
        }
        
        if (timerSlider != null)
        {
            Image fillImage = timerSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null) fillImage.color = warningColor;
        }
        
        // Start gentle pulse
        if (enablePulseAnimation && timerText != null)
        {
            StartPulseAnimation(pulseDuration);
        }
    }
    
    void SetUrgentState()
    {
        isInWarningState = false;
        isInUrgentState = true;
        
        // Stop current animations
        StopCurrentAnimations();
        
        // Set urgent color
        if (timerText != null)
        {
            timerText.color = urgentColor;
        }
        
        if (timerFillBar != null)
        {
            timerFillBar.color = urgentColor;
        }
        
        if (timerSlider != null)
        {
            Image fillImage = timerSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null) fillImage.color = urgentColor;
        }
        
        // Start fast pulse and shake
        if (enablePulseAnimation && timerText != null)
        {
            StartPulseAnimation(pulseDuration * 0.5f); // Faster pulse
        }
        
        if (enableShakeOnUrgent && timerText != null)
        {
            StartShakeAnimation();
        }
    }
    
    void StartPulseAnimation(float duration)
    {
        if (timerText == null) return;
        
        currentPulseTween = timerText.transform
            .DOScale(pulseScale, duration * 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true); // Continue during pause
    }
    
    void StartShakeAnimation()
    {
        if (timerText == null) return;
        
        currentShakeTween = timerText.transform
            .DOShakePosition(0.5f, strength: 2f, vibrato: 20, randomness: 90)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true); // Continue during pause
    }
    
    void StopCurrentAnimations()
    {
        currentPulseTween?.Kill();
        currentShakeTween?.Kill();
        
        // Reset transform
        if (timerText != null)
        {
            timerText.transform.localScale = Vector3.one;
            timerText.transform.localPosition = Vector3.zero;
        }
    }
    
    void OnTimerWarning()
    {
        Debug.Log("Timer Warning - UI Response");
        
        // Play warning sound
        if (enableTimerSounds && SimpleSoundManager.Instance != null)
        {
            // SimpleSoundManager.Instance.PlayTimerWarning();
        }
        
        // Show warning effect
        if (SimpleEffects.Instance != null)
        {
            // Could add screen effect here
        }
    }
    
    void OnTimerUrgent()
    {
        Debug.Log("Timer Urgent - UI Response");
        
        // Play urgent sound
        if (enableTimerSounds && SimpleSoundManager.Instance != null)
        {
            // SimpleSoundManager.Instance.PlayTimerUrgent();
        }
        
        // Show urgent effect
        if (SimpleEffects.Instance != null)
        {
            // Could add screen flash or similar
        }
    }
    
    void OnTimerExpired()
    {
        Debug.Log("Timer Expired - UI Response");
        
        // Stop all animations
        StopCurrentAnimations();
        
        // Set final state
        if (timerText != null)
        {
            timerText.text = "00:00";
            timerText.color = urgentColor;
        }
        
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 1f;
            timerFillBar.color = urgentColor;
        }
        
        // Play expired sound
        if (enableTimerSounds && SimpleSoundManager.Instance != null)
        {
            // SimpleSoundManager.Instance.PlayTimerExpired();
        }
    }
    
    // Public methods for external control
    public void ShowTimer()
    {
        if (timerContainer != null)
        {
            timerContainer.SetActive(true);
        }
    }
    
    public void HideTimer()
    {
        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }
    }
    
    public void SetTimerVisibility(bool visible)
    {
        if (visible)
            ShowTimer();
        else
            HideTimer();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        TimerManager.OnTimerChanged -= UpdateTimerDisplay;
        TimerManager.OnTimerWarning -= OnTimerWarning;
        TimerManager.OnTimerUrgent -= OnTimerUrgent;
        TimerManager.OnTimerExpired -= OnTimerExpired;
        
        // Stop animations
        StopCurrentAnimations();
    }
}