using UnityEngine;

public class MomentumSystem : MonoBehaviour
{
    [Header("Momentum Settings")]
    public float baseSpeed = 3f;
    public float speedIncrement = 0.2f;
    public float speedIncreasePerHit = 0.1f; // For upgrades
    public float maxSpeedMultiplier = 3f;
    public int consecutiveHits { get; private set; }
    
    [Header("Momentum Decay")]
    public float momentumDecayTime = 2f;
    public bool resetOnMiss = true;
    
    private PlayerController playerController;
    private float lastHitTime;
    private float currentSpeedMultiplier = 1f;
    
    public float CurrentSpeed => baseSpeed * currentSpeedMultiplier;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("MomentumSystem requires PlayerController component!");
        }
        
        consecutiveHits = 0;
        lastHitTime = Time.time;
        
        // Apply starting upgrades
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.ApplyStartingUpgrades();
        }
        
        UpdatePlayerSpeed();
    }
    
    void Update()
    {
        CheckMomentumDecay();
    }
    
    public void OnBubbleHit()
    {
        consecutiveHits++;
        lastHitTime = Time.time;
        
        // Calculate new speed multiplier (using upgradeable speedIncreasePerHit)
        currentSpeedMultiplier = Mathf.Min(
            1f + (consecutiveHits * speedIncreasePerHit), 
            maxSpeedMultiplier
        );
        
        UpdatePlayerSpeed();
        
        // Debug feedback
        Debug.Log($"Consecutive hits: {consecutiveHits}, Speed: {CurrentSpeed:F1}x");
        
        // Notify ScoreManager if exists
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnBubbleHit(consecutiveHits);
        }
    }
    
    public void OnBubbleMiss()
    {
        if (resetOnMiss)
        {
            ResetMomentum();
        }
        
        Debug.Log("Bubble missed! Momentum reset.");
    }
    
    public void OnBubbleShot()
    {
        // Called when player shoots, can be used for additional logic
        lastHitTime = Time.time;
    }
    
    void CheckMomentumDecay()
    {
        if (Time.time - lastHitTime > momentumDecayTime && consecutiveHits > 0)
        {
            ResetMomentum();
            Debug.Log("Momentum decayed due to inactivity");
        }
    }
    
    public void ResetMomentum()
    {
        consecutiveHits = 0;
        currentSpeedMultiplier = 1f;
        UpdatePlayerSpeed();
    }
    
    void UpdatePlayerSpeed()
    {
        if (playerController != null)
        {
            playerController.SetSpeed(CurrentSpeed);
        }
    }
    
    // Public getters for UI/other systems
    public float GetSpeedMultiplier()
    {
        return currentSpeedMultiplier;
    }
    
    public int GetComboCount()
    {
        return consecutiveHits;
    }
    
    public bool IsAtMaxSpeed()
    {
        return currentSpeedMultiplier >= maxSpeedMultiplier;
    }
    
    // Method for upgrades to manually add hits
    public void OnSuccessfulHit()
    {
        OnBubbleHit();
    }
}