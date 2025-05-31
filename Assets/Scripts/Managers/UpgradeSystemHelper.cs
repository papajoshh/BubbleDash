using UnityEngine;

/// <summary>
/// Helper script to make testing and debugging upgrades easier
/// Attach to any GameObject for quick testing functions
/// </summary>
public class UpgradeSystemHelper : MonoBehaviour
{
    [Header("Quick Test Functions")]
    [SerializeField] private bool enableTestControls = true;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    
    void Update()
    {
        if (!enableTestControls) return;
        
        // Test controls (only in development)
        #if UNITY_EDITOR
        HandleTestInput();
        #endif
    }
    
    void HandleTestInput()
    {
        // Give test coins
        if (Input.GetKeyDown(KeyCode.C))
        {
            GiveTestCoins(100);
        }
        
        // Test upgrade purchase
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestPurchaseUpgrade("speed_boost");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestPurchaseUpgrade("fire_rate");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestPurchaseUpgrade("momentum_gain");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestPurchaseUpgrade("coin_magnet");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestPurchaseUpgrade("start_combo");
        }
        
        // Reset all upgrades
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            ResetAllUpgrades();
        }
        
        // Show upgrade status
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowUpgradeStatus();
        }
    }
    
    [ContextMenu("Give 100 Test Coins")]
    public void GiveTestCoins(int amount = 100)
    {
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(amount);
            Debug.Log($"Added {amount} test coins. Total: {CoinSystem.Instance.GetCurrentCoins()}");
        }
        else
        {
            Debug.LogWarning("CoinSystem instance not found!");
        }
    }
    
    [ContextMenu("Test Speed Upgrade")]
    public void TestSpeedUpgrade()
    {
        TestPurchaseUpgrade("speed_boost");
    }
    
    [ContextMenu("Test Fire Rate Upgrade")]
    public void TestFireRateUpgrade()
    {
        TestPurchaseUpgrade("fire_rate");
    }
    
    [ContextMenu("Show All Upgrade Status")]
    public void ShowUpgradeStatus()
    {
        if (UpgradeSystem.Instance == null)
        {
            Debug.LogWarning("UpgradeSystem instance not found!");
            return;
        }
        
        var upgrades = UpgradeSystem.Instance.GetAllUpgrades();
        Debug.Log("=== UPGRADE STATUS ===");
        
        foreach (var upgrade in upgrades)
        {
            string status = $"{upgrade.name}: Level {upgrade.currentLevel}/{upgrade.maxLevel}";
            if (upgrade.currentLevel > 0)
            {
                status += $" (Value: {upgrade.GetCurrentValue()})";
            }
            status += $" - Next Cost: {upgrade.GetCost()}";
            Debug.Log(status);
        }
        
        // Show current stats
        Debug.Log("=== CURRENT STATS ===");
        
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Debug.Log($"Player Base Speed: {player.baseSpeed}");
        }
        
        BubbleShooter shooter = FindObjectOfType<BubbleShooter>();
        if (shooter != null)
        {
            Debug.Log($"Shoot Cooldown: {shooter.shootCooldown}");
        }
        
        MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
        if (momentum != null)
        {
            Debug.Log($"Speed Increase Per Hit: {momentum.speedIncreasePerHit}");
        }
        
        Debug.Log($"Coin Magnet Bonus: {PlayerPrefs.GetFloat("CoinMagnetBonus", 0f)}");
        Debug.Log($"Starting Combo: {PlayerPrefs.GetInt("StartingCombo", 0)}");
    }
    
    public void TestPurchaseUpgrade(string upgradeId)
    {
        if (UpgradeSystem.Instance == null)
        {
            Debug.LogWarning("UpgradeSystem instance not found!");
            return;
        }
        
        bool success = UpgradeSystem.Instance.PurchaseUpgrade(upgradeId);
        if (success)
        {
            Debug.Log($"Successfully purchased upgrade: {upgradeId}");
        }
        else
        {
            Debug.LogWarning($"Failed to purchase upgrade: {upgradeId}");
        }
    }
    
    [ContextMenu("Reset All Upgrades")]
    public void ResetAllUpgrades()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.ResetAllUpgrades();
            Debug.Log("All upgrades reset to level 0");
        }
        else
        {
            Debug.LogWarning("UpgradeSystem instance not found!");
        }
    }
    
    [ContextMenu("Max All Upgrades (Cheat)")]
    public void MaxAllUpgrades()
    {
        if (UpgradeSystem.Instance == null)
        {
            Debug.LogWarning("UpgradeSystem instance not found!");
            return;
        }
        
        // Give enough coins first
        GiveTestCoins(10000);
        
        var upgrades = UpgradeSystem.Instance.GetAllUpgrades();
        foreach (var upgrade in upgrades)
        {
            while (!upgrade.IsMaxLevel() && CoinSystem.Instance.CanAfford(upgrade.GetCost()))
            {
                UpgradeSystem.Instance.PurchaseUpgrade(upgrade.id);
            }
        }
        
        Debug.Log("All upgrades maxed out!");
        ShowUpgradeStatus();
    }
    
    void OnGUI()
    {
        if (!showDebugInfo || !enableTestControls) return;
        
        #if UNITY_EDITOR
        GUI.Box(new Rect(10, 10, 250, 120), "Upgrade System Debug");
        
        GUILayout.BeginArea(new Rect(15, 35, 240, 100));
        
        GUILayout.Label("Test Controls:");
        GUILayout.Label("C - Add 100 coins");
        GUILayout.Label("1-5 - Buy upgrades");
        GUILayout.Label("Shift+R - Reset upgrades");
        GUILayout.Label("I - Show status");
        
        GUILayout.EndArea();
        #endif
    }
}