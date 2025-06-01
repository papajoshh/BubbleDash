using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnergyUI : MonoBehaviour
{
    [Header("Energy Bar")]
    public Slider energySlider;
    public Image energyFillImage;
    public TextMeshProUGUI energyText;
    
    [Header("Energy Colors")]
    public Color highEnergyColor = Color.green;
    public Color mediumEnergyColor = Color.yellow;
    public Color lowEnergyColor = Color.red;
    public Color criticalEnergyColor = new Color(1f, 0.2f, 0.2f);
    
    [Header("Shield Display")]
    public GameObject shieldPanel;
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;
    public Image shieldIcon;
    
    [Header("Safe Zone Display")]
    public GameObject safeZonePanel;
    public TextMeshProUGUI safeZoneText;
    public Image safeZoneBackground;
    
    [Header("Animations")]
    public float animationDuration = 0.3f;
    public AnimationCurve energyAnimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Warning Effects")]
    public float lowEnergyThreshold = 0.3f;
    public float criticalEnergyThreshold = 0.15f;
    public float pulseSpeed = 2f;
    
    private bool isLowEnergyWarning = false;
    private bool isCriticalEnergyWarning = false;
    private Sequence currentPulseSequence;
    
    // For smooth energy bar updates
    private float targetEnergyPercent = 1f;
    private float currentEnergyPercent = 1f;
    private float energyUpdateSpeed = 5f; // How fast the bar catches up
    
    void Awake()
    {
        // Initialize UI state
        if (shieldPanel != null)
            shieldPanel.SetActive(false);
            
        if (safeZonePanel != null)
            safeZonePanel.SetActive(false);
    }
    
    void Start()
    {
        // Subscribe to energy events
        if (EnergyManager.Instance != null)
        {
            EnergyManager.OnEnergyChanged += UpdateEnergyDisplay;
            EnergyManager.OnShieldChanged += UpdateShieldDisplay;
            EnergyManager.OnSafeZoneChanged += UpdateSafeZoneDisplay;
            EnergyManager.OnEnergyEmpty += OnEnergyEmpty;
        }
        
        // Initialize displays
        InitializeEnergyDisplay();
        UpdateShieldDisplay(0f);
        UpdateSafeZoneDisplay(false);
    }
    
    void Update()
    {
        // Smoothly update energy bar
        if (energyFillImage != null && Mathf.Abs(currentEnergyPercent - targetEnergyPercent) > 0.001f)
        {
            currentEnergyPercent = Mathf.Lerp(currentEnergyPercent, targetEnergyPercent, Time.deltaTime * energyUpdateSpeed);
            energyFillImage.fillAmount = currentEnergyPercent;
        }
    }
    
    void InitializeEnergyDisplay()
    {
        if (EnergyManager.Instance != null)
        {
            float energyPercent = EnergyManager.Instance.GetEnergyPercent();
            currentEnergyPercent = energyPercent;
            targetEnergyPercent = energyPercent;
            
            if (energyFillImage != null)
            {
                energyFillImage.fillAmount = energyPercent;
            }
            
            UpdateEnergyDisplay(EnergyManager.Instance.currentEnergy);
        }
        else
        {
            // Default state
            currentEnergyPercent = 1f;
            targetEnergyPercent = 1f;
            
            if (energyFillImage != null)
            {
                energyFillImage.fillAmount = 1f;
            }
            
            UpdateEnergyDisplay(10f);
        }
    }
    
    void UpdateEnergyDisplay(float currentEnergy)
    {
        if (EnergyManager.Instance == null) return;
        
        float energyPercent = EnergyManager.Instance.GetEnergyPercent();
        
        // Update target for smooth interpolation
        targetEnergyPercent = energyPercent;
        
        // Update slider if using one
        if (energySlider != null)
        {
            energySlider.value = energyPercent;
        }
        
        // Update text
        if (energyText != null)
        {
            energyText.text = $"{Mathf.Ceil(currentEnergy)}";
        }
        
        // Update color based on energy level
        Color targetColor = GetEnergyColor(energyPercent);
        if (energyFillImage != null)
        {
            energyFillImage.color = targetColor;
        }
        
        // Handle warning states
        HandleEnergyWarnings(energyPercent);
    }
    
    Color GetEnergyColor(float energyPercent)
    {
        if (energyPercent > 0.6f)
            return highEnergyColor;
        else if (energyPercent > 0.3f)
            return Color.Lerp(mediumEnergyColor, highEnergyColor, (energyPercent - 0.3f) / 0.3f);
        else if (energyPercent > 0.15f)
            return Color.Lerp(lowEnergyColor, mediumEnergyColor, (energyPercent - 0.15f) / 0.15f);
        else
            return criticalEnergyColor;
    }
    
    void HandleEnergyWarnings(float energyPercent)
    {
        bool shouldShowCritical = energyPercent <= criticalEnergyThreshold;
        bool shouldShowLow = energyPercent <= lowEnergyThreshold && !shouldShowCritical;
        
        // Critical energy warning
        if (shouldShowCritical && !isCriticalEnergyWarning)
        {
            StartCriticalWarning();
        }
        else if (!shouldShowCritical && isCriticalEnergyWarning)
        {
            StopCriticalWarning();
        }
        
        // Low energy warning
        if (shouldShowLow && !isLowEnergyWarning)
        {
            StartLowEnergyWarning();
        }
        else if (!shouldShowLow && isLowEnergyWarning)
        {
            StopLowEnergyWarning();
        }
        
        // Stop all warnings if energy is good
        if (energyPercent > lowEnergyThreshold)
        {
            StopAllWarnings();
        }
    }
    
    void StartLowEnergyWarning()
    {
        isLowEnergyWarning = true;
        
        // Gentle pulse effect
        if (energySlider != null && currentPulseSequence == null)
        {
            currentPulseSequence = DOTween.Sequence()
                .Append(energySlider.transform.DOScale(1.05f, 1f / pulseSpeed))
                .Append(energySlider.transform.DOScale(1f, 1f / pulseSpeed))
                .SetLoops(-1, LoopType.Restart);
        }
        
        // Play warning sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayEnergyLow();
        }
    }
    
    void StartCriticalWarning()
    {
        isCriticalEnergyWarning = true;
        StopLowEnergyWarning(); // Stop low warning
        
        // Intense pulse effect
        if (energySlider != null)
        {
            currentPulseSequence?.Kill();
            currentPulseSequence = DOTween.Sequence()
                .Append(energySlider.transform.DOScale(1.1f, 0.5f / pulseSpeed))
                .Append(energySlider.transform.DOScale(1f, 0.5f / pulseSpeed))
                .SetLoops(-1, LoopType.Restart);
        }
        
        // Flash the fill color
        if (energyFillImage != null)
        {
            energyFillImage.DOColor(Color.white, 0.2f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
    
    void StopLowEnergyWarning()
    {
        isLowEnergyWarning = false;
        StopPulseEffect();
    }
    
    void StopCriticalWarning()
    {
        isCriticalEnergyWarning = false;
        StopPulseEffect();
        
        // Stop color flashing
        if (energyFillImage != null)
        {
            energyFillImage.DOKill();
        }
    }
    
    void StopAllWarnings()
    {
        StopLowEnergyWarning();
        StopCriticalWarning();
    }
    
    void StopPulseEffect()
    {
        if (currentPulseSequence != null)
        {
            currentPulseSequence.Kill();
            currentPulseSequence = null;
        }
        
        if (energySlider != null)
        {
            energySlider.transform.DOScale(1f, 0.2f);
        }
    }
    
    void UpdateShieldDisplay(float shieldTime)
    {
        bool hasShield = shieldTime > 0;
        
        if (shieldPanel != null)
        {
            if (hasShield && !shieldPanel.activeInHierarchy)
            {
                shieldPanel.SetActive(true);
                shieldPanel.transform.localScale = Vector3.zero;
                shieldPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }
            else if (!hasShield && shieldPanel.activeInHierarchy)
            {
                shieldPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
                    .OnComplete(() => shieldPanel.SetActive(false));
            }
        }
        
        if (hasShield)
        {
            // Update shield slider
            if (shieldSlider != null && EnergyManager.Instance != null)
            {
                float shieldPercent = shieldTime / EnergyManager.Instance.maxShieldTime;
                shieldSlider.DOValue(shieldPercent, 0.1f);
            }
            
            // Update shield text
            if (shieldText != null)
            {
                shieldText.text = $"{Mathf.Ceil(shieldTime)}s";
            }
            
            // Animate shield icon
            if (shieldIcon != null)
            {
                shieldIcon.transform.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart);
            }
        }
        else
        {
            // Stop shield icon animation
            if (shieldIcon != null)
            {
                shieldIcon.transform.DOKill();
                shieldIcon.transform.rotation = Quaternion.identity;
            }
        }
    }
    
    void UpdateSafeZoneDisplay(bool inSafeZone)
    {
        if (safeZonePanel != null)
        {
            if (inSafeZone && !safeZonePanel.activeInHierarchy)
            {
                safeZonePanel.SetActive(true);
                safeZonePanel.transform.localScale = Vector3.zero;
                safeZonePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                
                // Pulsing background effect
                if (safeZoneBackground != null)
                {
                    safeZoneBackground.DOColor(new Color(0f, 1f, 0f, 0.3f), 0.5f)
                        .SetLoops(-1, LoopType.Yoyo);
                }
            }
            else if (!inSafeZone && safeZonePanel.activeInHierarchy)
            {
                safeZonePanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
                    .OnComplete(() => safeZonePanel.SetActive(false));
                    
                // Stop background pulsing
                if (safeZoneBackground != null)
                {
                    safeZoneBackground.DOKill();
                }
            }
        }
        
        if (safeZoneText != null && inSafeZone)
        {
            safeZoneText.text = "SAFE ZONE";
        }
    }
    
    void OnEnergyEmpty()
    {
        // Final energy depletion effect
        if (energySlider != null)
        {
            energySlider.transform.DOShakeScale(0.5f, 0.2f, 10, 90);
        }
        
        // Flash the entire energy UI red
        if (energyFillImage != null)
        {
            energyFillImage.DOColor(Color.red, 0.1f)
                .SetLoops(6, LoopType.Yoyo);
        }
    }
    
    // Public method for manual energy display (for testing)
    public void SetEnergyDisplay(float energy, float maxEnergy)
    {
        if (energySlider != null)
        {
            energySlider.value = energy / maxEnergy;
        }
        
        if (energyText != null)
        {
            energyText.text = $"{Mathf.Ceil(energy)}";
        }
        
        if (energyFillImage != null)
        {
            energyFillImage.color = GetEnergyColor(energy / maxEnergy);
        }
    }
    
    void OnDestroy()
    {
        // Clean up events and animations
        if (EnergyManager.Instance != null)
        {
            EnergyManager.OnEnergyChanged -= UpdateEnergyDisplay;
            EnergyManager.OnShieldChanged -= UpdateShieldDisplay;
            EnergyManager.OnSafeZoneChanged -= UpdateSafeZoneDisplay;
            EnergyManager.OnEnergyEmpty -= OnEnergyEmpty;
        }
        
        // Kill any running animations
        DOTween.Kill(transform);
        currentPulseSequence?.Kill();
    }
}