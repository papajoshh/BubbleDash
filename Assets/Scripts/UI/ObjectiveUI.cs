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
    public Slider objectiveProgressSlider;
    public Image objectiveProgressImage; // Alternative to slider
    public TextMeshProUGUI objectiveProgressText;
    public Image objectiveIcon;
    
    [Header("Timer Display")]
    public TextMeshProUGUI objectiveTimerText;
    
    [Header("Colors")]
    public Color normalTimerColor = Color.green;
    public Color warningTimerColor = Color.yellow;
    public Color criticalTimerColor = Color.red;
    public Color completedColor = Color.cyan;
    public Color failedColor = Color.red;
    
    [Header("Wave Display")]
    public GameObject wavePanel;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveProgressText;
    
    [Header("Feedback Effects")]
    public float animationDuration = 0.3f;
    public float completionEffectDuration = 1f;
    public AnimationCurve bounceAnimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private ObjectiveManager.Objective currentObjective;
    private bool isObjectiveActive = false;
    private Sequence currentTimerPulse;
    
    void Awake()
    {
        // Initialize UI state
        if (objectivePanel != null)
            objectivePanel.SetActive(false);
            
        if (wavePanel != null)
            wavePanel.SetActive(false);
    }
    
    void Start()
    {
        // Subscribe to objective events
        ObjectiveManager.OnObjectiveStarted += ShowObjective;
        ObjectiveManager.OnObjectiveCompleted += OnObjectiveCompleted;
        ObjectiveManager.OnObjectiveFailed += OnObjectiveFailed;
        
        // Subscribe to wave events
        if (WaveManager.Instance != null)
        {
            WaveManager.OnWaveStarted += OnWaveStarted;
            WaveManager.OnWaveCompleted += OnWaveCompleted;
        }
        
        // Initialize wave display
        UpdateWaveDisplay(0, "");
    }
    
    void Update()
    {
        // Update objective progress and timer if active
        if (isObjectiveActive && currentObjective != null)
        {
            UpdateObjectiveProgress();
            UpdateObjectiveTimer();
        }
    }
    
    void ShowObjective(ObjectiveManager.Objective objective)
    {
        currentObjective = objective;
        isObjectiveActive = true;
        
        // Show and animate objective panel
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
            objectivePanel.transform.localScale = Vector3.zero;
            objectivePanel.transform.DOScale(1f, animationDuration)
                .SetEase(Ease.OutBack);
        }
        
        // Timer text will be updated in UpdateObjectiveTimer
        
        // Update objective info
        if (objectiveTitle != null)
        {
            objectiveTitle.text = objective.displayName;
        }
        
        if (objectiveDescription != null)
        {
            objectiveDescription.text = objective.description;
        }
        
        // Initialize progress
        UpdateObjectiveProgress();
        UpdateObjectiveTimer();
        
        // Set icon if available
        if (objectiveIcon != null && objective.icon != null)
        {
            objectiveIcon.sprite = objective.icon;
            objectiveIcon.gameObject.SetActive(true);
        }
        else if (objectiveIcon != null)
        {
            objectiveIcon.gameObject.SetActive(false);
        }
        
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayObjectiveComplete(); // Reuse for start
        }
        
        Debug.Log($"Objective UI: Showing {objective.displayName}");
    }
    
    void UpdateObjectiveProgress()
    {
        if (currentObjective == null) return;
        
        float progress = currentObjective.GetProgressPercent();
        
        // Update progress slider (if using slider)
        if (objectiveProgressSlider != null)
        {
            objectiveProgressSlider.DOValue(progress, 0.1f);
        }
        
        // Update progress image (if using image with fill)
        if (objectiveProgressImage != null)
        {
            objectiveProgressImage.DOFillAmount(progress, 0.1f);
        }
        
        // Update progress text
        if (objectiveProgressText != null)
        {
            objectiveProgressText.text = $"{currentObjective.currentProgress}/{currentObjective.targetValue}";
        }
        
        // Color feedback based on progress
        Image targetImage = null;
        
        // Get the fill image from either slider or direct image
        if (objectiveProgressSlider != null && objectiveProgressSlider.fillRect != null)
        {
            targetImage = objectiveProgressSlider.fillRect.GetComponent<Image>();
        }
        else if (objectiveProgressImage != null)
        {
            targetImage = objectiveProgressImage;
        }
        
        if (targetImage != null)
        {
            if (progress >= 1f)
                targetImage.color = completedColor;
            else if (progress >= 0.75f)
                targetImage.color = Color.Lerp(normalTimerColor, completedColor, (progress - 0.75f) / 0.25f);
            else
                targetImage.color = normalTimerColor;
        }
    }
    
    void UpdateObjectiveTimer()
    {
        if (currentObjective == null) return;
        
        float timePercent = ObjectiveManager.Instance != null ? 
            currentObjective.timeRemaining / ObjectiveManager.Instance.objectiveTimeLimit : 0f;
        
        // Update timer text
        if (objectiveTimerText != null)
        {
            float timeRemaining = Mathf.Ceil(currentObjective.timeRemaining);
            objectiveTimerText.text = $"{timeRemaining}s";
            
            // Update text color based on time remaining
            if (timePercent <= 0.2f) // Critical
            {
                objectiveTimerText.color = criticalTimerColor;
            }
            else if (timePercent <= 0.4f) // Warning
            {
                objectiveTimerText.color = warningTimerColor;
            }
            else // Normal
            {
                objectiveTimerText.color = Color.white;
            }
        }
        
        // Update warning effects if needed
        UpdateTimerWarning(timePercent);
    }
    
    void UpdateTimerWarning(float timePercent)
    {
        bool shouldPulse = false;
        
        if (timePercent <= 0.2f) // 20% time left
        {
            shouldPulse = true;
        }
        
        // Handle pulsing effect on the timer text
        if (shouldPulse && currentTimerPulse == null)
        {
            StartTimerPulse();
        }
        else if (!shouldPulse && currentTimerPulse != null)
        {
            StopTimerPulse();
        }
    }
    
    void StartTimerPulse()
    {
        if (objectiveTimerText == null) return;
        
        // Pulse the timer text instead
        currentTimerPulse = DOTween.Sequence()
            .Append(objectiveTimerText.transform.DOScale(1.2f, 0.3f))
            .Append(objectiveTimerText.transform.DOScale(1f, 0.3f))
            .SetLoops(-1, LoopType.Restart);
    }
    
    void StopTimerPulse()
    {
        if (currentTimerPulse != null)
        {
            currentTimerPulse.Kill();
            currentTimerPulse = null;
        }
        
        if (objectiveTimerText != null)
        {
            objectiveTimerText.transform.DOScale(1f, 0.2f);
        }
    }
    
    void OnObjectiveCompleted(ObjectiveManager.Objective objective)
    {
        if (objective != currentObjective) return;
        
        // Success animation
        if (objectivePanel != null)
        {
            // Green flash
            Image panelImage = objectivePanel.GetComponent<Image>();
            if (panelImage != null)
            {
                Color originalColor = panelImage.color;
                panelImage.DOColor(completedColor, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => panelImage.color = originalColor);
            }
            
            // Bounce effect
            objectivePanel.transform.DOScale(1.2f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => HideObjectiveAfterDelay(completionEffectDuration));
        }
        
        // Update progress one final time to show completion
        if (objectiveProgressSlider != null)
        {
            objectiveProgressSlider.DOValue(1f, 0.3f);
        }
        
        if (objectiveProgressImage != null)
        {
            objectiveProgressImage.DOFillAmount(1f, 0.3f);
        }
        
        if (objectiveProgressText != null)
        {
            objectiveProgressText.text = "COMPLETED!";
            objectiveProgressText.DOColor(completedColor, 0.3f);
        }
        
        // Play completion sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayObjectiveComplete();
        }
        
        // Visual effect
        if (SimpleEffects.Instance != null && objectivePanel != null)
        {
            SimpleEffects.Instance.ShowScorePopup(objectivePanel.transform.position, 100, completedColor);
        }
        
        Debug.Log("Objective UI: Completed animation played");
    }
    
    void OnObjectiveFailed(ObjectiveManager.Objective objective)
    {
        if (objective != currentObjective) return;
        
        // Failure animation
        if (objectivePanel != null)
        {
            // Red flash
            Image panelImage = objectivePanel.GetComponent<Image>();
            if (panelImage != null)
            {
                Color originalColor = panelImage.color;
                panelImage.DOColor(failedColor, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => panelImage.color = originalColor);
            }
            
            // Shake effect
            objectivePanel.transform.DOShakePosition(0.5f, 10f, 10, 90)
                .OnComplete(() => HideObjectiveAfterDelay(0.5f));
        }
        
        if (objectiveProgressText != null)
        {
            objectiveProgressText.text = "FAILED!";
            objectiveProgressText.DOColor(failedColor, 0.3f);
        }
        
        // Play failure sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayObjectiveFailed();
        }
        
        Debug.Log("Objective UI: Failed animation played");
    }
    
    void HideObjectiveAfterDelay(float delay)
    {
        DOVirtual.DelayedCall(delay, () => {
            HideObjective();
        });
    }
    
    void HideObjective()
    {
        isObjectiveActive = false;
        currentObjective = null;
        StopTimerPulse();
        
        // Hide objective panel
        if (objectivePanel != null)
        {
            objectivePanel.transform.DOScale(0f, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => objectivePanel.SetActive(false));
        }
        
        // Timer text is part of ObjectivePanel, so it hides with it
    }
    
    void OnWaveStarted(WaveManager.WaveData waveData, int waveIndex)
    {
        // Update wave display
        UpdateWaveDisplay(waveIndex, waveData.waveName);
        
        // Show wave transition effect
        ShowWaveTransition(waveIndex, waveData.waveName);
    }
    
    void OnWaveCompleted(WaveManager.WaveData waveData, int waveIndex)
    {
        // Could add wave completion effects here if needed
        Debug.Log($"Wave {waveIndex + 1} completed: {waveData.waveName}");
    }
    
    void UpdateWaveDisplay(int waveIndex, string waveName)
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(true);
        }
        
        if (waveText != null)
        {
            waveText.text = $"WAVE {waveIndex + 1}: {waveName}";
        }
        
        if (waveProgressText != null && WaveManager.Instance != null)
        {
            var waveManager = WaveManager.Instance;
            float progress = waveManager.GetWaveProgress();
            waveProgressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }
    
    void ShowWaveTransition(int newWaveIndex, string newWaveName)
    {
        // Create temporary wave transition display
        GameObject transitionObj = new GameObject("WaveTransition");
        transitionObj.transform.SetParent(transform, false);
        
        TextMeshProUGUI transitionText = transitionObj.AddComponent<TextMeshProUGUI>();
        transitionText.text = $"ENTERING WAVE {newWaveIndex + 1}\n{newWaveName}";
        transitionText.fontSize = 48;
        transitionText.color = Color.white;
        transitionText.alignment = TextAlignmentOptions.Center;
        
        // Position in center of screen
        RectTransform rectTransform = transitionObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Animation sequence
        transitionObj.transform.localScale = Vector3.zero;
        Sequence transitionSequence = DOTween.Sequence()
            .Append(transitionObj.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack))
            .AppendInterval(1.5f)
            .Append(transitionText.DOFade(0f, 0.5f))
            .OnComplete(() => Destroy(transitionObj));
        
        // Play wave transition sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayWaveTransition();
        }
        
        Debug.Log($"Wave Transition UI: Showing {newWaveName}");
    }
    
    // Public methods for manual control (testing)
    public void ForceShowObjective(string title, string description, int current, int target)
    {
        var testObjective = new ObjectiveManager.Objective
        {
            displayName = title,
            description = description,
            currentProgress = current,
            targetValue = target,
            timeRemaining = 30f
        };
        
        ShowObjective(testObjective);
    }
    
    public void ForceHideObjective()
    {
        HideObjective();
    }
    
    void OnDestroy()
    {
        // Clean up events
        ObjectiveManager.OnObjectiveStarted -= ShowObjective;
        ObjectiveManager.OnObjectiveCompleted -= OnObjectiveCompleted;
        ObjectiveManager.OnObjectiveFailed -= OnObjectiveFailed;
        
        if (WaveManager.Instance != null)
        {
            WaveManager.OnWaveStarted -= OnWaveStarted;
            WaveManager.OnWaveCompleted -= OnWaveCompleted;
        }
        
        // Kill animations
        DOTween.Kill(transform);
        currentTimerPulse?.Kill();
    }
}