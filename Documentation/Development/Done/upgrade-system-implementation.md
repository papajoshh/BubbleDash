# UPGRADE SYSTEM IMPLEMENTATION GUIDE

## Overview
This guide walks you through setting up the upgrade system in Unity. All code integrations are already implemented - you just need to create GameObjects and configure them.

## Prerequisites
- Unity scene with basic player movement
- UI Canvas created
- UpgradeUI prefabs created (from unity-setup guide)

## Step 1: Create System GameObjects

### 1.1 UpgradeSystem GameObject
```
1. In Hierarchy: Create Empty GameObject "UpgradeSystem"
2. Add Component: UpgradeSystem.cs
3. Position: (0, 0, 0)
4. ✅ System is auto-persistent via singleton pattern
```

### 1.2 CoinSystem GameObject  
```
1. In Hierarchy: Create Empty GameObject "CoinSystem"
2. Add Component: CoinSystem.cs
3. Configure in Inspector:
   - coinPrefab: (will assign after creating coin prefab)
   - coinSpawnChance: 0.3
   - minCoinsPerSpawn: 1
   - maxCoinsPerSpawn: 3
   - coinSpacing: 1
   - usePatterns: ✓ checked
```

### 1.3 UpgradeSystemHelper GameObject (Optional - For Testing)
```
1. In Hierarchy: Create Empty GameObject "UpgradeHelper"
2. Add Component: UpgradeSystemHelper.cs
3. Configure in Inspector:
   - enableTestControls: ✓ checked (for development)
   - showDebugInfo: ✓ checked (for development)
```

## Step 2: Create Coin Prefab

### 2.1 Coin GameObject Setup
```
1. In Hierarchy: Create Empty GameObject "Coin"
2. Add SpriteRenderer Component:
   - Sprite: tile_coin.png (from Assets/Art/Sprites/)
   - Color: Yellow (1, 1, 0, 1)
   - Sorting Layer: Default
   - Order in Layer: 5
3. Transform Scale: (0.8, 0.8, 1)
```

### 2.2 Coin Physics
```
1. Add CircleCollider2D:
   - Is Trigger: ✓ checked
   - Radius: 0.3
2. Add Component: Coin.cs
3. Configure Coin.cs in Inspector:
   - coinValue: 1
   - rotationSpeed: 180
   - bobSpeed: 2
   - bobHeight: 0.2
   - magnetRange: 3
   - magnetSpeed: 8
   - lifetime: 20
```

### 2.3 Save as Prefab
```
1. Drag Coin GameObject to Assets/Prefabs/ folder
2. Name: "CoinPrefab"
3. Delete original from scene
4. Assign CoinPrefab to CoinSystem.coinPrefab in Inspector
```

## Step 3: Connect Upgrade UI

### 3.1 Link Upgrade Button
```
1. Select your UIManager GameObject
2. In UIManager.cs Inspector:
   - Drag your upgrade button to "upgradeButton" field
3. ✅ Button functionality is already coded
```

### 3.2 Setup UpgradeUI GameObject
```
1. Find your UpgradePanel GameObject (created in unity-setup)
2. Add Component: UpgradeUI.cs  
3. Configure UpgradeUI.cs in Inspector:
   - upgradePanel: The panel GameObject itself
   - upgradeMenuButton: Your upgrade button from UIManager
   - closeUpgradeButton: Close button inside panel
   - upgradeContainer: Container for upgrade items
   - totalCoinsText: Text element showing coins
   - selectedUpgradeInfo: Text for feedback messages
```

## Step 4: Verify Player Setup

### 4.1 Player Tag
```
1. Select Player GameObject
2. In Inspector, set Tag: "Player"
3. If "Player" tag doesn't exist:
   - Click Tag dropdown > "Add Tag..."
   - Create "Player" tag
```

### 4.2 Player Components Check
```
✅ PlayerController.cs should have:
   - baseSpeed = 3f (upgradeable)
   
✅ BubbleShooter.cs should have:
   - shootCooldown = 0.2f (upgradeable)
   
✅ MomentumSystem.cs should have:
   - speedIncreasePerHit = 0.1f (upgradeable)
```

## Step 5: Test the System

### 5.1 Quick Test (Play Mode)
```
1. Start Play Mode
2. Press 'C' key → Adds 100 test coins
3. Press '1' key → Buys speed upgrade
4. Press '2' key → Buys fire rate upgrade
5. Press 'I' key → Shows current upgrade status
6. Check console for upgrade confirmations
```

### 5.2 UI Test
```
1. Start Play Mode  
2. Click upgrade button → Menu should open with pause
3. Try purchasing upgrades (need coins first)
4. Close menu → Game should resume
5. Restart game → Upgrades should persist
```

## Step 6: Upgrade Effects Verification

### 6.1 Speed Upgrade Test
```
1. Buy speed upgrade (press '1' or via UI)
2. Check Player Inspector: baseSpeed should increase
3. Player should move faster
```

### 6.2 Fire Rate Test  
```
1. Buy fire rate upgrade (press '2' or via UI)
2. Check BubbleShooter Inspector: shootCooldown should decrease
3. Shooting should be faster
```

### 6.3 Coin Magnet Test
```
1. Buy coin magnet upgrade (press '4' or via UI)
2. Spawn coins near player
3. Collection range should be larger
```

## Step 7: Balancing and Polish

### 7.1 Starting Coins (For Testing)
```
Add to PlayerController.Start() temporarily:
if (CoinSystem.Instance != null)
    CoinSystem.Instance.AddCoins(200); // Starting coins for testing
```

### 7.2 Upgrade Prices
Current prices are balanced for ~5 minute sessions:
- Speed Boost: 50, 75, 100, 125... coins
- Fire Rate: 75, 110, 145, 180... coins  
- Momentum: 100, 150, 200, 250... coins
- Coin Magnet: 60, 90, 120, 150... coins
- Head Start: 150, 225, 300... coins

## Step 8: Debug Tools

### 8.1 UpgradeSystemHelper Controls
```
Development hotkeys (only in editor):
- C: Add 100 coins
- 1-5: Buy specific upgrades
- Shift+R: Reset all upgrades  
- I: Show upgrade status in console
```

### 8.2 Inspector Context Menus
```
Right-click UpgradeSystemHelper in Inspector:
- "Give 100 Test Coins"
- "Test Speed Upgrade"  
- "Show All Upgrade Status"
- "Reset All Upgrades"
- "Max All Upgrades (Cheat)"
```

## Troubleshooting

### Common Issues
**Problem**: Upgrade button doesn't work
**Solution**: Check UIManager.upgradeButton is assigned

**Problem**: Coins don't spawn  
**Solution**: Verify CoinSystem.coinPrefab is assigned

**Problem**: Upgrades don't save
**Solution**: Check PlayerPrefs in Edit > Project Settings > Player

**Problem**: Magnet not working
**Solution**: Ensure player has "Player" tag

**Problem**: No coins to buy upgrades
**Solution**: Use test controls (C key) or add starting coins

## Success Criteria
- [ ] Can open/close upgrade menu
- [ ] Can purchase upgrades with coins
- [ ] Speed upgrade makes player faster
- [ ] Fire rate upgrade shoots faster  
- [ ] Magnet upgrade increases collection range
- [ ] Upgrades persist after restart
- [ ] Starting combo applies on new games

---
**Status**: ✅ Ready for Unity Implementation  
**Dependencies**: unity-setup-complete.md, prefabs created  
**Code**: All integrations complete, just needs Unity setup  
**Estimated Time**: 20-30 minutes