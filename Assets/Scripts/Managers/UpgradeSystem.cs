using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Upgrade
{
    public string id;
    public string name;
    public string description;
    public int baseCost;
    public int costIncrement;
    public int currentLevel;
    public int maxLevel;
    public float valuePerLevel;
    
    public int GetCost()
    {
        return baseCost + (costIncrement * currentLevel);
    }
    
    public float GetCurrentValue()
    {
        return valuePerLevel * currentLevel;
    }
    
    public bool IsMaxLevel()
    {
        return currentLevel >= maxLevel;
    }
}

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance { get; private set; }
    
    [Header("Upgrades")]
    public List<Upgrade> upgrades = new List<Upgrade>();
    
    [Header("References")]
    private PlayerController playerController;
    private BubbleShooter bubbleShooter;
    private MomentumSystem momentumSystem;
    
    // Events
    public Action<Upgrade> OnUpgradePurchased;
    public Action<string> OnUpgradeFailed;
    
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
        InitializeUpgrades();
        LoadUpgrades();
        FindReferences();
        ApplyAllUpgrades();
    }
    
    void InitializeUpgrades()
    {
        // Speed Upgrade
        upgrades.Add(new Upgrade
        {
            id = "speed_boost",
            name = "Speed Boost",
            description = "Increase base movement speed",
            baseCost = 50,
            costIncrement = 25,
            currentLevel = 0,
            maxLevel = 10,
            valuePerLevel = 0.5f // +0.5 speed per level
        });
        
        // Fire Rate Upgrade
        upgrades.Add(new Upgrade
        {
            id = "fire_rate",
            name = "Rapid Fire",
            description = "Reduce shooting cooldown",
            baseCost = 75,
            costIncrement = 35,
            currentLevel = 0,
            maxLevel = 8,
            valuePerLevel = 0.02f // -0.02s cooldown per level
        });
        
        // Momentum Gain Upgrade
        upgrades.Add(new Upgrade
        {
            id = "momentum_gain",
            name = "Momentum Master",
            description = "Increase speed gain per combo",
            baseCost = 100,
            costIncrement = 50,
            currentLevel = 0,
            maxLevel = 5,
            valuePerLevel = 0.1f // +10% momentum gain per level
        });
        
        // Golden Touch Upgrade (Coin Multiplier)
        upgrades.Add(new Upgrade
        {
            id = "golden_touch",
            name = "Golden Touch",
            description = "Coin bubbles give +1 extra coin",
            baseCost = 100,
            costIncrement = 100,
            currentLevel = 0,
            maxLevel = 4,
            valuePerLevel = 1f // +1 coin per level
        });
        
        // Bubble Breaker Upgrade (VIP ONLY)
        upgrades.Add(new Upgrade
        {
            id = "bubble_breaker",
            name = "Bubble Breaker [VIP]",
            description = "ALL colors break ANY coin bubble",
            baseCost = 0, // VIP only, not purchasable with coins
            costIncrement = 0,
            currentLevel = 0,
            maxLevel = 1,
            valuePerLevel = 1f
        });
        
        // Auto Pop Upgrade (Premium)
        upgrades.Add(new Upgrade
        {
            id = "auto_pop",
            name = "Auto Pop",
            description = "Coin bubbles pop when you pass below",
            baseCost = 2000,
            costIncrement = 0, // One-time purchase
            currentLevel = 0,
            maxLevel = 1,
            valuePerLevel = 1f
        });
        
        // Starting Combo + Time Extension Upgrade
        upgrades.Add(new Upgrade
        {
            id = "start_combo",
            name = "Head Start",
            description = "Start with combo + extra time",
            baseCost = 150,
            costIncrement = 75,
            currentLevel = 0,
            maxLevel = 3,
            valuePerLevel = 2f // +2 starting combo per level
        });
        
        // Coin Spawn Rate Upgrade
        upgrades.Add(new Upgrade
        {
            id = "coin_spawn_rate",
            name = "Lucky Coins",
            description = "Increase coin spawn chance by +5%",
            baseCost = 40,
            costIncrement = 20,
            currentLevel = 0,
            maxLevel = 5,
            valuePerLevel = 0.05f // +5% per level (5% base + 25% max = 30% total)
        });
    }
    
    void FindReferences()
    {
        playerController = FindObjectOfType<PlayerController>();
        bubbleShooter = FindObjectOfType<BubbleShooter>();
        momentumSystem = FindObjectOfType<MomentumSystem>();
    }
    
    public bool PurchaseUpgrade(string upgradeId)
    {
        Upgrade upgrade = GetUpgrade(upgradeId);
        if (upgrade == null)
        {
            OnUpgradeFailed?.Invoke("Upgrade not found!");
            return false;
        }
        
        if (upgrade.IsMaxLevel())
        {
            OnUpgradeFailed?.Invoke("Already at max level!");
            return false;
        }
        
        // Check if this is a VIP-only upgrade
        if (upgradeId == "bubble_breaker")
        {
            // Check if player has VIP
            bool hasVIP = PlayerPrefs.GetInt("HasVIP", 0) == 1;
            if (!hasVIP)
            {
                OnUpgradeFailed?.Invoke("VIP required! Tap to unlock VIP");
                return false;
            }
            
            // VIP upgrades are free
            upgrade.currentLevel++;
            ApplyUpgrade(upgrade);
            SaveUpgrades();
            
            OnUpgradePurchased?.Invoke(upgrade);
            
            if (SimpleSoundManager.Instance != null)
                SimpleSoundManager.Instance.PlayButtonClick();
                
            Debug.Log($"Unlocked VIP upgrade: {upgrade.name}");
            return true;
        }
        
        int cost = upgrade.GetCost();
        if (!CoinSystem.Instance.CanAfford(cost))
        {
            OnUpgradeFailed?.Invoke($"Not enough coins! Need {cost}");
            return false;
        }
        
        // Purchase successful
        if (CoinSystem.Instance.SpendCoins(cost))
        {
            upgrade.currentLevel++;
            ApplyUpgrade(upgrade);
            SaveUpgrades();
            
            OnUpgradePurchased?.Invoke(upgrade);
            
            // Sound feedback
            if (SimpleSoundManager.Instance != null)
                SimpleSoundManager.Instance.PlayButtonClick();
                
            Debug.Log($"Purchased {upgrade.name} Level {upgrade.currentLevel}");
            return true;
        }
        
        return false;
    }
    
    void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.id)
        {
            case "speed_boost":
                if (playerController != null)
                {
                    // Base speed increases
                    float speedBonus = upgrade.GetCurrentValue();
                    playerController.baseSpeed = 5f + speedBonus; // Default 5 + upgrades
                }
                break;
                
            case "fire_rate":
                if (bubbleShooter != null)
                {
                    // Reduce cooldown
                    float cooldownReduction = upgrade.GetCurrentValue();
                    bubbleShooter.shootCooldown = Mathf.Max(0.05f, 0.2f - cooldownReduction);
                }
                break;
                
            case "momentum_gain":
                if (momentumSystem != null)
                {
                    // Increase momentum multiplier
                    float momentumBonus = 1f + upgrade.GetCurrentValue();
                    momentumSystem.speedIncreasePerHit = 0.1f * momentumBonus;
                }
                break;
                
            case "golden_touch":
                // Store golden touch level for coin bubbles to read
                PlayerPrefs.SetInt("GoldenTouchLevel", upgrade.currentLevel);
                break;
                
            case "start_combo":
                if (momentumSystem != null)
                {
                    // Will be applied on game start
                    PlayerPrefs.SetInt("StartingCombo", (int)upgrade.GetCurrentValue());
                    // Also store timer bonus (30 seconds per level)
                    PlayerPrefs.SetFloat("HeadStartBonus", upgrade.GetCurrentValue() * 30f);
                }
                break;
                
            case "bubble_breaker":
                // Store bubble breaker unlock state (VIP feature)
                // When active, ALL colors can break ANY coin bubble
                PlayerPrefs.SetInt("BubbleBreakerUnlocked", upgrade.currentLevel);
                break;
                
            case "auto_pop":
                // Store auto pop unlock state (0 or 1)
                PlayerPrefs.SetInt("AutoPopUnlocked", upgrade.currentLevel);
                break;
                
            case "coin_spawn_rate":
                // Store coin spawn rate bonus for CoinSystem to read
                // Base 5% + (5% * level) = max 30% at level 5
                PlayerPrefs.SetFloat("CoinSpawnRateBonus", upgrade.GetCurrentValue());
                break;
        }
    }
    
    void ApplyAllUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade.currentLevel > 0)
            {
                ApplyUpgrade(upgrade);
            }
        }
    }
    
    public void ApplyStartingUpgrades()
    {
        // Called at the start of each game
        int startingCombo = PlayerPrefs.GetInt("StartingCombo", 0);
        if (startingCombo > 0 && momentumSystem != null)
        {
            for (int i = 0; i < startingCombo; i++)
            {
                momentumSystem.OnSuccessfulHit();
            }
        }
    }
    
    Upgrade GetUpgrade(string id)
    {
        return upgrades.Find(u => u.id == id);
    }
    
    public List<Upgrade> GetAllUpgrades()
    {
        return new List<Upgrade>(upgrades);
    }
    
    public float GetCoinMagnetBonus()
    {
        Upgrade magnet = GetUpgrade("coin_magnet");
        return magnet != null ? magnet.GetCurrentValue() : 0f;
    }
    
    void SaveUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            PlayerPrefs.SetInt($"Upgrade_{upgrade.id}_Level", upgrade.currentLevel);
        }
        PlayerPrefs.Save();
    }
    
    void LoadUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = PlayerPrefs.GetInt($"Upgrade_{upgrade.id}_Level", 0);
        }
    }
    
    public void ResetAllUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = 0;
            PlayerPrefs.DeleteKey($"Upgrade_{upgrade.id}_Level");
        }
        PlayerPrefs.Save();
        
        // Reset to default values
        FindReferences();
        if (playerController != null) playerController.baseSpeed = 5f;
        if (bubbleShooter != null) bubbleShooter.shootCooldown = 0.2f;
        if (momentumSystem != null) momentumSystem.speedIncreasePerHit = 0.1f;
        
        PlayerPrefs.DeleteKey("CoinMagnetBonus");
        PlayerPrefs.DeleteKey("StartingCombo");
    }
}