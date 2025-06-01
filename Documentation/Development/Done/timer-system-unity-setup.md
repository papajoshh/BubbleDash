# TIMER SYSTEM - UNITY IMPLEMENTATION GUIDE

## 🎯 OVERVIEW
Esta guía te lleva paso a paso para implementar el sistema de timer roguelite en Unity. Todos los scripts ya están creados, solo necesitas configurar GameObjects y UI.

---

## 📋 PREREQUISITES
- ✅ Unity scene con GameManager, UpgradeSystem, CoinSystem funcionando
- ✅ Scripts de timer ya copiados a Assets/Scripts/
- ✅ DOTween integrado en el proyecto
- ✅ UI Canvas existente

---

## 🔧 STEP 1: CREATE TIMER SYSTEM GAMEOBJECTS

### 1.1 TimerManager GameObject
```
1. En Hierarchy: Create Empty GameObject "TimerManager"
2. Add Component: TimerManager.cs
3. Configure en Inspector:
   ├── baseDurationSeconds: 180 (3 minutos)
   ├── enableTimer: ✓ checked
   ├── pauseTimerOnGamePause: ✓ checked
   ├── urgentWarningTime: 30
   └── warningTime: 60
4. Position: (0, 0, 0)
5. ✅ Se hace persistent automáticamente via singleton
```

### 1.2 Verify GameManager Integration
```
1. Select GameManager GameObject
2. Verify que GameManager.cs incluye la nueva integración
3. Check que OnGameOverTimer event está disponible
4. ✅ El GameManager ya está actualizado para usar TimerManager
```

---

## 🎨 STEP 2: CREATE TIMER UI

### 2.1 Timer Display Container
```
1. En UI Canvas: Right-click → UI → Empty
2. Rename a "TimerContainer"
3. Configure RectTransform:
   ├── Anchor: Top-Center (preset)
   ├── Pos X: 0, Pos Y: -50
   ├── Width: 200, Height: 80
   └── Pivot: 0.5, 1
```

### 2.2 Timer Text Element
```
1. En TimerContainer: Right-click → UI → Text - TextMeshPro
2. Rename a "TimerText"
3. Configure TextMeshPro:
   ├── Text: "03:00"
   ├── Font Size: 36
   ├── Alignment: Center Middle
   ├── Color: White (255, 255, 255, 255)
   └── Auto Size: Off
4. Configure RectTransform:
   ├── Anchor: Stretch (preset)
   ├── Left: 0, Top: 0, Right: 0, Bottom: 20
   └── Pivot: 0.5, 0.5
```

### 2.3 Timer Fill Bar (Optional)
```
1. En TimerContainer: Right-click → UI → Slider
2. Rename a "TimerFillBar"
3. Delete Handle Slide Area (no lo necesitamos)
4. Configure Slider:
   ├── Interactable: ❌ unchecked
   ├── Value: 0
   ├── Max Value: 1
   └── Whole Numbers: ❌ unchecked
5. Configure Background:
   ├── Color: Dark Gray (60, 60, 60, 255)
   └── Image Type: Filled
6. Configure Fill:
   ├── Color: Red (255, 100, 100, 255)
   └── Image Type: Filled
7. Configure RectTransform:
   ├── Anchor: Bottom-Stretch (preset)
   ├── Left: 10, Top: 0, Right: 10, Bottom: 0
   └── Height: 15
```

### 2.4 Add TimerUI Component
```
1. Select TimerContainer GameObject
2. Add Component: TimerUI.cs
3. Configure en Inspector:
   ├── Timer Text: Drag TimerText element
   ├── Timer Fill Bar: Drag Slider element (si creaste)
   ├── Timer Container: Drag TimerContainer (self-reference)
   ├── Normal Color: White (255, 255, 255, 255)
   ├── Warning Color: Yellow (255, 255, 0, 255)
   ├── Urgent Color: Red (255, 0, 0, 255)
   ├── Enable Pulse Animation: ✓ checked
   ├── Enable Shake On Urgent: ✓ checked
   ├── Pulse Scale: 1.1
   ├── Pulse Duration: 0.5
   └── Enable Timer Sounds: ✓ checked
```

---

## 🎮 STEP 3: CREATE GAME OVER TIMER UI

### 3.1 Game Over Timer Panel
```
1. En UI Canvas: Right-click → UI → Panel
2. Rename a "GameOverTimerPanel"
3. Configure Panel Image:
   ├── Color: Dark Semi-transparent (0, 0, 0, 200)
   └── Raycast Target: ✓ checked
4. Configure RectTransform:
   ├── Anchor: Stretch (preset)
   ├── Left: 0, Top: 0, Right: 0, Bottom: 0
   └── Pivot: 0.5, 0.5
5. Initially: SetActive(false)
```

### 3.2 Main Container
```
1. En GameOverTimerPanel: Right-click → UI → Empty
2. Rename a "MainContainer"
3. Configure RectTransform:
   ├── Anchor: Center (preset)
   ├── Pos X: 0, Pos Y: 0
   ├── Width: 600, Height: 700
   └── Pivot: 0.5, 0.5
4. Add Component: Vertical Layout Group
   ├── Spacing: 20
   ├── Child Alignment: Upper Center
   ├── Control Child Size Width: ❌
   ├── Control Child Size Height: ❌
   ├── Use Child Scale: ✓ checked
   └── Child Force Expand: Width ✓, Height ❌
```

### 3.3 Title Text
```
1. En MainContainer: Right-click → UI → Text - TextMeshPro
2. Rename a "TitleText"
3. Configure TextMeshPro:
   ├── Text: "TIME'S UP!"
   ├── Font Size: 48
   ├── Alignment: Center Middle
   ├── Color: Red (255, 100, 100, 255)
   └── Font Style: Bold
4. Add Component: Layout Element
   ├── Min Height: 80
   └── Preferred Height: 80
```

### 3.4 Stats Container
```
1. En MainContainer: Right-click → UI → Empty
2. Rename a "StatsContainer"
3. Add Component: Vertical Layout Group
   ├── Spacing: 15
   ├── Child Alignment: Upper Center
   └── Child Force Expand: Width ✓, Height ❌
4. Add Component: Layout Element
   ├── Min Height: 300
   └── Flexible Height: 1
```

### 3.5 Individual Stat Texts
```
Para cada stat (RunTime, FinalScore, CoinsEarned, HighScore):

1. En StatsContainer: Right-click → UI → Text - TextMeshPro
2. Rename a "[StatName]Text" (ej: "RunTimeText")
3. Configure TextMeshPro:
   ├── Text: "Run Time: 03:45"
   ├── Font Size: 24
   ├── Alignment: Center Middle
   ├── Color: White (255, 255, 255, 255)
   └── Auto Size: Off
4. Add Component: Layout Element
   ├── Min Height: 35
   └── Preferred Height: 35

Repetir para:
- RunTimeText
- FinalScoreText
- CoinsEarnedText
- HighScoreText
```

### 3.6 Detailed Stats (Optional)
```
1. En StatsContainer: Right-click → UI → Empty
2. Rename a "DetailedStatsContainer"
3. Add Component: Vertical Layout Group (spacing: 10)
4. Add Component: Layout Element (Min Height: 100)

Crear TextMeshPro children:
- BubblesPopedText: "Bubbles Popped: --"
- MaxComboText: "Max Combo: --"  
- DistanceTraveledText: "Distance: --"
(Font Size: 18, Color: Light Gray)
```

### 3.7 Buttons Container
```
1. En MainContainer: Right-click → UI → Empty
2. Rename a "ButtonsContainer"
3. Add Component: Horizontal Layout Group
   ├── Spacing: 20
   ├── Child Alignment: Middle Center
   └── Child Force Expand: Width ✓, Height ❌
4. Add Component: Layout Element
   ├── Min Height: 60
   └── Preferred Height: 60
```

### 3.8 Action Buttons
```
Para cada botón (PlayAgain, Upgrades, MainMenu):

1. En ButtonsContainer: Right-click → UI → Button - TextMeshPro
2. Rename a "[Action]Button" (ej: "PlayAgainButton")
3. Configure Button:
   ├── Interactable: ✓ checked
   ├── Transition: Color Tint
   ├── Normal Color: Light Gray (220, 220, 220, 255)
   ├── Highlighted Color: White (255, 255, 255, 255)
   ├── Pressed Color: Gray (180, 180, 180, 255)
   └── Selected Color: Light Gray (220, 220, 220, 255)
4. Configure Button Text:
   ├── Text: "PLAY AGAIN" / "UPGRADES" / "MAIN MENU"
   ├── Font Size: 18
   ├── Alignment: Center Middle
   ├── Color: Black (0, 0, 0, 255)
   └── Font Style: Bold
5. Add Component: Layout Element
   ├── Min Width: 120
   └── Preferred Width: 150

Crear los 3 botones:
- PlayAgainButton
- UpgradesButton  
- MainMenuButton
```

### 3.9 Add GameOverTimerUI Component
```
1. Select GameOverTimerPanel GameObject
2. Add Component: GameOverTimerUI.cs
3. Configure en Inspector:
   ├── Game Over Timer Panel: Drag GameOverTimerPanel (self)
   ├── Title Text: Drag TitleText
   ├── Run Time Text: Drag RunTimeText
   ├── Final Score Text: Drag FinalScoreText
   ├── Coins Earned Text: Drag CoinsEarnedText
   ├── High Score Text: Drag HighScoreText
   ├── Play Again Button: Drag PlayAgainButton
   ├── Upgrades Button: Drag UpgradesButton
   ├── Main Menu Button: Drag MainMenuButton
   ├── Stats Container: Drag StatsContainer
   ├── Bubbles Poped Text: Drag BubblesPopedText
   ├── Max Combo Text: Drag MaxComboText
   ├── Distance Traveled Text: Drag DistanceTraveledText
   ├── Animation Delay: 1
   └── Element Delay: 0.2
```

---

## ⚙️ STEP 4: UPGRADE SYSTEM INTEGRATION

### 4.1 Verify Head Start Upgrade
```
1. Start Play Mode
2. Press 'C' key → Add test coins
3. Press 'U' key → Open upgrade menu
4. Buy "Head Start" upgrade
5. Check Console: Should show "HeadStartBonus" being saved
6. Restart game
7. Timer should show 3:30 instead of 3:00 (if level 1)
```

### 4.2 Test Timer Progression
```
Level 0: 3:00 (180s base)
Level 1: 3:30 (180s + 30s)
Level 2: 4:00 (180s + 60s)
Level 3: 4:30 (180s + 90s)
```

---

## 🧪 STEP 5: TESTING & VALIDATION

### 5.1 Timer Functionality Test
```
✅ BASIC TIMER:
1. Start game → Timer should show 3:00 and count down
2. Pause game (ESC) → Timer should pause
3. Resume game → Timer should continue
4. Wait for timer to expire → Game Over Timer screen should appear

✅ WARNING STATES:
1. Let timer reach 1:00 → Text should turn yellow and pulse
2. Let timer reach 0:30 → Text should turn red, pulse faster, shake
3. Timer expiration → "TIME'S UP!" screen appears

✅ GAME OVER SCREEN:
1. Timer expires → GameOverTimerPanel activates
2. Check all stats are populated correctly
3. "Play Again" → Should restart with full timer
4. "Upgrades" → Should open upgrade menu
5. Stats animation → Should animate in sequence
```

### 5.2 Integration Test
```
✅ UPGRADE INTEGRATION:
1. Buy Head Start upgrade → Timer should extend next run
2. Multiple Head Start levels → Timer extension should stack
3. Timer persists through upgrade menu → Should keep counting

✅ COIN SYSTEM:
1. Timer Game Over → Should award coins based on score
2. Coin amount = Final Score * 0.1 (minimum 5)
3. Coins should be added to total coin count
```

### 5.3 Performance Test
```
✅ SMOOTH OPERATION:
1. Timer UI updates smoothly every frame
2. No frame drops during warning animations
3. Pause/resume responsive
4. DOTween animations clean (no memory leaks)
```

---

## 🎨 STEP 6: VISUAL POLISH (Optional)

### 6.1 Enhanced Timer Display
```
CIRCULAR TIMER OPTION:
1. Replace timer fill bar with circular progress
2. Use UI → Image with "Filled" type and "Radial 360" fill method
3. Rotate starting angle to top (90 degrees)
4. Update TimerUI.cs to use circular progress

GLOW EFFECTS:
1. Add Outline component to TimerText for glow
2. Configure: Distance 2, Color matching timer state
3. Animate outline in warning/urgent states
```

### 6.2 Enhanced Game Over Screen
```
BACKGROUND BLUR:
1. Add UI → Raw Image behind MainContainer
2. Use blurred screenshot or gradient background
3. Animate fade-in for dramatic effect

PARTICLE EFFECTS:
1. Add 2D particle system for dramatic Game Over
2. Configure for scattered coins or time pieces
3. Trigger on timer expiration
```

---

## 🔧 STEP 7: TROUBLESHOOTING

### 7.1 Common Issues

**❌ Timer not starting:**
```
Solution: Check GameManager.StartGame() calls TimerManager.StartTimer()
Verify: TimerManager.enableTimer = true
```

**❌ Timer UI not updating:**
```
Solution: Verify TimerUI event subscriptions in Start()
Check: TimerManager.OnTimerChanged event is being called
```

**❌ Game Over screen not appearing:**
```
Solution: Check GameManager.OnGameOverTimer event subscription
Verify: GameOverTimerPanel.SetActive(false) initially
```

**❌ Animations not working during pause:**
```
Solution: Ensure all DOTween calls use .SetUpdate(true)
Check: Time.timeScale doesn't affect timer DOTween
```

**❌ Head Start upgrade not working:**
```
Solution: Verify UpgradeSystem saves "HeadStartBonus" to PlayerPrefs
Check: TimerManager reads PlayerPrefs in CalculateTotalDuration()
```

### 7.2 Debug Commands

**Console Commands for Testing:**
```csharp
// En TimerManager, add for debug:
[ContextMenu("Add 30 Seconds")]
void DebugAddTime() { AddTime(30f); }

[ContextMenu("Set Warning State")]  
void DebugWarning() { currentTime = 45f; }

[ContextMenu("Set Urgent State")]
void DebugUrgent() { currentTime = 15f; }

[ContextMenu("Force Expire")]
void DebugExpire() { ExpireTimer(); }
```

---

## ✅ SUCCESS CRITERIA

### Must Work:
- [ ] Timer counts down from 3:00 to 0:00
- [ ] Visual warnings at 1:00 (yellow) and 0:30 (red)
- [ ] Game Over screen appears when timer expires
- [ ] Head Start upgrade extends timer duration
- [ ] Timer integrates with existing game systems
- [ ] Pause/resume functionality works correctly

### Should Work:
- [ ] Smooth animations and transitions
- [ ] Proper coin rewards calculation
- [ ] Stats display correctly on Game Over
- [ ] Button navigation functions properly

### Nice to Have:
- [ ] Sound effects for timer warnings
- [ ] Visual effects for urgency states
- [ ] Circular progress display option

---

## 📚 FILE REFERENCES

**Scripts Created:**
- `/Assets/Scripts/Managers/TimerManager.cs`
- `/Assets/Scripts/UI/TimerUI.cs`
- `/Assets/Scripts/UI/GameOverTimerUI.cs`

**Scripts Modified:**
- `/Assets/Scripts/Managers/GameManager.cs` (added timer integration)
- `/Assets/Scripts/Managers/UpgradeSystem.cs` (added Head Start timer bonus)

**Documentation:**
- `/Documentation/Development/sprint1-autonomous-decisions.md`
- `/Documentation/Development/timer-system-unity-setup.md` (this file)

---

**STATUS**: ✅ Ready for Unity Implementation  
**ESTIMATED TIME**: 45-60 minutes  
**COMPLEXITY**: Medium - UI setup intensive  
**DEPENDENCIES**: TimerManager, GameManager, UpgradeSystem integration complete