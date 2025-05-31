# COIN SYSTEM IMPLEMENTATION GUIDE

## Overview
This guide implements the complete coin collection system including spawning, collection mechanics, and magnet upgrades.

## Prerequisites
- Basic player movement working
- Obstacle generation system functional
- UpgradeSystem setup (for magnet bonus)

## Step 1: Create Coin Prefab

### 1.1 Basic Coin GameObject
```
1. Create empty GameObject: "Coin"
2. Add SpriteRenderer:
   - Sprite: tile_coin.png from Assets/Art/Sprites/
   - Sorting Layer: Default
   - Order in Layer: 5
   - Color: Yellow (1, 1, 0, 1)
3. Scale: (0.8, 0.8, 1) for appropriate size
```

### 1.2 Add Physics Components
```
1. Add CircleCollider2D:
   - Is Trigger: ✓ checked
   - Radius: 0.3
   - Material: None
2. Do NOT add Rigidbody2D (coins are kinematic)
```

### 1.3 Add Coin Script
```
1. Add Coin.cs script
2. Configure in Inspector:
   - coinValue: 1
   - rotationSpeed: 180
   - bobSpeed: 2
   - bobHeight: 0.2
   - magnetRange: 3
   - magnetSpeed: 8
   - lifetime: 20
```

### 1.4 Save as Prefab
```
1. Drag to Assets/Prefabs/ folder
2. Name: "CoinPrefab"
3. Delete from scene
```

## Step 2: Setup CoinSystem GameObject

### 2.1 Create CoinSystem Manager
```
1. Create empty GameObject: "CoinSystem"
2. Add CoinSystem.cs script
3. Position: (0, 0, 0)
4. Configure in Inspector:
   - coinPrefab: Drag CoinPrefab from Assets/Prefabs/
   - coinSpawnChance: 0.3 (30% chance)
   - minCoinsPerSpawn: 1
   - maxCoinsPerSpawn: 3
   - coinSpacing: 1
   - usePatterns: ✓ checked
```

### 2.2 Verify Singleton
CoinSystem is already configured as singleton - no additional setup needed.

## Step 3: Integrate with Obstacle Generation

### 3.1 Find Your Obstacle Generator
Locate your obstacle spawning script (likely in Gameplay folder).

### 3.2 Add Coin Spawning
Add this code after obstacle creation:
```csharp
// After spawning an obstacle
Vector3 coinSpawnPosition = obstacle.transform.position + Vector3.up * 2f;
if (CoinSystem.Instance != null)
{
    CoinSystem.Instance.SpawnCoinsAtPosition(coinSpawnPosition, 1f);
}
```

### 3.3 Alternative: Manual Spawning Points
If you want specific coin locations:
```csharp
// Spawn coins at exact positions
Vector3[] coinPositions = {
    new Vector3(10, 2, 0),
    new Vector3(15, 3, 0), 
    new Vector3(20, 1, 0)
};

foreach (Vector3 pos in coinPositions)
{
    CoinSystem.Instance.SpawnCoinsAtPosition(pos);
}
```

## Step 4: Setup Player Coin Collection

### 4.1 Verify Player Tag
Ensure your player GameObject has tag "Player":
```
1. Select player GameObject
2. In Inspector, set Tag: "Player"
3. If "Player" tag doesn't exist, create it in Tags & Layers
```

### 4.2 Alternative: Component Detection
If you can't use tags, Coin.cs also detects PlayerController component:
```csharp
// This is already in Coin.cs OnTriggerEnter2D:
if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
```

## Step 5: Test Basic Collection

### 5.1 Quick Test Setup
```
1. Start play mode
2. In Hierarchy, right-click > Create Empty "TestCoin"
3. Add CoinPrefab as child
4. Position near player
5. Move player to collect coin
6. Verify console shows: "Collected 1 coins! Total: X"
```

### 5.2 Verify Events
Check that UI updates when coins are collected (if UIManager is connected).

## Step 6: Configure Coin Patterns

### 6.1 Pattern Types Available
- **2 coins**: Horizontal line
- **3 coins**: Triangle formation  
- **4 coins**: Square formation
- **5 coins**: Cross/plus formation
- **6+ coins**: Horizontal line

### 6.2 Test Different Patterns
```csharp
// Test spawning different amounts
CoinSystem.Instance.SpawnCoinsAtPosition(Vector3.zero, 0f); // Random 1-3
CoinSystem.Instance.SpawnCoinsAtPosition(Vector3.up * 5, 0f); // Different pattern
```

## Step 7: Implement Magnet Upgrade Integration

### 7.1 Verify Magnet Bonus Reading
The Coin.cs script already reads the magnet bonus:
```csharp
// In Coin.Start():
float magnetBonus = PlayerPrefs.GetFloat("CoinMagnetBonus", 0f);
effectiveMagnetRange = magnetRange + magnetBonus;
```

### 7.2 Test Magnet Upgrade
```
1. Purchase coin magnet upgrade in-game
2. Spawn coins near player
3. Verify collection range increases
4. Check gizmo in Scene view shows larger range (when coin selected)
```

## Step 8: Setup Coin UI Display

### 8.1 Connect to UIManager
Ensure UIManager has coin text reference:
```csharp
[Header("Score UI")]
public TextMeshProUGUI coinText;

// In Start():
if (CoinSystem.Instance != null)
{
    CoinSystem.Instance.OnCoinsChanged += UpdateCoinUI;
    UpdateCoinUI(CoinSystem.Instance.GetCurrentCoins());
}

void UpdateCoinUI(int coins)
{
    if (coinText != null)
        coinText.text = $"Coins: {coins}";
}
```

### 8.2 Create Coin UI Element
```
1. In Canvas/HUD, create: UI > Text - TextMeshPro "CoinText"
2. Position: Top-right corner
3. Text: "Coins: 0"
4. Font Size: 24
5. Color: Yellow
6. Assign to UIManager.coinText in Inspector
```

## Step 9: Balancing and Tuning

### 9.1 Recommended Spawn Settings
**For Testing (More Coins)**:
```
coinSpawnChance: 0.8
minCoinsPerSpawn: 2
maxCoinsPerSpawn: 5
```

**For Production (Balanced)**:
```
coinSpawnChance: 0.3
minCoinsPerSpawn: 1  
maxCoinsPerSpawn: 3
```

### 9.2 Coin Values
**Standard Game**:
- Regular coins: 1 value
- Bonus coins from combos: 2-3 value
- Special event coins: 5 value

### 9.3 Magnet Range Progression
```
Base Range: 3 units
Level 1: 3.5 units (+0.5)
Level 5: 5.5 units (+2.5) 
Level 8: 7 units (+4.0) - Max level
```

## Step 10: Debug and Verification

### 10.1 Debug Checklist
- [ ] Coins spawn with obstacles
- [ ] Player can collect coins by touch
- [ ] Magnet pulls coins from distance
- [ ] Coin counter updates in UI
- [ ] Coins add to score (10 points each)
- [ ] Coins save/load properly
- [ ] Magnet upgrade increases range
- [ ] Old coins despawn after lifetime

### 10.2 Common Issues

**Problem**: Coins don't spawn
**Solution**: Check coinPrefab assignment and coinSpawnChance > 0

**Problem**: Can't collect coins  
**Solution**: Verify player has tag "Player" or PlayerController component

**Problem**: Magnet not working
**Solution**: Check UpgradeSystem purchased magnet upgrade and saved to PlayerPrefs

**Problem**: Coins spawn inside obstacles
**Solution**: Adjust spawn position offset: `position + Vector3.up * 2f`

**Problem**: Too many coins lag game
**Solution**: Reduce maxCoinsPerSpawn and implement object pooling if needed

## Step 11: Visual Polish (Optional)

### 11.1 Coin Collection Effects
The Coin.cs already includes:
- Rotation animation (DOTween)
- Bobbing animation (DOTween)  
- Collection scale/rotation effect
- Score popup via SimpleEffects

### 11.2 Sound Effects
Add coin collection sound:
```csharp
// In Coin.CollectCoin():
if (SimpleSoundManager.Instance != null)
{
    SimpleSoundManager.Instance.PlayCoinCollect();
}
```

### 11.3 Particle Effects
Consider adding particle system to coin collection for extra juice.

## Step 12: Advanced Features (Future)

### 12.1 Coin Multipliers
Add temporary coin value multipliers during special events.

### 12.2 Rare Coins
Implement special golden coins worth 5x normal value.

### 12.3 Coin Trails
Add coin collection trails for satisfying feedback.

---
**Status**: ✅ Ready for Implementation  
**Dependencies**: Player movement, Obstacle generation, UpgradeSystem  
**Estimated Time**: 30-45 minutes