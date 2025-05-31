# IDLE SYSTEM IMPLEMENTATION GUIDE

## Overview
This guide implements the complete idle progression system that rewards players for time spent offline.

## Prerequisites
- CoinSystem already implemented
- IdleRewardsUI components created
- Basic game systems functional

## Step 1: Create IdleManager GameObject

### 1.1 Setup IdleManager
```
1. Create empty GameObject: "IdleManager"
2. Add IdleManager.cs script
3. Position: (0, 0, 0)
4. Configure in Inspector:
   - enableIdleProgression: ✓ checked
   - idleCoinsPerSecond: 0.1
   - maxIdleHours: 8
   - maxIdleCoins: 500
   - speedUpgradeMultiplier: 0.05
```

### 1.2 Make Persistent
Ensure IdleManager survives scene changes (it's already configured as singleton).

## Step 2: Create IdleRewards UI

### 2.1 Create IdleRewardsPanel
```
1. In Canvas, create: UI > Panel "IdleRewardsPanel"
2. Set RectTransform:
   - Anchor: Center
   - Width: 600, Height: 400
   - Position: (0, 0, 0)
3. Background Image:
   - Color: (0.2, 0.2, 0.2, 0.9) - dark semi-transparent
```

### 2.2 Add Child Elements

#### Welcome Text
```
1. Create: UI > Text - TextMeshPro "WelcomeText"
2. Position: Top of panel
3. Text: "Welcome back!"
4. Font Size: 24
5. Color: White
6. Alignment: Center
```

#### Coins Earned Text
```
1. Create: UI > Text - TextMeshPro "CoinsEarnedText"  
2. Position: Center of panel
3. Text: "You earned X coins!"
4. Font Size: 28
5. Color: Yellow
6. Alignment: Center
```

#### Idle Rate Text
```
1. Create: UI > Text - TextMeshPro "IdleRateText"
2. Position: Below coins text
3. Text: "Idle rate: X coins/sec"
4. Font Size: 16
5. Color: Gray
6. Alignment: Center
```

#### Claim Button
```
1. Create: UI > Button "ClaimButton"
2. Position: Bottom left of panel
3. Size: 150x50
4. Text: "CLAIM"
5. Colors: Normal (Green), Highlighted (Light Green)
```

#### Double Rewards Button (Ad)
```
1. Create: UI > Button "DoubleRewardsButton"
2. Position: Bottom right of panel
3. Size: 150x50  
4. Text: "WATCH AD\nDOUBLE!"
5. Colors: Normal (Gold), Highlighted (Light Gold)
```

#### Coin Icon (Optional)
```
1. Create: UI > Image "CoinIcon"
2. Position: Next to coins earned text
3. Sprite: tile_coin.png
4. Size: 64x64
5. Color: Yellow
```

### 2.3 Setup IdleRewardsUI Component
```
1. Add IdleRewardsUI.cs to IdleRewardsPanel
2. Assign all UI elements in inspector:
   - idleRewardsPanel: The panel itself
   - offlineTimeText: WelcomeText
   - coinsEarnedText: CoinsEarnedText  
   - idleRateText: IdleRateText
   - claimButton: ClaimButton
   - doubleRewardsButton: DoubleRewardsButton
   - coinIcon: CoinIcon (if created)
3. Set animationDuration: 0.5
```

## Step 3: Configure Idle Calculations

### 3.1 Understanding the Formula
```
Idle Rate = Base Rate + (Speed Upgrade Level × Speed Multiplier)
Total Coins = Min(Idle Rate × Offline Seconds, Max Idle Coins)
```

### 3.2 Default Values Explained
- **Base Rate**: 0.1 coins/second = 6 coins/minute = 360 coins/hour
- **Max Time**: 8 hours = 28,800 seconds  
- **Max Coins**: 500 coins (prevents exploitation)
- **Min Time**: 30 seconds (prevents spam)

### 3.3 With Upgrades
```
Speed Upgrade Level 1: 0.1 + (1 × 0.05) = 0.15 coins/sec
Speed Upgrade Level 5: 0.1 + (5 × 0.05) = 0.35 coins/sec  
Speed Upgrade Level 10: 0.1 + (10 × 0.05) = 0.6 coins/sec
```

## Step 4: Integration with Game Flow

### 4.1 Connect to Game Start
IdleManager automatically triggers on:
- Application start (`Start()` method)
- Application focus gained (`OnApplicationFocus(true)`)
- Application unpause (`OnApplicationPause(false)`)

### 4.2 No Additional Code Needed
The system works automatically once setup is complete.

## Step 5: Testing the System

### 5.1 Quick Test (Development)
```csharp
// Add this to IdleManager for testing only
void Update()
{
    if (Input.GetKeyDown(KeyCode.T))
    {
        ForceIdleCalculation();
    }
}
```

### 5.2 Manual Time Manipulation (Testing)
```csharp
// Temporarily modify CheckIdleProgress() for testing
DateTime lastSaveTime = DateTime.Now.AddMinutes(-5); // Simulate 5 minutes offline
```

### 5.3 Real Testing Process
1. Start game, play briefly
2. Close application completely
3. Wait 1+ minutes
4. Reopen application
5. Verify idle rewards popup appears

## Step 6: Ad Integration (Optional)

### 6.1 Setup for Future Ad System
The double rewards button is ready for ad integration:
```csharp
// In IdleRewardsUI.WatchAdForDoubleRewards()
if (AdManager.Instance != null)
{
    AdManager.Instance.ShowRewardedAd((success) => {
        if (success) { /* Double rewards logic */ }
    });
}
```

### 6.2 Current Behavior
Without ad system, button falls back to normal claim.

## Step 7: Balancing and Tuning

### 7.1 Recommended Settings for Testing
```
idleCoinsPerSecond: 0.2 (faster for testing)
maxIdleHours: 4 (shorter for testing)
maxIdleCoins: 300 (achievable for testing)
```

### 7.2 Production Settings
```  
idleCoinsPerSecond: 0.05 (slower, more balanced)
maxIdleHours: 12 (longer for retention)
maxIdleCoins: 1000 (higher ceiling)
```

### 7.3 Upgrade Impact
With max speed upgrade (level 10):
- Test: 0.2 + (10 × 0.05) = 0.7 coins/sec
- Production: 0.05 + (10 × 0.05) = 0.55 coins/sec

## Step 8: Debug and Verification

### 8.1 Debug Checklist
- [ ] IdleManager saves time on app close
- [ ] Time calculation is accurate
- [ ] Rewards popup shows correct values
- [ ] Claim button adds coins properly
- [ ] Upgrade bonuses apply to idle rate
- [ ] Maximum limits are respected

### 8.2 Common Issues

**Problem**: Popup doesn't appear
**Solution**: Check minimum time threshold (30 seconds) and enableIdleProgression flag

**Problem**: Wrong coin amounts
**Solution**: Verify GetCurrentIdleRate() includes upgrade bonuses

**Problem**: Popup appears every time
**Solution**: Check if SaveCurrentTime() is being called properly

**Problem**: Time calculation incorrect
**Solution**: Verify DateTime parsing and TimeSpan.TotalSeconds usage

## Step 9: Player Experience

### 9.1 Expected Player Flow
1. Player plays game normally
2. Player minimizes/closes app
3. Player returns 10+ minutes later
4. Popup shows: "Welcome back! You were away for 12m 34s. You earned 47 coins!"
5. Player claims rewards or watches ad for double

### 9.2 Retention Benefits
- Encourages players to return after breaks
- Provides progression even when not actively playing
- Rewards longer upgrade investment
- Ads provide optional bonus monetization

## Next Steps
- Monitor idle rates for balance
- Add visual polish to rewards popup
- Consider adding idle upgrade categories
- Implement idle achievements

---
**Status**: ✅ Ready for Implementation  
**Dependencies**: CoinSystem, UpgradeSystem, UI components  
**Estimated Time**: 45-60 minutes